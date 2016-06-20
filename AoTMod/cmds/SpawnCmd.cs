using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Threading;

public class SpawnCmd : ICommand{
	//spawn <amount> <id>
	//spawn -a <amount> <id>
	//spawn -c <amount> <id>
	//spawn -s <scale> <amount> <id>
	//spawn -t <type> <amount> <id>
	//spawn -p <x> <y> <z>
	private string[] helpMessages = new string[]{
		"spawn <amount> <id>",
		"spawn -a <amount> <id>",
		"spawn -c <amount> <id>",
		"spawn -s <scale> <amount> <id>",
		"spawn -t <type> <amount> <id>",
		"spawn -p <.x> <.y> <.z>"
	};

	public bool cmd(string[] args, FengGameManagerMKII gm){
		if (args.Length < 2){
			ModMain.instance.sendToPlayer(helpMessages);
			return true;
		}

		if (args.Length == 2){
			doNormalSpawns(args, gm);
		}else if (args[0].Equals("-a")){
			doAnnieSpawn(args, gm);
		}else if (args[0].Equals("-c")){
			doCTSpawn(args, gm);
		}else{
			Vector3 pos = Vector3.zero;
			Vector3 scale = new Vector3(3f, 3f, 3f);//large titan

			AbnormalType type = AbnormalType.NORMAL;

			int index = 0;
			int amount = 0;

			foreach(string s in args){
				if (s.StartsWith("-")){
					index += 1;
					switch(s[1]){
						case 's':
							float f = float.Parse(args[index]);
							scale = new Vector3(f, f, f);
							index += 1;
							break;

						case 't':
							type = TitanSize.getTitanType(args[index][0]);
							index += 1;
							break;

						case 'p':
							float x = float.Parse(args[index]);
							float y = float.Parse(args[index + 1]);
							float z = float.Parse(args[index + 2]);
							pos = new Vector3(x, y, z);
							index += 3;
							break;
					}
				}
			}

			amount = int.Parse(args[index++]);

			if (pos.Equals(Vector3.zero)){
				pos = this.getPlayerPos(args[index]);
			}

			doCustomSpawns(amount, type, scale, pos);
		}

		return true;

	}

	private void doNormalSpawns(string[] args, FengGameManagerMKII gm){
		//spawn <amount> <player>
		int max = int.Parse(args[0]);
		Vector3 pos = Vector3.zero;
		HERO[] players = Object.FindObjectsOfType<HERO>() as HERO[];

		foreach(HERO h in players){
			if ("" + h.photonView.owner.ID == args[1]){
				pos = h.transform.position;
				break;
			}
		}
		
		for(int i = 0; i < max; i++){
			//GameObject go = PhotonNetwork.Instantiate("TITAN_NEW_1", pos, Quaternion.Euler(-90f, 0f, 0f), 0);
			GameObject go = gm.spawnTitan((int)Random.Range(0f, 100f), pos, Quaternion.Euler(-90f, 0f, 0f), false);
			//new Thread(() => SpawnCmd.changeTitanScale(go)).Start();
		}
	}

	private void doCustomSpawns(int amount, AbnormalType type, Vector3 scale, Vector3 pos){
		for(int i = 0; i < amount; i++){
			//Old titans = TITAN_NEW_1 or TITAN_NEW_2
			GameObject titan = PhotonNetwork.Instantiate("TITAN_VER3.1", pos, Quaternion.Euler(-90f, 0f, 0f), 0);
			new Thread(() => this.changeTitanAttributes(titan, type, scale)).Start();
		}
	}

	//spawn -ca <amount> <id>
	private void doCTSpawn(string[] args, FengGameManagerMKII gm){
		if (args.Length >= 3){
			Vector3 pos = getPlayerPos(args[2]);
			int amount = int.Parse(args[1]);

			for(int i = 0; i < amount; i++){
				GameObject ct = PhotonNetwork.Instantiate("COLOSSAL_TITAN", pos, Quaternion.Euler(0f,0f,0f), 0);
				new Thread(() => this.changeCTPosition(ct, pos)).Start();
			}
		}

	}

	private void doAnnieSpawn(string[] args, FengGameManagerMKII gm){
		if (args.Length >= 3){
			Vector3 pos = getPlayerPos(args[2]);
			int amount = int.Parse(args[1]);

			for(int i = 0; i < amount; i++){
				PhotonNetwork.Instantiate("FEMALE_TITAN", pos, Quaternion.Euler(0f,0f,0f), 0);
			}
		}
	}

	private Vector3 getPlayerPos(string id){
		Vector3 pos = Vector3.zero;
		HERO[] players = Object.FindObjectsOfType<HERO>() as HERO[];
		
		foreach(HERO h in players){
			if ("" + h.photonView.owner.ID == id){
				pos = h.transform.position;
				break;
			}
		}
		return pos;
	}

	public void changeCTPosition(GameObject ct, Vector3 pos){
		COLOSSAL_TITAN s = ct.GetComponent<COLOSSAL_TITAN>();
		FieldInfo f = s.GetType().GetField("waitTime", BindingFlags.Instance | BindingFlags.NonPublic);

		int val = (int) ((float)f.GetValue(s) * 1000) + 500;
		Thread.Sleep(val);
		ct.transform.position = pos;
	}

	public void changeTitanAttributes(GameObject titan, AbnormalType type, Vector3 scale){
		Thread.Sleep(100);

		TITAN t = titan.GetComponent<TITAN>();

		lock(t){
			t.nonAI = false;
			t.setAbnormalType(type, type == AbnormalType.TYPE_CRAWLER);
			object[] nslParams = new object[]{scale.x, 0, 0};
			t.photonView.RPC("netSetLevel", PhotonTargets.AllBuffered, nslParams);
		}
		//titan.transform.localScale = scale;

	}

    public string getDescriptionString() {
        return "Spawns titans.";
    }
}

