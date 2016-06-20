using UnityEngine;
using System.Reflection;
using System.Collections;
using System;
public class SetCmd : ICommand{

	private float originalSpeed = -1f;
	//set <blades|gas> <on|off>
	public bool cmd(string[] args, FengGameManagerMKII gm){
		if (args.Length < 2){
			ModMain.instance.sendToPlayer(new string[]{
				"/set damage <newAverage> - sets the target average damage",
				"/set <blades|gas|damage> <on|off> - Enables/Disables gas/blade usage",
				"/set <kills|deaths|max_dmg|total_dmg|name|guild|chatName> <value>",
				"/set time <amount> - Set the time elapsed in the game",
				"/set time -1 - Ends the game",
				"/set messageCount <amount> - Sets the number of messages to be displayed in chat",
				"/set killTitan <new name> - Set the titan name of the kill command",
				"/set stat [-g] <spd,gas,acl,bla> <value>",
				"/set player <id> <name|guild|kills|deaths|max_damage|total_damage|isTitan|dead|acl|spd|bla|gas|hair_color|character|skill> <value>",
				"The name can contain spaces",
                "/set <prefix|suffix> <value> | The chat prefix"
			});
			return true;
		}

		if (args[0].Equals("blades", System.StringComparison.OrdinalIgnoreCase)){
			this.setBlades(args);
		}else if (args[0].Equals("gas", System.StringComparison.OrdinalIgnoreCase)){
			this.setGas(args);
		}else if(args[0].Equals("damage", System.StringComparison.OrdinalIgnoreCase)){
			ModMain.instance.setAverageDamage(float.Parse(args[1]));
		}else if(args[0].Equals("time", System.StringComparison.OrdinalIgnoreCase)){
			this.doTime(args, gm);
		}else if(args[0].Equals("speed", System.StringComparison.OrdinalIgnoreCase)){
			this.doSpeed(args);
		}else if(args[0].Equals("killTitan", System.StringComparison.OrdinalIgnoreCase)){
			this.doKillTitan(args);
		}else if(args[0].Equals("stat", System.StringComparison.OrdinalIgnoreCase)){
			this.setStats(args);
			return true;
		}else if(args[0].Equals("messageCount", System.StringComparison.OrdinalIgnoreCase)){
			this.doMessageCount(args);
		}else if(args[0].Equals("player", System.StringComparison.OrdinalIgnoreCase)){
			this.setOtherPlayer(args);
        } else if (args[0].Equals("prefix", System.StringComparison.OrdinalIgnoreCase)) {
            string[] temp = new string[args.Length - 1];
            Array.Copy(args, 1, temp, 0, temp.Length);
            ModMain.instance.setChatPrefix(string.Join(" ", temp));
        } else if (args[0].Equals("suffix", System.StringComparison.OrdinalIgnoreCase)) {
            string[] temp = new string[args.Length - 1];
            Array.Copy(args, 1, temp, 0, temp.Length);
            ModMain.instance.setChatSuffix(string.Join(" ", temp));
        } else {
            doOtherSet(args);
        }
		return true;
	}
	//set messageCount <amount>
	private void doMessageCount(string[] args){
		ModMain.instance.setMessageCount(int.Parse(args[1]));
	}

	private void setBlades(string[] args){
		ModMain.instance.setUseBlades(args[1].Equals("on", System.StringComparison.OrdinalIgnoreCase));
	}

	private void setGas(string[] args){
		ModMain.instance.setUseGas(args[1].Equals("on", System.StringComparison.OrdinalIgnoreCase));
	}

