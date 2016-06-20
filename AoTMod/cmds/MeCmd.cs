using UnityEngine;
using System.Collections;

public class MeCmd : ICommand{
	//me <message>
	public bool cmd(string[] args, FengGameManagerMKII gm){
		string message = "*" + ModMain.instance.getNameManager().getPlayerName() + " ";
		message += string.Join(" ", args);
		message += "*";

		gm.photonView.RPC("Chat", PhotonTargets.All, new object[]{message, string.Empty});
		return true;
	}

    public string getDescriptionString() {
        return "Performs an action in chat.";
    }
}

