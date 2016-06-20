using ExitGames.Client.Photon;
using System;

namespace AoTMod.cmds {
    class FakeModCmd : ICommand {
        private string[] help = {
            "/fakeMod <on|off>",
            "/fakeMod list - Shows the list of mods you can fake as.",
            "/fakeMod as <ListIndex> - Fake as that mod.",
            "/fakeMod set - Sets your properties if you were in a room already."
        };

        private ModInfo[] infos = {
            new ModInfo() { modName = "Raoph's mod", propertyName = "raohsopmod", propertyValue = "" },
            new ModInfo() { modName = "Ghost's mod", propertyName = "GHOST", propertyValue = "" },
            new ModInfo() { modName = "NRC", propertyName = "NRC", propertyValue = "" },
            new ModInfo() { modName = "SRC", propertyName = "SRC", propertyValue = "" },
            new ModInfo() { modName = "Nathan's mod", propertyName = "Nathan", propertyValue = "" },
            new ModInfo() { modName = "Kage no kishi", propertyName = "KageNoKishi", propertyValue = "" },
            new ModInfo() { modName = "Pix's mod", propertyName = "Angry_Guest", propertyValue = "" },
            new ModInfo() { modName = "Cyan mod", propertyName = "CyanMod", propertyValue = "" },
            new ModInfo() { modName = "Cyan mod new", propertyName = "CyanModNew", propertyValue = "" },
            new ModInfo() { modName = "OhSoShO", propertyName = "OhSoShO", propertyValue = "" },
            new ModInfo() { modName = "Cear", propertyName = "Cear", propertyValue = "" },
            new ModInfo() { modName = "Arch", propertyName = "Arch", propertyValue = "" },
            new ModInfo() { modName = "Redskies", propertyName = "REDSKIES", propertyValue = "" },
        };
        public bool cmd(string[] args, FengGameManagerMKII gm) {
            if (args.Length == 0) {
                ModMain.instance.sendToPlayer(this.help);
                return true;
            }

            if (args[0].Equals("on", StringComparison.OrdinalIgnoreCase)) {
                ModMain.instance.setFakingAsOtherMod(true);
            }else if (args[0].Equals("off", StringComparison.OrdinalIgnoreCase)) {
                ModMain.instance.setFakingAsOtherMod(false);
                Hashtable table = new Hashtable();

                foreach(ModInfo info in this.infos) {
                    table[info.propertyName] = null;
                }

                PhotonNetwork.player.SetCustomProperties(table);
            } else if (args[0].Equals("list", StringComparison.OrdinalIgnoreCase)) {
                ModMain.instance.sendToPlayer("Index || Mod name || Mod property || Known property value");
                int index = 0;
                foreach(ModInfo info in this.infos) {
                    ModMain.instance.sendToPlayer(index + " || " + info.modName + " || " + info.propertyName + " || " + info.propertyValue);
                    index++;
                }
            } else if (args[0].Equals("as", StringComparison.OrdinalIgnoreCase)) {
                if (args.Length < 2) {
                    ModMain.instance.sendToPlayer("Missing mod indexes.");
                } else {
                    string[] names = new string[args.Length - 1];
                    object[] values = new object[args.Length - 1];

                    for(int i = 1; i < args.Length; i++) {
                        names[i - 1] = this.infos[i].propertyName;
                        values[i - 1] = this.infos[i].propertyValue;
                    }

                    ModMain.instance.setFakeModProperties(names, values);
                    ModMain.instance.sendToPlayer("Properties set.");
                }
            } else if (args[0].Equals("set", StringComparison.OrdinalIgnoreCase)) {
                ModMain.instance.updateFakeModProperties();
            } else {
                ModMain.instance.sendToPlayer("No such argument: " + args[0]);
            }

            return true;
        }

        public string getDescriptionString() {
            return "Fake this mod as other mods.";
        }

        private struct ModInfo {
            public string modName;
            public string propertyName;
            public object propertyValue;
        }
    }
}