	private void doOtherSet(string[] args){
		//set <kills|deaths|max_dmg|total_dmg|name|guild> <value>
		ExitGames.Client.Photon.Hashtable toSet = new ExitGames.Client.Photon.Hashtable();

		if (args.Length > 1){
			if (args[0].Equals("kills", System.StringComparison.OrdinalIgnoreCase)){
				int val = int.Parse(args[1]);
				toSet.Add(PhotonPlayerProperty.kills, val);
			}else if (args[0].Equals("deaths", System.StringComparison.OrdinalIgnoreCase)){
				int val = int.Parse(args[1]);
				toSet.Add(PhotonPlayerProperty.deaths, val);
			}else if (args[0].Equals("max_dmg", System.StringComparison.OrdinalIgnoreCase)){
				int val = int.Parse(args[1]);
				toSet.Add(PhotonPlayerProperty.max_dmg, val);
			}else if (args[0].Equals("total_dmg", System.StringComparison.OrdinalIgnoreCase)){
				int val = int.Parse(args[1]);
				toSet.Add(PhotonPlayerProperty.total_dmg, val);
			}else if (args[0].Equals("name", System.StringComparison.OrdinalIgnoreCase)){
				//set args[0] args[1] ... args[n]
				//set name [-k] <value>
				bool keep = args[1].Equals("-k");
				string[] a = new string[args.Length - ((keep) ? 2 : 1)];

				if (args[((keep) ? 2 : 1)].Equals("%null%")){
					if (keep){LoginFengKAI.player.name = null;

					}else{
						LoginFengKAI.player.name = null;
					}
				}else if (a.Length > 0){
					Array.Copy(args, ((keep) ? 2 : 1), a, 0, a.Length);
					string name = string.Join(" ", a).Replace("\\n", "\n");
					toSet.Add(PhotonPlayerProperty.name, name);

					if (keep){
						LoginFengKAI.player.name = name;
					}
				}
			}else if (args[0].Equals("guild", System.StringComparison.OrdinalIgnoreCase)){
				//set args[0] args[1] ... args[n]
				//set guild -[k] <value>
				bool keep = args[1].Equals("-k");
				string[] a = new string[args.Length - ((keep) ? 2 : 1)];

				if (args[((keep) ? 2 : 1)].Equals("%null%")){
					if (keep){LoginFengKAI.player.guildname = null;
					}else{
						LoginFengKAI.player.guildname = null;
					}
				}else if (a.Length > 0){
					Array.Copy(args, ((keep) ? 2 : 1), a, 0, a.Length);
					string guild = string.Join(" ", a).Replace("\\n", "\n");
					toSet.Add(PhotonPlayerProperty.guildName, guild);

					if (keep){
						LoginFengKAI.player.guildname = guild;
					}
				}
			}else if (args[0].Equals("chatName", System.StringComparison.OrdinalIgnoreCase)){
				//set args[0] args[1] ... args[n]
				//set chatName <value>
				string[] a = new string[args.Length -1];
				if (a.Length > 0){
					Array.Copy(args, 1, a, 0, a.Length);
					string chatName = string.Join(" ", a).Replace("\\n", "\n");
					ModMain.instance.getNameManager().setName(chatName);
				}
			}else{
				ModMain.instance.sendToPlayer("/set <kills|deaths|max_dmg|total_dmg|name> <value>");
				return;
			}

			PhotonNetwork.player.SetCustomProperties(toSet);
            ModMain.instance.getModMainThread().updateInternalPlayerProperties();
		}
	}

	private void doTime(string[] args, FengGameManagerMKII gm){
		//set time <time>
		//set time -1
		FieldInfo serverTimeField = gm.GetType().GetField("timeTotalServer", BindingFlags.NonPublic | BindingFlags.Instance);
		float time = float.Parse(args[1]);

		if (time >= 0){
			serverTimeField.SetValue(gm, time);
		}else{
			FieldInfo timeField = gm.GetType().GetField("time", BindingFlags.NonPublic | BindingFlags.Instance);
			int val = (int)timeField.GetValue(gm);
			serverTimeField.SetValue(gm, (float) val);
		}
	}

