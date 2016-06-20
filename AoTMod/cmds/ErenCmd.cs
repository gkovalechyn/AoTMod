using UnityEngine;
using System.Collections;
using System.Reflection;
public class ErenCmd : ICommand{
	public bool cmd(string[] args, FengGameManagerMKII gm){
		string id = args.Length > 0 ? args[0] : ("" + PhotonNetwork.player.ID);

		foreach (HERO h in GameObject.FindObjectsOfType<HERO>()){
			if ("" + h.photonView.ownerId == id){
				MethodInfo erenFormMethod = h.GetType().GetMethod("erenTransform", BindingFlags.NonPublic | BindingFlags.Instance);
				erenFormMethod.Invoke(h, new object[0]);
				break;
			}
		}

		return true;
	}

    public string getDescriptionString() {
        return "Transforms you/other players into eren titan.";
    }
}

