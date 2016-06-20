using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using ExitGames.Client.Photon;

public class CManager{
	private HashSet<int> currentPlayers = new HashSet<int>();
	private LinkedList<int> queue = new LinkedList<int>();
	private Hashtable playerData = new Hashtable();

	private int player1ID = -1;
	private int player2ID = -1;

	public volatile bool isRunning = false;
	private bool toStop = false;
	private bool inMatch = false;

	private int roundTime = 120; //seconds

	private bool wasDeathListEnabled = false;

	private GameObject[] titanRespawns;

	private void updatePlayerList(){
		HashSet<int> tmp = new HashSet<int>();
        
        foreach(int i in this.currentPlayers) {
            tmp.Add(i);
        }

		foreach(PhotonPlayer player in PhotonNetwork.playerList){
			if (!currentPlayers.Contains(player.ID)){
				currentPlayers.Add(player.ID);

				this.handleJoin(player);
			}
			
			tmp.Remove(player.ID);
		}

		foreach(int i in tmp){
			this.currentPlayers.Remove(i);
			this.handleQuit(i);
		}

	}

	public string getInfoString(){
		string res = "";
		res += "player1ID: " + this.player1ID + "\n" +
			"player2ID: " + this.player2ID + "\n" +
			"Queue: ";
		int i = 1;
		foreach(int j in this.queue){
			res += "(" + i + ")" + j +", ";
		}
		res += "\n" +
			"roundTime: "+ this.roundTime;

		return res;
	}

	public void start(){
		if (!this.isRunning){
			this.wasDeathListEnabled = ModMain.instance.getModMainThread().isDeathListEnabled();
			//get the current online players and add them to the list.
			//this.updatePlayerList();/
			//the player list is updated on the loop
			this.currentPlayers.Clear();
			this.queue.Clear();
			this.playerData.Clear();

			this.titanRespawns = GameObject.FindGameObjectsWithTag("titanRespawn");

			new Thread(this.run).Start();
		}
	}

	private void handleQuit(int playerID){
		ModMain.instance.sendToAll("{Debug} Player #" + playerID + " left the game.");
		if (this.inMatch){
			if (playerID == player1ID){
				this.handleWin(player2ID);
				player1ID = -1;
			}else if (playerID == player2ID){
				this.handleWin(player1ID);
				player2ID = -1;
			}
		}

		this.queue.Remove(playerID);
		this.playerData.Remove(playerID);
	}

	private void handleWin(int playerID){
		PlayerData p1Data = (PlayerData) this.playerData[player1ID];
		PlayerData p2Data = (PlayerData) this.playerData[player2ID];

		if (player1ID == playerID){//player1 won
			p1Data.Wins += 1;
			p2Data.Losses += 1;

			ModMain.instance.sendToAll("{Debug} Player #" + player1ID + " won the match");
	
			this.queue.AddLast(player2ID);
			this.player2ID = -1;
		}else{//player 2 won
			p2Data.Wins += 1;
			p1Data.Losses += 1;

			ModMain.instance.sendToAll("{Debug} Player #" + player2ID + " won the match");

			this.queue.AddLast(player1ID);
			this.player1ID = -1;
		}
	}

	private void handleTie(){
		PlayerData p1Data = (PlayerData) this.playerData[player1ID];
		PlayerData p2Data = (PlayerData) this.playerData[player2ID];

		p1Data.Losses += 1;
		p2Data.Losses += 1;

		//swap them so that they might not face eachother again
		this.queue.AddLast(this.player2ID);
		this.queue.AddLast(this.player1ID);

		this.player2ID = -1;
		this.player1ID = -1;

		ModMain.instance.sendToAll("{Debug} Draw, everybody loses. Choosing new players.");
	}

	private void handleJoin(PhotonPlayer player){
		this.playerData[player.ID] = new PlayerData();
		this.queue.AddLast(player.ID);
		ModMain.instance.sendToAll("{Debug} Player #" + player.ID + " has joined the game, his place on the queue is: " + this.queue.Count);
	}

	public void handleKill(TITAN titan, int playerID, int damage){
		PlayerData data = (PlayerData) this.playerData[playerID];
		data.Kills += 1;
		data.TotalDamage += damage;
		ModMain.instance.sendToAll("{Debug} Player #" + playerID + " has killed a titan.");

		if (!titan.hasDie){
			lock(titan){
				titan.photonView.RPC("netDie", PhotonTargets.All, new object[0]);
			}

			lock(ModMain.instance.getGameManager()){
                ModMain.instance.getGameManager().photonView.RPC("titanGetKill", PhotonNetwork.player, PhotonPlayer.Find(playerID), damage, titan.name);
			}
		}

		this.spawnTitans(1);
	}

	public bool hasFinished(){
		return !this.isRunning;
	}

	public bool isInMatch(){
		return this.inMatch;
	}

	public void stop(){
		this.toStop = true;
	}

	private void end(){
		ModMain.instance.getModMainThread().setDeathListEnabled(this.wasDeathListEnabled);
		this.wasDeathListEnabled = false;

		this.queue.Clear();
		this.currentPlayers.Clear();
	}

