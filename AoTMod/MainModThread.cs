using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class MainModThread{
	private Hashtable bannedNames = new Hashtable();
    private Hashtable crashMap = new Hashtable();

    private List<int> currentBeingCrashed = new List<int>();

	private List<int> deathList = new List<int>();
	private List<int> destroyList  = new List<int>();
    

	private readonly object bannedLock = new object();
	private readonly object deathLock = new object();
	private readonly object destroyLock = new object();

	private volatile bool deathListEnabled = false;
	private volatile bool destroyListEnabled = false;
    private volatile bool crashListEnabled = false;

	private Hashtable nullNamePasses = new Hashtable();
	private int allowedPasses = 2;

	private bool toStop = false;
	private bool running = false;

	public volatile bool autoRespawnEnabled = false;

	private FengGameManagerMKII gm;
	private ModMain mod;

    private ExitGames.Client.Photon.Hashtable playerProperties = null;

	public MainModThread (ModMain mod) {
		this.mod = mod;
	}
	
	public void banName(string name, BanType type){
		lock(bannedLock){
			bannedNames[ModMain.stripColorCodes(name)] = type;
		}
	}

	public void unban(int index){
        string[] keys = new string[bannedNames.Count];
		bannedNames.Keys.CopyTo(keys, 0);
        BanType type = (BanType) this.bannedNames[keys[index]];
        List<object> banHashKeys = new List<object>(FengGameManagerMKII.banHash.Keys);
        
        foreach(object key in banHashKeys) {
            string stripped = ModMain.stripColorCodes((string) FengGameManagerMKII.banHash[key]);

            if (type == BanType.CONTAINS_PART) {
                if (stripped.Contains(keys[index])) {
                    FengGameManagerMKII.banHash.Remove(key);
                }
            } else if (type == BanType.FULL_NAME) {
                if (stripped.Equals(keys[index])) {
                    FengGameManagerMKII.banHash.Remove(key);
                }
            }
        }
        

		lock(bannedLock){
			bannedNames.Remove(keys[index]);
		}
	}

	public Hashtable getBannedNames(){
		return this.bannedNames;
	}

	public void start(){
		if (!this.running){
			new Thread(this.run).Start();
			this.gm = this.mod.getGameManager();
		}
	}
	private void run(){
		this.running = true;
		bool dead = false;
		try{
			while(!this.toStop){
				if (PhotonNetwork.connected){
					bool hasDeadPlayers = false;

                    if (this.didSomeoneTamperTheScoreboard()) {
                        this.updateScoreboardHashtable();
                    }

                    this.doDestroyCheck();

					foreach(PhotonPlayer player in PhotonNetwork.playerList){
						object playerIsDead = player.customProperties[PhotonPlayerProperty.dead];
                        string name = (string) player.customProperties[PhotonPlayerProperty.name];
                        name = (name == null) ? string.Empty : name;
                        name = name.Trim();
                        string stripped = ModMain.stripColorCodes(name);

                        if (PhotonNetwork.isMasterClient) {
                            if ((name != null && name != string.Empty) || !canPass(player.ID, name)) {
                                if (isBanned(stripped) && !FengGameManagerMKII.ignoreList.Contains(player.ID)) {
                                    //FengGameManagerMKII.instance.kickPlayerRC(player, true, "[Public Security Mod] Name banned");
                                    PhotonNetwork.CloseConnection(player);
                                    continue;
                                } else if (this.crashListEnabled && !this.currentBeingCrashed.Contains(player.ID) && this.isToCrash(stripped)) {
                                    this.currentBeingCrashed.Add(player.ID);
                                    CrashCmd.doCrash3(player.ID);
                                }
                            }
                        }

                        if (this.deathListEnabled) {
                            foreach (int id in this.deathList) {
                                if (PhotonPlayer.Find(id) == null) {
                                    this.deathList.Remove(id);
                                }
                            }

                            if (this.deathList.Count != 0) {
                                ModMain.instance.getTaskManager().addLateUpdateTask(new DeathCheckTask(this.deathList));
                            }
                        }

                        playerIsDead = (playerIsDead == null) ? true : playerIsDead;

						if (((bool) playerIsDead)){
							hasDeadPlayers = true;
						}
					}

					if (this.autoRespawnEnabled && hasDeadPlayers){
						ModMain.instance.getTaskManager().addLateUpdateTask(new RespawnTask());
					}
				}

				Thread.Sleep(1000);
			}
		}catch(System.Exception e){
            int i = 0;
            i+= 10;
			dead = true;
			ModMain.instance.log("Main mod thread died. Exception: ");
			ModMain.instance.log(e);
			ModMain.instance.log("Restarting Thread.");
            Thread.Sleep(500);
		}
		this.running = false;

		if (dead){
			this.start();
		}
	}

    public bool isToCrash(string name) {
        foreach (string crashName in this.crashMap.Keys) {
            switch ((BanType)crashMap[name]) {
                case BanType.CONTAINS_PART:
                    if (name.Contains(crashName)) {
                        return true;
                    }
                    break;
                case BanType.FULL_NAME:
                    if (name.Equals(crashName)) {
                        return true;
                    }
                    break;
                case BanType.GUILD_FULL:
                    return false;
                //@TODO
                //banned = stripped.Equals(bannedName);
                case BanType.GUILD_PART:
                    return false;
                //@TODO
                //banned = stripped.Equals(bannedName);
            }
        }

        return false;
    }

	public bool isBanned(string stripped){
		foreach(string bannedName in bannedNames.Keys){
			switch((BanType)bannedNames[bannedName]){
				case BanType.CONTAINS_PART:
					if(stripped.Contains(bannedName)){
						return true;
					}
					break;
				case BanType.FULL_NAME:
					if (stripped.Equals(bannedName)){
						return true;
					}
					break;
				case BanType.GUILD_FULL:
					return false;
					//@TODO
					//banned = stripped.Equals(bannedName);
				case BanType.GUILD_PART:
					return false;
					//@TODO
					//banned = stripped.Equals(bannedName);
			}
		}

		return false;
	}

    public void addToCrashMap(string name, BanType type) {
        this.crashMap[name] = type;
    }

    public void removeFromCrash(string name) {
        this.crashMap.Remove(name);
    }

    public bool isAutoCrashEnabled() {
        return this.crashListEnabled;
    }

    public Hashtable getCrashTable() {
        return this.crashMap;
    }
    public void toggleAutoCrash(bool value) {
        this.crashListEnabled = value;
    }

	public bool isDeathListEnabled(){
		return this.deathListEnabled;
	}

	public void setDeathListEnabled(bool val){
		this.deathListEnabled = val;
	}

	public List<int> getDeathList(){
		return this.deathList;
	}

	public void removeDeathList(int item){
		lock(this.deathLock){
			this.deathList.Remove(item);
		}
	}

	public void addToDeathList(int id){
		lock(this.deathLock){
			this.deathList.Add(id);
		}
	}
	private bool canPass(int id, string name){
		if (name == null || name == string.Empty){
			if (this.nullNamePasses.ContainsKey(id)){
				this.nullNamePasses[id] = (int)this.nullNamePasses[id] + 1;
				
				if ((int)this.nullNamePasses[id] > this.allowedPasses){
					this.nullNamePasses.Remove(id);
					return false;
				}else{
					return true;
				}
			}else{
				this.nullNamePasses[id] = 0;
				return true;
			}

		}else if (this.nullNamePasses.ContainsKey(id)){
			this.nullNamePasses.Remove(id);
			return true;
		}else{
			return true;
		}
	}

	private void doDestroyCheck(){
		lock(this.destroyLock){
			foreach(int id in this.destroyList){
				if (PhotonPlayer.Find(id) == null){
					this.destroyList.Remove(id);
					continue;
				}

				PhotonNetwork.DestroyPlayerObjects(id);
			}
		}
	}


	public bool isToStop(){
		return this.toStop;
	}

	public void stop(){
		this.toStop = true;
	}

	public bool isRunning(){
		return this.running;
	}

	public void addToDestroyList(int id){
		lock(this.destroyLock){
			this.destroyList.Add(id);
		}
	}

	public void removeFromDestroyList(int id){
		lock(this.destroyLock){
			this.destroyList.Remove(id);
		}
	}

	public bool DestroyListEnabled{
		get{
			return this.destroyListEnabled;
		}

		set {
			this.destroyListEnabled = value;
		}
	}

	public List<int> getDestroyList(){
		return this.destroyList;
	}

    public void updateInternalPlayerProperties() {
        this.playerProperties = new ExitGames.Client.Photon.Hashtable();
        this.updatePlayerProperty(PhotonPlayerProperty.kills, 0);
        this.updatePlayerProperty(PhotonPlayerProperty.deaths, 0);
        this.updatePlayerProperty(PhotonPlayerProperty.max_dmg, 0);
        this.updatePlayerProperty(PhotonPlayerProperty.total_dmg, 0);
        this.updatePlayerProperty(PhotonPlayerProperty.name, LoginFengKAI.player.name);
        this.updatePlayerProperty(PhotonPlayerProperty.guildName, LoginFengKAI.player.guildname);
    }

    private void updatePlayerProperty(string ppp, object defaultValue) {
        if (PhotonNetwork.player.customProperties.ContainsKey(ppp)) {
            this.playerProperties[ppp] = PhotonNetwork.player.customProperties[ppp];
        } else {
            this.playerProperties[ppp] = defaultValue;
        }
    }

    public void resetStatsOnRoomJoin() {
        ExitGames.Client.Photon.Hashtable ht = this.getHashtable();
        ht[PhotonPlayerProperty.kills] = 0;
        ht[PhotonPlayerProperty.deaths] = 0;
        ht[PhotonPlayerProperty.max_dmg] = 0;
        ht[PhotonPlayerProperty.total_dmg] = 0;
        ht[PhotonPlayerProperty.name] = LoginFengKAI.player.name;
        ht[PhotonPlayerProperty.guildName] = LoginFengKAI.player.guildname;
    }

    private ExitGames.Client.Photon.Hashtable getHashtable() {
        if (this.playerProperties == null) {
            this.updateInternalPlayerProperties();

            if (this.playerProperties == null) {
                this.playerProperties = new ExitGames.Client.Photon.Hashtable();
                this.playerProperties[PhotonPlayerProperty.kills] = 0;
                this.playerProperties[PhotonPlayerProperty.deaths] = 0;
                this.playerProperties[PhotonPlayerProperty.max_dmg] = 0;
                this.playerProperties[PhotonPlayerProperty.total_dmg] = 0;
                this.playerProperties[PhotonPlayerProperty.name] = LoginFengKAI.player.name;
                this.playerProperties[PhotonPlayerProperty.guildName] = LoginFengKAI.player.guildname;
            } else {
                if (!this.playerProperties.ContainsKey(PhotonPlayerProperty.kills)) {
                    this.playerProperties[PhotonPlayerProperty.kills] = 0;
                }
                if (!this.playerProperties.ContainsKey(PhotonPlayerProperty.deaths)) {
                    this.playerProperties[PhotonPlayerProperty.deaths] = 0;
                }
                if (!this.playerProperties.ContainsKey(PhotonPlayerProperty.max_dmg)) {
                    this.playerProperties[PhotonPlayerProperty.max_dmg] = 0;
                }
                if (!this.playerProperties.ContainsKey(PhotonPlayerProperty.total_dmg)) {
                    this.playerProperties[PhotonPlayerProperty.total_dmg] = 0;
                }
                if (!this.playerProperties.ContainsKey(PhotonPlayerProperty.name)) {
                    this.playerProperties[PhotonPlayerProperty.name] = ModMain.instance.getConfig().get("displayName");
                }
                if (!this.playerProperties.ContainsKey(PhotonPlayerProperty.guildName)) {
                    this.playerProperties[PhotonPlayerProperty.guildName] = ModMain.instance.getConfig().get("guild");
                }
            }
        }
        return this.playerProperties;
    }

    public void addKillTitanInfo(int damage) {
        ExitGames.Client.Photon.Hashtable ht = this.getHashtable();
        ht[PhotonPlayerProperty.kills] = ((int)ht[PhotonPlayerProperty.kills]) + 1;
        ht[PhotonPlayerProperty.total_dmg] = ((int)ht[PhotonPlayerProperty.total_dmg]) + damage;

        if (damage > ((int)ht[PhotonPlayerProperty.max_dmg])) {
            ht[PhotonPlayerProperty.max_dmg] = damage;
        }
    }

    public void playerDied() {
        this.getHashtable()[PhotonPlayerProperty.deaths] = ((int)this.getHashtable()[PhotonPlayerProperty.deaths]) + 1;
    }

    private void updateScoreboardHashtable() {
        ModMain.instance.getTaskManager().addLateUpdateTask(new SetScoreboardTask(this.getHashtable()));
    }

    private bool didSomeoneTamperTheScoreboard() {
        ExitGames.Client.Photon.Hashtable ht = this.getHashtable();
        foreach (string s in ht.Keys) {
            if (ht[s] != PhotonNetwork.player.customProperties[s]) {
                return true;
            }
        }

        return false;
    }

    public BanType getBanTypeByInt(int type) {
        switch (type) {
            case 0:
                return BanType.FULL_NAME;
            case 1:
                return BanType.CONTAINS_PART;
            case 2:
                return BanType.GUILD_FULL;
            case 3:
                return BanType.GUILD_PART;
            default:
                return BanType.FULL_NAME;
        }
    }
	public enum BanType{
		FULL_NAME = 0,
		CONTAINS_PART = 1,
		GUILD_FULL = 2,
		GUILD_PART = 3
	}
    private class SetScoreboardTask : Task {
        private readonly ExitGames.Client.Photon.Hashtable table;

        public SetScoreboardTask(ExitGames.Client.Photon.Hashtable table) {
            this.table = table;
        }

        public bool execute() {
            PhotonNetwork.player.SetCustomProperties(table);
            return true;
        }
    }
    private class DeathCheckTask : Task {
        private readonly List<int> ids;

        public DeathCheckTask(List<int> ids) {
            this.ids = new List<int>(ids);
        }
        public bool execute() {
            foreach (HERO h in GameObject.FindObjectsOfType<HERO>()) {
                foreach (int i in this.ids) {
                    if (h.photonView != null && h.photonView.ownerId == i) {
                        h.photonView.RPC("netDie2", PhotonTargets.All, int.MinValue, string.Empty);
                        break;
                    }
                }
            }

            foreach (TITAN titan in GameObject.FindObjectsOfType<TITAN>()) {
                foreach (int i in this.ids) {
                    if (titan.photonView != null && titan.photonView.ownerId == i) {
                        titan.photonView.RPC("netDie", PhotonTargets.All, null);
                        break;
                    }
                }
            }

            foreach (COLOSSAL_TITAN titan in GameObject.FindObjectsOfType<COLOSSAL_TITAN>()) {
                foreach (int i in this.ids) {
                    if (titan.photonView != null && titan.photonView.ownerId == i) {
                        titan.photonView.RPC("netDie", PhotonTargets.All, null);
                        break;
                    }
                }
            }

            foreach (FEMALE_TITAN titan in GameObject.FindObjectsOfType<FEMALE_TITAN>()) {
                foreach (int i in this.ids) {
                    if (titan.photonView != null && titan.photonView.ownerId == i) {
                        titan.photonView.RPC("netDie", PhotonTargets.All, null);
                        break;
                    }
                }
            }

            return true;
        }
    }
	private class RespawnTask : Task{
		
		public bool execute (){
			ModMain.instance.getGameManager().photonView.RPC("respawnHeroInNewRound", PhotonTargets.All, new object[0]);
			return true;
		}
	}
}