	private void doSpeed(string[] args){
		foreach (HERO h in UnityEngine.Object.FindObjectsOfType<HERO>()){
			if (h.photonView.ownerId == PhotonNetwork.player.ID){
				if (this.originalSpeed < 0f){
					this.originalSpeed = h.speed;
				}

				h.speed = float.Parse(args[1]);
				break;
			}
		}
	}
	//set killTitan arg1 ... argn
	private void doKillTitan(string[] args){
		string[] nameParts = new string[args.Length - 1];

		Array.Copy(args, 1, nameParts, 0, nameParts.Length);
		KillCmd.titanName = string.Join(" ", nameParts);
	}
	//set stat [g] <stat> <value>
	private void setStats(string[] args){
		bool isGlobal = args[1].Equals("-g");
		int startIndex = isGlobal ? 2 : 1;
		ExitGames.Client.Photon.Hashtable toSet = new ExitGames.Client.Photon.Hashtable();
		HERO h = this.getPlayerHero();
		string slot = IN_GAME_MAIN_CAMERA.singleCharacter;
		if (h == null){
			ModMain.instance.sendToPlayer("Please wait untill you have spawned to do this command.");
			return;
		}
		ModMain.instance.log("Current costume ID: " + h.setup.myCostume.id);
		int costumeID = h.setup.myCostume.id;
		if (args[startIndex].Equals("acl", StringComparison.OrdinalIgnoreCase)){
			h.setup.myCostume.stat.ACL = int.Parse(args[startIndex + 1]);
			toSet[PhotonPlayerProperty.statACL] = int.Parse(args[startIndex + 1]);
			HeroCostume.costume[costumeID].stat.ACL = int.Parse(args[startIndex + 1]);
			if (isGlobal){
				PlayerPrefs.SetInt("" + slot + PhotonPlayerProperty.statACL, int.Parse(args[startIndex + 1]));
			}
		}else if (args[startIndex].Equals("bla", StringComparison.OrdinalIgnoreCase)){
			h.setup.myCostume.stat.BLA = int.Parse(args[startIndex + 1]);
			toSet[PhotonPlayerProperty.statBLA] = int.Parse(args[startIndex + 1]);
			HeroCostume.costume[costumeID].stat.BLA = int.Parse(args[startIndex + 1]);

			if (isGlobal){
				PlayerPrefs.SetInt("" + slot + PhotonPlayerProperty.statBLA, int.Parse(args[startIndex + 1]));
			}
		}else if (args[startIndex].Equals("gas", StringComparison.OrdinalIgnoreCase)){
			h.setup.myCostume.stat.GAS = int.Parse(args[startIndex + 1]);
			toSet[PhotonPlayerProperty.statGAS] = int.Parse(args[startIndex + 1]);
			HeroCostume.costume[costumeID].stat.GAS = int.Parse(args[startIndex + 1]);

			if (isGlobal){
				PlayerPrefs.SetInt("" + slot + PhotonPlayerProperty.statGAS, int.Parse(args[startIndex + 1]));
			}
		}else if (args[startIndex].Equals("spd", StringComparison.OrdinalIgnoreCase)){
			h.setup.myCostume.stat.SPD = int.Parse(args[startIndex + 1]);	
			toSet[PhotonPlayerProperty.statSPD] = int.Parse(args[startIndex + 1]);
			HeroCostume.costume[costumeID].stat.SPD = int.Parse(args[startIndex + 1]);

			if (isGlobal){
				PlayerPrefs.SetInt("" + slot + PhotonPlayerProperty.statSPD, int.Parse(args[startIndex + 1]));
			}
		}

		h.setup.setCharacterComponent();

		h.setStat();
		h.setSkillHUDPosition();

		if (isGlobal){
			PhotonNetwork.player.SetCustomProperties(toSet);
		}
	}

