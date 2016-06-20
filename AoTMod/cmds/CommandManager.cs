using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Text;
using AoTMod.cmds;

public class CommandManager{
	private List<string> cmdHistory = new List<string>();
	private IDictionary<string, ICommand> commandMap = new Dictionary<string, ICommand>();

	public CommandManager(){
		this.registerCommands();
	}
	//cmd = /test arg0 arg1
	public bool parseCommand(string cmd, FengGameManagerMKII gm){
		string[] parts;

		try{
			parts = this.parseInput(cmd);
		}catch(System.Exception e){
			ModMain.instance.sendToPlayer("An error occured while parsing arguments.");
			ModMain.instance.log(e);
			return false;
		}

		string[] args = new string[parts.Length - 1];
		string command = parts[0].Substring(1, parts[0].Length - 1);
		Array.Copy(parts, 1, args, 0, args.Length);
		ICommand icmd;

		this.commandMap.TryGetValue(command, out icmd);

        this.cmdHistory.Add(cmd);

        if (this.cmdHistory.Count > 16) {
            this.cmdHistory.RemoveAt(0);//Kind of resources reordering the list, but whatever
        }

#if DEBUG
		if (command.Equals("configKeys")){
			string message = "";
			foreach (string s in ModMain.instance.getConfig().getKeyList()){
				message += s + "=" + ModMain.instance.getConfig().get(s) + "\n";
			}
			ModMain.instance.sendToPlayer(new string[]{message});
			return true;
		}

		if (command.Equals("assets")){
			this.displayLoadedAssets();
			return true;
		}

		if (command.Equals("monos")){
			this.displayMonos();
			return true;
		}

		if (command.Equals("prefabs")){
			string s = args.Length > 0 ? args[0] : "";
			foreach(object o in Resources.LoadAll(s)){
				ModMain.instance.log(s + " Object: "+ o);
			}
			return true;
		}

		//sp <prefab> <playerID>
		if (command.Equals("sp")){
			Vector3 pos = Vector3.zero;
			foreach(HERO h in GameObject.FindObjectsOfType<HERO>()){
				if ("" + h.photonView.ownerId == args[1]){
					pos = h.transform.position;
					break;
				}
			}

			PhotonNetwork.Instantiate(args[0], pos, Quaternion.Euler(new Vector3(0f, 1f, 0f)), 0);
			return true;
		}

		/*
		if (command.Equals("/flareFields")){
			HERO h = GameObject.FindObjectOfType<HERO>();
			Vector3 pos = h.transform.position;
			GameObject goFlare = (GameObject) Resources.Load("FX/flareBullet1");

			GameObject flare = PhotonNetwork.Instantiate("FX/flareBullet1", pos, Quaternion.Euler(new Vector3(0f, 1f, 0f)), 0);
			ParticleSystem ps = (ParticleSystem) flare.GetComponent(typeof(ParticleSystem));

			if (args.Length < 3){
				ps.startColor = new Color(0.43f, 0.070f, 0.870f);
			}else{
				ps.startColor = new Color(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]));
			}
			foreach(string s in PhotonNetwork.PhotonServerSettings.RpcList){
				ModMain.instance.log("\tRPC: " + s);
			}
		}
		*/
#endif

		if (icmd == null){
			ModMain.instance.sendToPlayer("Command not found: \'" + command + '\'');
			this.displayCommands();
			return true;
		}else{
			try{
				if (!icmd.cmd(args, gm)){
					ModMain.instance.sendToPlayer("An error occured while performing the command \"" + cmd + "\". Make sure you got the arguments right.");
				}
				return true;
			}catch(System.Exception e){
                ModMain.instance.sendToPlayer("An error occured while performing the command \"" + cmd + "\". Make sure you got the arguments right.");
				ModMain.instance.log("Exception during command: " + cmd);
				ModMain.instance.log(e);
				return true;
			}

		}
	}

	private void displayCommands(){
		ModMain.instance.sendToPlayer("/help <1-5>");
	}

#if DEBUG
	private void displayLoadedAssets(){
		GameObject[] objects = GameObject.FindObjectsOfType<GameObject>() as GameObject[];

		foreach (GameObject obj in objects){
			ModMain.instance.log("GameObject: " + obj.tag + " | " + obj);
		}
	}

	private void displayMonos(){
		MonoBehaviour[] objects = GameObject.FindObjectsOfType<MonoBehaviour>() as MonoBehaviour[];
		
		foreach (MonoBehaviour obj in objects){
			ModMain.instance.log("MonoBehaviour: " + obj);
		}
	}
#endif

