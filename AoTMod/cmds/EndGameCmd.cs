using UnityEngine;
using System.Collections;
using System.Reflection;

public class EndGameCmd : ICommand{
	//endGame <id|*>
	public bool cmd(string[] args, FengGameManagerMKII gm){
		if (args[0].Equals("*")){
			string[] res = EndGameCmd.generateEndStrings(gm);
			gm.photonView.RPC("showResult", PhotonTargets.Others, res);
		}else{
			foreach(PhotonPlayer p in PhotonNetwork.playerList){
				if ("" + p.ID == args[0]){
					string[] res = EndGameCmd.generateEndStrings(gm);
					gm.photonView.RPC("showResult", p, res);
				}
			}
		}

		return true;
	}

	private static string[] generateEndStrings(FengGameManagerMKII gm){
		string[] res = new string[6];
		foreach (PhotonPlayer player2 in PhotonNetwork.playerList){
			if (player2 != null){
				res[0] += player2.customProperties[PhotonPlayerProperty.name] + "\n";
				res[1] += player2.customProperties[PhotonPlayerProperty.kills] + "\n";
				res[2] += player2.customProperties[PhotonPlayerProperty.deaths] + "\n";
				res[3] += player2.customProperties[PhotonPlayerProperty.max_dmg] + "\n";
				res[4] += player2.customProperties[PhotonPlayerProperty.total_dmg] + "\n";
			}
		}

		if (IN_GAME_MAIN_CAMERA.gamemode == GAMEMODE.SURVIVE_MODE){
			FieldInfo waveField = gm.GetType().GetField("highestwave", BindingFlags.NonPublic | BindingFlags.Instance);
			res[5] = "Highest Wave : " + waveField.GetValue(gm);
		}
		else{
			FieldInfo humanScoreField = gm.GetType().GetField("humanScore", BindingFlags.NonPublic | BindingFlags.Instance);
			FieldInfo titanScoreField = gm.GetType().GetField("titanScore", BindingFlags.NonPublic | BindingFlags.Instance);
			res[5] = string.Concat(new object[] { "Humanity ", humanScoreField.GetValue(gm), " : Titan ", titanScoreField.GetValue(gm) });
		}

		return res;
	}

    public string getDescriptionString() {
        return "Sends the endgame screen to other players.";
    }
}

