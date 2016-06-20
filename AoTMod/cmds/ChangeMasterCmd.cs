using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

public class ChangeMasterCmd : ICommand{
	public bool cmd(string[] args, FengGameManagerMKII gm){
		ModMain.instance.log("Changing master client to player with id " + args[0]);

        if (args[0].Equals("-t")) {
            this.testFakeMCChange(int.Parse(args[1]));
            return true;
        }
		foreach(PhotonPlayer player in PhotonNetwork.playerList){

			if (("" + player.ID) == args[0]){
				PhotonNetwork.SetMasterClient(player);
				return true;
			}
		}
		
		return true;
	}

    public string getDescriptionString() {
        return "Changes the master client to another player.";
    }

    private void testFakeMCChange(int id) {
        PhotonNetwork.logLevel = PhotonLogLevel.Full;

        Type type = typeof(PhotonNetwork);
        FieldInfo networkingPeerField = type.GetField("networkingPeer", BindingFlags.Static | BindingFlags.NonPublic);
        object networkingPeer = networkingPeerField.GetValue(null);
        MethodInfo raiseOpMethod = networkingPeer.GetType().GetMethod("OpRaiseEvent", BindingFlags.Public | BindingFlags.Instance);
        ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable();
        RaiseEventOptions opts = new RaiseEventOptions();

        table[0] = id;
        table[1] = id;
        table[2] = id;

        opts.Receivers = ExitGames.Client.Photon.Lite.ReceiverGroup.All;
        //        opts.Receivers = ExitGames.Client.Photon.Lite.ReceiverGroup.All;
        // opts.CachingOption = ExitGames.Client.Photon.Lite.EventCaching.AddToRoomCache;

        try {
            foreach (PhotonPlayer player in PhotonNetwork.playerList) {
                table[1] = player.ID;
                raiseOpMethod.Invoke(networkingPeer, new object[] { (byte)208, table, true, opts });
            }

        } catch (System.Exception e) {
            ModMain.instance.log(e);
        }
    }
}

