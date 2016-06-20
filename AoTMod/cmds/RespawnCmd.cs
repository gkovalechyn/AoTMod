using UnityEngine;
using System.Collections;
using System.Reflection;

public class RespawnCmd : ICommand{
	public bool cmd(string[] args, FengGameManagerMKII gm){
		if (args.Length == 0){
			FieldInfo lastCharacterField = gm.GetType().GetField("myLastHero", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
			ModMain.instance.log("MyLastHero: " + lastCharacterField.GetValue(gm));

			gm.SpawnPlayer((string) lastCharacterField.GetValue(gm), "playerRespawn");
			return true;
		}else if (args[0].Equals("*")){
			ModMain.instance.getGameManager().photonView.RPC("respawnHeroInNewRound", PhotonTargets.All, new object[0]);
			return true;
		}else{
			PhotonPlayer target = null;
			foreach (PhotonPlayer p in PhotonNetwork.playerList){
				if ("" + p.ID == args[0]){
					target = p;
					break;
				}
			}
			
			if (target != null){
				gm.photonView.RPC("respawnHeroInNewRound", target, new object[0]);
				return true;
			}
			
			return false;
		}

	}

    public string getDescriptionString() {
        return "Respawns you and/or another players.";
    }
}