	private void registerCommands(){
		this.commandMap.Add("ban", new BanCmd());
		this.commandMap.Add("bind", new BindCmd());
		this.commandMap.Add("cMaster", new ChangeMasterCmd());
		this.commandMap.Add("cl", new ChatLogCmd());
		this.commandMap.Add("dl", new DeathListCmd());
		this.commandMap.Add("destroy", new DestroyCmd());
		this.commandMap.Add("endGame", new EndGameCmd());
		this.commandMap.Add("eren", new ErenCmd());
		this.commandMap.Add("gravity", new GravityCmd());
		this.commandMap.Add("greeter", new GreeterCmd());
		this.commandMap.Add("help", new HelpCmd());
		this.commandMap.Add("info", new InfoCmd());
		this.commandMap.Add("kick", new KickCmd());
		this.commandMap.Add("kill", new KillCmd());
		this.commandMap.Add("me", new MeCmd());
		this.commandMap.Add("msg", new MsgCmd());
		this.commandMap.Add("persona", new PersonaCmd());
		this.commandMap.Add("reset", new ResetCmd());
		this.commandMap.Add("respawn", new RespawnCmd());
		this.commandMap.Add("restart", new RestartCmd());
		this.commandMap.Add("set", new SetCmd());
		this.commandMap.Add("spawn", new SpawnCmd());
		this.commandMap.Add("tc", new TitanControlCmd());
		this.commandMap.Add("th", new TitanHealthCmd());
		this.commandMap.Add("transform", new TransformCmd());
		this.commandMap.Add("whois", new WhoisCmd());
		this.commandMap.Add("ch", new ChampionshipCmd());
		this.commandMap.Add("sc", new SpawnControlCmd());
		this.commandMap.Add("room", new RoomCmd());
		this.commandMap.Add("destroyList", new DestroyListCmd());
		this.commandMap.Add("crash", new CrashCmd());
		this.commandMap.Add("steal", new StealCmd());
		this.commandMap.Add("autoRespawn", new AutoRespawnCmd());
		this.commandMap.Add("nameChanger", new NameChangerCmd());
		this.commandMap.Add("raw", new RawCmd());
		this.commandMap.Add("addForce", new AddForceCmd());
		this.commandMap.Add("lag", new LagCmd());
#if DEBUG
		this.commandMap.Add("debug", new DebugCmd());
#endif
		this.commandMap.Add("horse", new HorseCmd());
		this.commandMap.Add("godMode", new GodModeCmd());
        this.commandMap.Add("find", new FindCmd());
        this.commandMap.Add("autoCrash", new AutoCrashCmd());
        this.commandMap.Add("forceAnim", new ForceAnimCmd());
        this.commandMap.Add("dc", new DcCommand());
        this.commandMap.Add("clearChat", new ClearChatCmd());
        this.commandMap.Add("test", new TestCmd());
        this.commandMap.Add("fake", new FakeCmd());
        this.commandMap.Add("detect", new DetectCmd());
        this.commandMap.Add("batch", new BatchCmd());
        this.commandMap.Add("hidden", new InvisibleCmd());
        this.commandMap.Add("fakeMod", new FakeModCmd());
    }

	private string[] parseInput(string src){
		List<string> resList = new List<string>();
		StringBuilder builder = new StringBuilder(32);

		src = src.Trim();

		for(int i = 0; i < src.Length; i++){
			switch(src[i]){
				case '\"':
                    i++;

                    if (builder.Length > 0) {
                        resList.Add(builder.ToString());
                        builder.Length = 0;
                    }

                    while (i < src.Length) {
                        if (src[i].Equals('\\')) {
                            i++;
                            builder.Append(this.getEscapedCharacter(src[i]));
                        } else  if (src[i].Equals('\"')){
                            resList.Add(builder.ToString());
                            builder.Length = 0;
                            break;
                        }else{
                            builder.Append(src[i]);
                        }

                        i++;
                    }

                    i++;
                    break;
				case '\\':
                    i++;
                    builder.Append(this.getEscapedCharacter(src[i]));
					break;
				case ' ':
                    if (builder.Length > 0) {
                        resList.Add(builder.ToString());
                        builder.Length = 0;
                    }
					break;
				default:
                    builder.Append(src[i]);
					break;
			}
		}

        if (builder.Length > 0) {
            resList.Add(builder.ToString());
        }

		return resList.ToArray();
	}

    private string getEscapedCharacter(char c) {
        switch (c) {
            case 'n':
                return "\n";
            case 't':
                return "\t";
            case '\\':
                return "\\";
            case '\"':
                return "\"";
            default:
                return "\\" + c;
        }
    }


	public ICommand getCommand(string name){
		ICommand res;
		this.commandMap.TryGetValue(name, out res);
		return res;
	}

    public void buildHelpList() {
        ((HelpCmd)this.getCommand("help")).buildHelpCommands(this.commandMap.Keys);
    }

    public string getCommandInHistory(int index) {
        if (index <= this.cmdHistory.Count && index > 0) {
            return this.cmdHistory[this.cmdHistory.Count - index];
        } else {
            return string.Empty;
        }
    }

}

