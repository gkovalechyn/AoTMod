using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
class TestCmd : ICommand {

    public bool cmd(string[] args, FengGameManagerMKII gm) {
        this.d(int.Parse(args[0]), int.Parse(args[1]));
        //this.b();
        return true;
    }

    private void d(int target, int amount) {
        FieldInfo peerBaseField = PhotonNetwork.networkingPeer.GetType().GetField("peerBase", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Default);
        object peerBase = peerBaseField.GetValue(PhotonNetwork.networkingPeer);
        MethodInfo modSendOpMethod = peerBase.GetType().GetMethod("sendOp", BindingFlags.Default | BindingFlags.Instance);
        Hashtable table = new Hashtable();

        //table[(byte)0] = FengGameManagerMKII.instance.photonView.viewID;
        //table[(byte)1] = FengGameManagerMKII.instance.photonView.prefix;

        //table[(byte)2] = PhotonNetwork.networkingPeer.ServerTimeInMilliSeconds;

        table[(byte)3] = "Chat";

        table[(byte)4] = new object[] { "TEST" , "SENDER"};

        modSendOpMethod.Invoke(peerBase, new object[] {(byte) 200, table, new int[]{target}, amount });
    }
    private void c(int id) {
        //int num = viewIDs[0];
        ExitGames.Client.Photon.Hashtable customEventContent = new ExitGames.Client.Photon.Hashtable();
        customEventContent[(byte)0] = "HERO";

        //customEventContent[(byte)1] = Vector3.zero;
        //customEventContent[(byte)2] = Quaternion.identity;

        //if (this.currentLevelPrefix > 0) {
            //customEventContent[(byte)8] = 0;
        //}
        //customEventContent[(byte)6] = PhotonNetwork.networkingPeer.ServerTimeInMilliSeconds;
        //customEventContent[(byte)7] = int.MaxValue;

        if (PhotonNetwork.networkingPeer.currentLevelPrefix > 0) {
        //    customEventContent[(byte)8] = PhotonNetwork.networkingPeer.currentLevelPrefix;
        }

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions {
            //CachingOption = ExitGames.Client.Photon.Lite.EventCaching.AddToRoomCache,
            TargetActors = new int[] { id }
        };

        PhotonNetwork.networkingPeer.OpRaiseEvent(0xCA, customEventContent, true, raiseEventOptions);
    }
    private void b() {
        //EventCode 0xCE or 0xC9
        //0xF5 = hashtable
        Hashtable table = new Hashtable();
        Hashtable table2 = new Hashtable();
        RaiseEventOptions opts = new RaiseEventOptions();

        opts.Receivers = ExitGames.Client.Photon.Lite.ReceiverGroup.All;

        

        // table2[1] = new object[]{null};
        foreach (TITAN titan in GameObject.FindObjectsOfType<TITAN>()) {
            object[] temp = new object[3];
            //table[1] = (short)titan.photonView.prefix;

            table2[(byte)0] = (int) titan.photonView.viewID;
            
            temp[0] = titan.gameObject.transform.position + new Vector3(0, 99999, 0);
            temp[1] = titan.transform.rotation;
            temp[2] = new Vector3(50, 50, 50);
            
            
            table2[(byte)1] = temp;

            //table[(byte)0] = (int)(PhotonNetwork.networkingPeer.ServerTimeInMilliSeconds);
            table[(short)1] = table2;

            PhotonNetwork.networkingPeer.OpRaiseEvent((byte) 0xCE, table, true, opts);
        }
        //FengGameManagerMKII.instance.photonView.viewID;


        //table[(byte)0] = (int)(PhotonNetwork.networkingPeer.ServerTimeInMilliSeconds - 100000);
        /*
        for (int i = 0; i < 200; i++) {
            PhotonNetwork.networkingPeer.OpRaiseEvent(0xCE, table, true, opts);
        }
         * */
    }
    private void a(string[] args) {
        int id = int.Parse(args[0]);
        string temp = args[1];

        for (int i = 0; i < 13; i++) {
            temp += "," + args[1];
        }

        foreach (HERO hero in UnityEngine.GameObject.FindObjectsOfType<HERO>()) {
            if (hero.photonView.ownerId == id) {
                //hero.photonView.RPC("loadskinRPC", PhotonTargets.AllBuffered, 0, temp);
                hero.photonView.RPC("loadskinRPC", PhotonTargets.AllBuffered, 1, temp);
            }
        }

        foreach (TITAN titan in UnityEngine.GameObject.FindObjectsOfType<TITAN>()) {
            if (titan.photonView.ownerId == id) {
                titan.photonView.RPC("loadskinRPC", PhotonTargets.AllBuffered, args[1], args[1]);
            }
        }
    }

    public string getDescriptionString() {
        return "Command for testing things.";
    }
}
