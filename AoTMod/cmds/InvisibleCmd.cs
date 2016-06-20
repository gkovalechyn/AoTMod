using ExitGames.Client.Photon;
using System;
using System.Text;

namespace AoTMod.cmds {
    class InvisibleCmd : ICommand{
        private string[] help = {
                                    "/hidden on - Hides you on the list",
                                    "/hidden off - Makes you visible"
                                };
        public bool cmd(string[] args, FengGameManagerMKII gm) {
            if (args.Length == 0) {
                ModMain.instance.sendToPlayer(this.help);
                return true;
            }

            if (args[0].Equals("on", StringComparison.OrdinalIgnoreCase)) {
                Hashtable table = new Hashtable();
                table[PhotonPlayerProperty.dead] = null;
                PhotonNetwork.player.SetCustomProperties(table);
                ModMain.instance.setHidden(true);
            } else if (args[0].Equals("off", StringComparison.OrdinalIgnoreCase)) {
                Hashtable table = new Hashtable();
                table[PhotonPlayerProperty.dead] = false;
                PhotonNetwork.player.SetCustomProperties(table);
                ModMain.instance.setHidden(true);
            } else {
                ModMain.instance.sendToPlayer(this.help);
            }

            return true;
        }

        public string getDescriptionString() {
            return "Toggles you being invisible in the player list. (Good mods will see pass this)";
        }
    }
}