	private void setOtherPlayer(string[] args){
		//set player <id> <name|guild|kills|deaths|max_damage|total_damage|isTitan|dead|acl|spd|bla|gas|hair_color|character|skill> <value>
		ModMain.instance.log("Finding id: " + args[1]);
		PhotonPlayer target = null;
		foreach(PhotonPlayer player in PhotonNetwork.playerList){
			if ("" + player.ID == args[1]){
				target = player;
				break;
			}
		}
		ExitGames.Client.Photon.Hashtable toSet = target.customProperties;

		if (args[2].Equals("name", StringComparison.OrdinalIgnoreCase)){
			toSet[PhotonPlayerProperty.name] = string.Join(" ", args, 3, args.Length - 3);
		}else if (args[2].Equals("guild", StringComparison.OrdinalIgnoreCase)){
			toSet[PhotonPlayerProperty.guildName] = string.Join(" ", args, 3, args.Length - 3);
		}else if (args[2].Equals("deaths", StringComparison.OrdinalIgnoreCase)){
			toSet[PhotonPlayerProperty.deaths] = int.Parse(args[3]);
		}else if (args[2].Equals("max_damage", StringComparison.OrdinalIgnoreCase)){
			toSet[PhotonPlayerProperty.max_dmg] = int.Parse(args[3]);
		}else if (args[2].Equals("total_damage", StringComparison.OrdinalIgnoreCase)){
			toSet[PhotonPlayerProperty.total_dmg] = int.Parse(args[3]);
		}else if (args[2].Equals("isTitan", StringComparison.OrdinalIgnoreCase)){
			toSet[PhotonPlayerProperty.isTitan] = bool.Parse(args[3]);
		}else if (args[2].Equals("dead", StringComparison.OrdinalIgnoreCase)){
			toSet[PhotonPlayerProperty.dead] = bool.Parse(args[3]);
		}else if (args[2].Equals("kills", StringComparison.OrdinalIgnoreCase)){
			toSet[PhotonPlayerProperty.kills] = int.Parse(args[3]);
		}else if (args[2].Equals("acl", StringComparison.OrdinalIgnoreCase)){
			toSet[PhotonPlayerProperty.statACL] = int.Parse(args[3]);
		}else if (args[2].Equals("spd", StringComparison.OrdinalIgnoreCase)){
			toSet[PhotonPlayerProperty.statSPD] = int.Parse(args[3]);
		}else if (args[2].Equals("gas", StringComparison.OrdinalIgnoreCase)){
			toSet[PhotonPlayerProperty.statGAS] = int.Parse(args[3]);
		}else if (args[2].Equals("bla", StringComparison.OrdinalIgnoreCase)){
			toSet[PhotonPlayerProperty.statBLA] = int.Parse(args[3]);
		}else if (args[2].Equals("hair_color", StringComparison.OrdinalIgnoreCase)){
			toSet[PhotonPlayerProperty.hair_color1] = float.Parse(args[3]);
			toSet[PhotonPlayerProperty.hair_color2] = float.Parse(args[4]);
			toSet[PhotonPlayerProperty.hair_color3] = float.Parse(args[5]);
		}else if (args[2].Equals("character", StringComparison.OrdinalIgnoreCase)){
			toSet[PhotonPlayerProperty.character] = args[3];
		}else if (args[2].Equals("skill", StringComparison.OrdinalIgnoreCase)){
			toSet[PhotonPlayerProperty.statSKILL] = args[3];
		}else if (args[2].Equals("-u", StringComparison.OrdinalIgnoreCase)){
			if (args.Length == 3){
				ModMain.instance.sendToPlayer("" + toSet);
			}else{
				this.doUnsafeSet(toSet, args);
			}
		}else {
			ModMain.instance.sendToPlayer("Unknown parameter: " + args[2]);
			return;
		}
		target.SetCustomProperties(toSet);
	}

	private HERO getPlayerHero(){
		foreach(HERO h in GameObject.FindObjectsOfType<HERO>()){
			if (h.photonView.ownerId == PhotonNetwork.player.ID){
				return h;
			}
		}
		return null;
	}

	private void doUnsafeSet(ExitGames.Client.Photon.Hashtable toSet, string[] args){
		//       0     1   2     3      4       5
		//set player <id> -u -<Type> <Param> <Value>
		//Types:
		//-F = float
		//-I = Integer
		//-S = String
		//-B = Boolean
		switch(args[3][1]){
			case 'F':
				toSet[args[4]] = float.Parse(args[5]);
				break;
			case 'I':
				toSet[args[4]] = int.Parse(args[5]);
				break;
			case 'S':
				toSet[args[4]] = string.Join(" ", args, 5, args.Length - 5);
				break;
			case 'B':
				toSet[args[4]] = bool.Parse(args[5]);
				break;
			default:
				ModMain.instance.log("Set::Unsafe - Unknown type:" + args[3][1]);
				return;
		}
	}

    public string getDescriptionString() {
        return "Set values for you/other players.(Name,guild,etc.)";
    }
}

