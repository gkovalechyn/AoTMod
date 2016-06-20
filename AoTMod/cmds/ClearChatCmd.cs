using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
class ClearChatCmd : ICommand{
    private string[] help = {
                                "/clearChat <id> ... [id] - Clears the chat for those players.",
                                "/clearChat self - Clears your chat",
                                "/clearChat * - Clears the chat for everyone.",
                                "/clearChat + - Clears the chat for everyone else."

                            };
    public bool cmd(string[] args, FengGameManagerMKII gm) {
        if (args.Length == 0) {
            ModMain.instance.sendToPlayer(this.help);
        }

        if (args[0].Equals("self", StringComparison.OrdinalIgnoreCase)) {
            if (InRoomChat.messages != null) {
                InRoomChat.messages.Clear();
            }
        } else if (args[0].Equals("*", StringComparison.OrdinalIgnoreCase)) {
            if (InRoomChat.messages != null) {
                InRoomChat.messages.Clear();
            }

            for (int i = 0; i < 16; i++) {
                gm.photonView.RPC("Chat", PhotonTargets.Others, "", "");
            }
        } else if (args[0].Equals("+", StringComparison.OrdinalIgnoreCase)) {
            for (int i = 0; i < 16; i++) {
                gm.photonView.RPC("Chat", PhotonTargets.Others, "", "");
            }
        } else {
            PhotonPlayer[] players = new PhotonPlayer[args.Length];
            for(int i = 0; i < players.Length; i++){
                players[i] = PhotonPlayer.Find(int.Parse(args[i]));
            }

            for (int i = 0; i < args.Length; i++) {
                foreach (PhotonPlayer player in players) {
                    gm.photonView.RPC("Chat", player, "", "");
                }
            }
        }
        

        return true;
    }

    public string getDescriptionString() {
        return "Clears the chat.";
    }
}
