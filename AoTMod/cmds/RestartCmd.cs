using UnityEngine;
using System.Collections;
using System.Reflection;

public class RestartCmd : ICommand{
	public bool cmd(string[] args, FengGameManagerMKII gm){
		//MethodInfo respawnMethod = gm.GetType().GetMethod("restartGame", BindingFlags.Instance | BindingFlags.NonPublic);
		//respawnMethod.Invoke(gm, new object[]{false});
		if (args.Length != 0){
			PhotonPlayer[] players = new PhotonPlayer[args.Length];
			object[] oargs = new object[0];

			for(int i = 0; i < args.Length; i++){
				players[i] = PhotonPlayer.Find(int.Parse(args[i]));
			}

			foreach (PhotonPlayer player in players){
				gm.photonView.RPC("RPCLoadLevel", player, oargs);
			}
		}else{
			gm.photonView.RPC("RPCLoadLevel", PhotonTargets.All, new object[0]);
		}
		return true;
	}

    public string getDescriptionString() {
        return "Restarts the game.";
    }
}

