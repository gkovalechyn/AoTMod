using UnityEngine;
using System;
using System.Collections;
//msg <id> <message>
public class MsgCmd : ICommand{
	public bool cmd(string[] args, FengGameManagerMKII gm){
		if (args.Length < 2){
			ModMain.instance.sendToPlayer(new string[]{"/msg <id> <message>"});
			return true;
		}
		PhotonPlayer player = null;

		foreach(PhotonPlayer p in PhotonNetwork.playerList){
			if ("" + p.ID == args[0]){
				player = p;
				break;
			}
		}

		if (player != null){
			string[] words = new string[args.Length - 1];
			Array.Copy(args, 1, words,0, words.Length);
			string message = "{msg}" + ModMain.instance.getNameManager().getPlayerName() + ": " + String.Join(" ", words);

			ModMain.instance.sendToPlayer(player, message);
			return true;
		}else{
			ModMain.instance.sendToPlayer(new string[]{"That player is not online."});
			return true;
		}
	}

    public string getDescriptionString() {
        return "Sends a message to another player.";
    }
}

