using UnityEngine;
using System.Collections;

//whois <id>
public class WhoisCmd : ICommand{
	public bool cmd(string[] args, FengGameManagerMKII gm){
		if (args.Length == 0){
			ModMain.instance.sendToPlayer("/whois <id>");
			return true;
		}

		PhotonPlayer target = null;
		foreach(PhotonPlayer p in PhotonNetwork.playerList){
			if ("" + p.ID == args[0]){
				target = p;
				break;
			}
		}

		if (target != null){
			string[] messages = new string[5];

			messages[0] = "Player: \"" +  target.customProperties[PhotonPlayerProperty.name] + "\"";
			messages[1] = "Character: " + target.customProperties[PhotonPlayerProperty.character];
			messages[2] = "(GAS) (BLA) (SPEED) (ACL)";
			messages[3] = " " + target.customProperties[PhotonPlayerProperty.statGAS] + "     "
					+ target.customProperties[PhotonPlayerProperty.statBLA] + "     "
					+ target.customProperties[PhotonPlayerProperty.statSPD] + "      " 
					+ target.customProperties[PhotonPlayerProperty.statACL];
			messages[4] = "Skill: " + target.customProperties[PhotonPlayerProperty.statSKILL];

			ModMain.instance.sendToPlayer(messages);
			return true;
		}else{
			ModMain.instance.sendToPlayer("That player is not online.");
			return true;
		}
	}

    public string getDescriptionString() {
        return "Shows information about a player.";
    }
}