	private void run(){
		this.isRunning = true;

		//main loop
		try{
		while(!this.toStop){
			this.updatePlayerList();
			this.checkAlivePlayers();

			if (this.currentPlayers.Count < 2){
				if (player1ID == -1){
					this.player1ID = this.getNextInQueue();
				}

				Thread.Sleep(1000);
				continue;
			}

			if (this.player1ID == -1){
				this.player1ID = this.getNextInQueue();
			}

			if (this.player2ID == -1){
				this.player2ID = this.getNextInQueue();
			}

			ModMain.instance.sendToAll("{Debug} Next duel:");
			ModMain.instance.sendToAll("{Debug} Player 1: #" + this.player1ID);
			ModMain.instance.sendToAll("{Debug} Player 2: #" + this.player2ID);

			Thread.Sleep(2000);

			ModMain.instance.sendToAll("{Debug} Restarting game.");
			this.restartGame();

			this.spawnTitans(5);

			ModMain.instance.sendToAll("{Debug} Entering match loop.");
			this.doMatchLoop();

			Thread.Sleep(1000);
		}
		}catch(System.Exception e){
			ModMain.instance.log(e);
		}


		this.end();

		this.isRunning = false;
		this.toStop = false;
	}

	private int getNextInQueue(){
		int next = this.queue.First.Value;

		this.queue.RemoveFirst();

		return next;
	}

	private void doMatchLoop(){
		int seconds = 0;
		this.inMatch = true;

		while(!this.toStop){
			this.updatePlayerList();
			this.checkAlivePlayers();

			//maybe one of the players left.
			if (player1ID == -1 || player2ID == -1){
				break;
			}

			if (seconds >= this.roundTime){
				ModMain.instance.sendToAll("{Debug} Round is over.");

				PlayerData p1Data = (PlayerData) this.playerData[player1ID];
				PlayerData p2Data = (PlayerData) this.playerData[player2ID];
				//score = averageDamage * killsPerMinute
				double p1Score = (p1Data.TotalDamage / (double) p1Data.Kills) * (p1Data.Kills / (seconds / 60D));
				double p2Score = (p2Data.TotalDamage / (double) p2Data.Kills) * (p2Data.Kills / (seconds / 60D));

				ModMain.instance.sendToAll("{Debug} Player #" + this.player1ID + " score: " + p1Score);
				ModMain.instance.sendToAll("{Debug} Player #" + this.player2ID + " score: " + p2Score);

				if (p1Score > p2Score){
					this.handleWin(player1ID);
				}else if (p2Score > p1Score){
					this.handleWin(player2ID);
				}else{
					this.handleTie();
				}

				break;
			}
			seconds++;
			Thread.Sleep(1000);
		}

		this.inMatch = false;
	}

	private void checkAlivePlayers(){
		foreach(HERO h in GameObject.FindObjectsOfType<HERO>()){
			int id = h.photonView.ownerId;

			if (this.inMatch && id != this.player1ID && id != this.player2ID){
				h.photonView.RPC("netDie2", PhotonTargets.All, new object[]{-1, "Championship controller"});
			}
		}
	}

	private void restartGame(){
		ModMain.instance.sendToAll("{Debug} Cleaning titans.");

		foreach(TITAN t in GameObject.FindObjectsOfType<TITAN>()){
			lock(t){
				t.photonView.RPC("netDie", PhotonTargets.All, new object[0]);
			}
		}

		ModMain.instance.sendToAll("{Debug} Killing all players.");
		foreach(HERO h in GameObject.FindObjectsOfType<HERO>()){
			lock(h){
				h.photonView.RPC("netDie2", PhotonTargets.All, new object[]{-1, "Championship controller"});
			}
		}

		Thread.Sleep(1000);

		ModMain.instance.sendToAll("{Debug} Respawning duel players.");

		try{
			this.updateLosingFieldInGM(ModMain.instance.getGameManager());
		}catch(System.Exception e){
			ModMain.instance.log(e);
		}

		this.respawnTheTwoPlayers();
	}

	private void spawnTitans(int amount){
		ModMain.instance.sendToAll("{Debug} Spawning " + amount + " titan(s)");
		//ModMain.instance.getGameManager().amountOfTitansToSpawn += amount;
		/*
		for(int i = 0; i < amount; i++){
			GameObject pos = this.titanRespawns[Random.Range(0, this.titanRespawns.Length)];

			lock(typeof(PhotonNetwork)){
				PhotonNetwork.Instantiate("TITAN_VER3.1", pos.transform.position, Quaternion.Euler(0, -90, 0), 0);	}

			//give it some time
			Thread.Sleep(100);
		}
		*/
	}

	public void handlePlayerDeath(int id){
		if (this.inMatch){
			if (id == player1ID){
				ModMain.instance.sendToAll("{Debug} Player 1 lost. Lost by dying.");
				this.handleWin(this.player2ID);
			}else if (id == player2ID){
				ModMain.instance.sendToAll("{Debug} Player 2 lost. Lost by dying.");
				this.handleWin(this.player1ID);
			}
		}
	}

	private void updateLosingFieldInGM(FengGameManagerMKII gm){
		lock(gm){
			FieldInfo isLosingField = gm.GetType().GetField("isLosing", BindingFlags.Instance | BindingFlags.NonPublic);
			isLosingField.SetValue(gm, false);
		}
	}

	private void respawnTheTwoPlayers(){
		PhotonPlayer p1 = null;
		PhotonPlayer p2 = null;
		foreach(PhotonPlayer p in PhotonNetwork.playerList){
			if (p.ID == this.player1ID){
				p1 = p;
			}else if (p.ID == this.player2ID){
				p2 = p;
			}
		}
		if (p1 != null){
			lock(ModMain.instance.getGameManager().photonView){
				ModMain.instance.getGameManager().photonView.RPC("respawnHeroInNewRound", p1, new object[0]);
			}
		}
		if (p2 != null){
			lock(ModMain.instance.getGameManager().photonView){
				ModMain.instance.getGameManager().photonView.RPC("respawnHeroInNewRound", p2, new object[0]);
			}
		}
	}

}

