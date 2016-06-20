using UnityEngine;
//using System.Collections;
using System.Reflection;
using ExitGames.Client.Photon;
public class DestroyCmd : ICommand{
	public bool cmd(string[] args, FengGameManagerMKII gm){
        int id = int.Parse(args[0]);
        PhotonPlayer player = PhotonPlayer.Find(id);
		if (player != null){
			PhotonNetwork.DestroyPlayerObjects(player);
            this.tryToRemoveGameManager(id);
			ModMain.instance.sendToPlayer("Destroyed objects.");
		}else{
			ModMain.instance.sendToPlayer("Player with ID " + args[0] + " not found");
		}
		return true;
	}

    public string getDescriptionString() {
        return "Destroys the gameobjects of players.";
    }

    private void tryToRemoveGameManager(int playerId) {
        //FieldInfo networkingField = typeof(PhotonNetwork).GetField("networkingPeer", BindingFlags.NonPublic | BindingFlags.Static);
        //object networkingPeer = networkingField.GetValue(null);
       // ModMain.instance.log("Networking peer: " + networkingPeer);
        //MethodInfo method = networkingPeer.GetType().GetMethod("ServerCleanInstantiateAndDestroy", BindingFlags.NonPublic | BindingFlags.Instance);
        //MethodInfo method = networkingPeer.GetType().GetMethod("OpRaiseEvent", BindingFlags.Public | BindingFlags.Instance);
        //ModMain.instance.log("Method: " + method);
        Hashtable table1 = new Hashtable();
        //Hashtable table2 = new Hashtable();
        //table2[0] = 2;
        table1[254] = playerId;
        table1[0] = 2;
        
        RaiseEventOptions opts = new RaiseEventOptions();
        opts.TargetActors = new int[] { playerId };
        opts.CachingOption = ExitGames.Client.Photon.Lite.EventCaching.DoNotCache;

        PhotonNetwork.networkingPeer.OpRaiseEvent(204, table1, true, opts);
        //method.Invoke(networkingPeer, new object[] { (byte) 204, table1, true, opts});
    }
}

