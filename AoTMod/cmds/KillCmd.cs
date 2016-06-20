using UnityEngine;
using System.Collections;
using System.Reflection;

public class KillCmd : ICommand{
	public static string titanName = "Titan";
    public static KillMode killMode = KillMode.SILENT;
    public enum KillMode {
        SILENT,
        TITAN_NAME
    }
	public bool cmd(string[] args, FengGameManagerMKII gm){
		if (args.Length < 1){
			ModMain.instance.sendToPlayer(new string[]{
				"/kill <id|*>",
				"/kill -a -> kills all annies",
				"/kill -c -> kills all colossal titans",
			});
			return true;
		}
		PhotonPlayer target = null;

		if (args[0] == "*"){
			this.killAll(gm);
			return true;
		}

		if (args[0] == "-a"){
			this.killAnnies();
			return true;
		}

		if (args[0] == "-c"){
			this.killCTs();
			return true;
		}

		foreach (PhotonPlayer p in PhotonNetwork.playerList){
			if ("" + p.ID == args[0]){
				target = p;
				break;
			}
		}

		if (target != null){
			this.killPlayer(target);
			return true;
		}

		return true;
	}

	private void killPlayer(PhotonPlayer player){
		HERO[] arr = Object.FindObjectsOfType<HERO>() as HERO[];
		string name = (string)player.customProperties[PhotonPlayerProperty.name];

		foreach (HERO h in arr){
			if(h.photonView.ownerId == player.ID){
				h.photonView.RPC("netDie2", PhotonTargets.All, new object[]{int.MinValue, (KillCmd.killMode == KillMode.SILENT) ? string.Empty : KillCmd.titanName});
				return;
			}
		}

		foreach(TITAN t in GameObject.FindObjectsOfType<TITAN>()){
			if (t.photonView.ownerId == player.ID){
				t.photonView.RPC("netDie", PhotonTargets.All, new object[0]);
				break;
			}
		}

	}

	private  void killAll(FengGameManagerMKII gm){
		HERO[] arr = Object.FindObjectsOfType<HERO>() as HERO[];
		
		foreach (HERO h in arr){
            h.photonView.RPC("netDie2", PhotonTargets.All, new object[] { int.MinValue, (KillCmd.killMode == KillMode.SILENT) ? string.Empty : KillCmd.titanName });
		}
	}

	private void killAnnies(){
		FEMALE_TITAN[] annies = GameObject.FindObjectsOfType<FEMALE_TITAN>();

		foreach (FEMALE_TITAN annie in annies){
			PhotonNetwork.Destroy(annie.photonView);
		}
	}

	private void killCTs(){
		COLOSSAL_TITAN[] cts = GameObject.FindObjectsOfType<COLOSSAL_TITAN>();

		foreach(COLOSSAL_TITAN ct in cts){
			PhotonNetwork.Destroy(ct.photonView);
		}
	}

    public string getDescriptionString() {
        return "Kills a player/Annies/CTs.";
    }
}

