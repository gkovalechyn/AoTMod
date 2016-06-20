using UnityEngine;
using System.Collections;

public class TransformCmd : ICommand{
	//transform type <type>
	//transform size <s>
	private string[] help = {
		"/transform type <type>",
		"/transform size <s>"
	};
	public bool cmd(string[] args, FengGameManagerMKII gm){
		if (args.Length == 0){
			ModMain.instance.sendToPlayer(help);
			return true;
		}

		if (args[0].Equals("type", System.StringComparison.OrdinalIgnoreCase)){
			AbnormalType type = TitanSize.getTitanType(args[1][0]);
			object[] aa = new object[]{(int) type};

			foreach(TITAN t in GameObject.FindObjectsOfType<TITAN>()){
				t.photonView.RPC("netSetAbnormalType", PhotonTargets.All, aa);
			}

			return true;
		}else if (args[0].Equals("size", System.StringComparison.OrdinalIgnoreCase)){
			float scale = float.Parse(args[1]);
			object[] aa = new object[3];

			aa[0] = scale;

			foreach(TITAN t in GameObject.FindObjectsOfType<TITAN>()){
				aa[1] = t.myDifficulty;
				aa[2] = 1;
				t.photonView.RPC("netSetLevel", PhotonTargets.All, aa);
			}

			return true;
		}else{
			ModMain.instance.sendToPlayer("Argument not recognized: " + args[0]);
			return true;
		}
	}

    public string getDescriptionString() {
        return "Transform all the titans currently spawned.";
    }
}

