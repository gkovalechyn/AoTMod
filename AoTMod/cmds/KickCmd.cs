using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class KickCmd : ICommand{
	public bool cmd(string[] args, FengGameManagerMKII gm){
		ModMain.instance.log("kicking player with id " + args[0]);

		if (args[0].Equals("-o")){//o = original
			ModMain.instance.sendToAll("/kick #" + args[1]);
			return true;
		}

        if (args[0].Equals("-t")) {
            this.kickPlayer2(int.Parse(args[1]));
            return true;
        }

        if (args[0].Equals("-t2")) {
            this.tryOpCustom(int.Parse(args[1]));
            return true;
        }
        
        if (args[0].Equals("-f")){
			for (int i =0;	i < Math.Ceiling((float)PhotonNetwork.room.playerCount / 2); i++){
				gm.photonView.RPC("Chat", PhotonTargets.All, new object[]{"/kick #" + args[1], "GUEST" + UnityEngine.Random.Range(0, 10000)});
			}

			return true;
		}

        try {
            this.sendChangeMC(int.Parse(args[0]));
            PhotonNetwork.CloseConnection(PhotonPlayer.Find(int.Parse(args[0])));
        } catch (System.Exception e) {
            ModMain.instance.log(e);
        }
		
		return true;
	}

	private void kickPlayer(string name,  FengGameManagerMKII gm){
		MethodInfo mInfo = gm.GetType().GetMethod("kickPhotonPlayer", BindingFlags.Instance | BindingFlags.NonPublic);
		mInfo.Invoke(gm, new object[]{name});
	}

	private void kickPlayer2(int id){
		Type type = typeof(PhotonNetwork);
		ModMain.instance.log("Got the PhotonNetwork type");
		FieldInfo networkingPeerField = type.GetField("networkingPeer", BindingFlags.Static | BindingFlags.NonPublic);
		object networkingPeer = networkingPeerField.GetValue(null);

		ModMain.instance.log("Networking peer: " + networkingPeer);
		ModMain.instance.log("got the networkPeer value");
		/*
		MethodInfo raiseOpMethod = networkingPeer.GetType().GetMethod("OpRaiseEvent", 
		                                                              BindingFlags.Instance | BindingFlags.Public,
		                                                              null,
		                                                              new Type[]{typeof(Byte), typeof(Hashtable), typeof(Boolean), typeof(Byte), typeof(Int32[])},
																	  null);
		*/
        MethodInfo raiseOpMethod = networkingPeer.GetType().GetMethod("OpRaiseEvent", BindingFlags.Public | BindingFlags.Instance);
        ModMain.instance.log("got the OpRaiseEvent method");
		ModMain.instance.log("Method: " + raiseOpMethod);
        ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable();
        table[254] = PhotonNetwork.masterClient.ID;
        RaiseEventOptions opts = new RaiseEventOptions();
        opts.TargetActors = new int[] { id };
        //opts.Receivers = ExitGames.Client.Photon.Lite.ReceiverGroup.Others;
        //opts.CachingOption = ExitGames.Client.Photon.Lite.EventCaching.DoNotCache;

		try{
			raiseOpMethod.Invoke(networkingPeer, new object[]{(byte)203, table, true, opts});
		}catch(Exception e){
			ModMain.instance.log(e);
		}
		ModMain.instance.log("After invoke of the RaiseOP method");
	}

    private void sendChangeMC(int targetId) {
        Type type = typeof(PhotonNetwork);
        //FieldInfo networkingPeerField = type.GetField("networkingPeer", BindingFlags.Static | BindingFlags.Public);
        //object networkingPeer = networkingPeerField.GetValue(null);
        //MethodInfo raiseOpMethod = networkingPeer.GetType().GetMethod("OpRaiseEvent", BindingFlags.Public | BindingFlags.Instance);
        ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable();
        RaiseEventOptions opts = new RaiseEventOptions();

        table[1] = PhotonNetwork.player.ID;
        
        opts.TargetActors = new int[] { targetId };
        opts.Receivers = ExitGames.Client.Photon.Lite.ReceiverGroup.All;
       // opts.CachingOption = ExitGames.Client.Photon.Lite.EventCaching.AddToRoomCache;

        try {
            PhotonNetwork.networkingPeer.OpRaiseEvent(208, table, true, opts);
            //raiseOpMethod.Invoke(networkingPeer, new object[] { (byte)208, table, true, opts });
        } catch (Exception e) {
            ModMain.instance.log(e);
        }
    }
    private void tryOpCustom(int targetId) {
        Dictionary<byte, object> dic = new Dictionary<byte, object>();
        Type type = typeof(PhotonNetwork);
        FieldInfo networkingPeerField = type.GetField("networkingPeer", BindingFlags.Static | BindingFlags.NonPublic);
        object networkingPeer = networkingPeerField.GetValue(null);
        MethodInfo method = typeof(ExitGames.Client.Photon.PhotonPeer).GetMethod("OpCustom", BindingFlags.Instance | BindingFlags.Public, Type.DefaultBinder,  new Type[]{typeof(byte), typeof(Dictionary<byte, object>), typeof(bool), typeof(byte), typeof(bool)}, null);

        RaiseEventOptions opts = new RaiseEventOptions();
        /*
        ModMain.instance.log("NetworkingPeer methods::");
        foreach (MethodInfo info in networkingPeer.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)) {
            ModMain.instance.log("\t" + info.Name + "- ");
            foreach (ParameterInfo pi in info.GetParameters()) {
                ModMain.instance.log("\t\t" + pi.Name + "(" + pi.GetType() + ")");
            }
        }

        ModMain.instance.log("PhotonPeer methods::");
        foreach (MethodInfo info in typeof(ExitGames.Client.Photon.PhotonPeer).GetMethods(BindingFlags.Instance | BindingFlags.Public)) {
            ModMain.instance.log("\t" + info.Name + "- ");
            foreach(ParameterInfo pi in info.GetParameters()){
                ModMain.instance.log("\t\t" + pi.Name + "(" + pi.ParameterType + ")");
            }
        }
        */
        PhotonNetwork.logLevel = PhotonLogLevel.Full;

        opts.TargetActors = new int[] { targetId };
        //opts.Receivers = ExitGames.Client.Photon.Lite.ReceiverGroup.Others;
        //opts.CachingOption = ExitGames.Client.Photon.Lite.EventCaching.DoNotCache;

        dic[244] = (byte) 203;
        dic[254] = 0;
        dic[252] = opts.TargetActors;

        if (opts.ForwardToWebhook) {
            dic[234] = true;
        }

        if (opts.Receivers != ExitGames.Client.Photon.Lite.ReceiverGroup.Others) {
            dic[246] = (byte)opts.Receivers;
        }

        if (opts.CachingOption != 0) {
            dic[247] = (byte)opts.CachingOption;
        }

        method.Invoke(networkingPeer, new object[] { (byte)253, dic, true, opts.SequenceChannel, false });
    }

    public string getDescriptionString() {
        return "Kicks a player.";
    }
}

