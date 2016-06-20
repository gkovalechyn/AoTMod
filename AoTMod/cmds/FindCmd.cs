using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
    class FindCmd : ICommand{
        private string help = "/find <player1> ... [PlayerN]";
        public bool cmd(string[] args, FengGameManagerMKII gm) {
            if (args.Length == 0) {
                ModMain.instance.sendToPlayer(this.help);
                return true;
            }

            int playersFound = 0;

            if (!PhotonNetwork.isMasterClient){
                playersFound = 1;
                string mcName = ModMain.stripColorCodes((string)PhotonNetwork.masterClient.customProperties[PhotonPlayerProperty.name]);
                ModMain.instance.sendToPlayer(Colorizer.colorize("Master client #" + PhotonNetwork.masterClient.ID + " (" + mcName + ")", Colorizer.Color.BLUE, true));
            }

            foreach (PhotonPlayer player in PhotonNetwork.playerList) {
                string unstripped = (string) player.customProperties[PhotonPlayerProperty.name];
                if (string.IsNullOrEmpty(unstripped)) {
                    ModMain.instance.sendToPlayer(Colorizer.colorize("Player #" + player.ID + " has a null name (" + unstripped + ").", Colorizer.Color.RED, true));
                    playersFound++;
                } else {
                    string stripped = ModMain.stripColorCodes(unstripped);
                    if (string.IsNullOrEmpty(stripped)) {
                        ModMain.instance.sendToPlayer("Player #" + player.ID + " only has colors in his name (" + unstripped + ").");
                        playersFound++;
                    } else {
                        foreach (string s in args) {
                            if (unstripped.Contains(s)) {
                                ModMain.instance.sendToPlayer("Found player #" + player.ID + " matching " + s + " (" + stripped + ")");
                                playersFound++;
                                break;
                            }
                        }
                    }
                }
            }

            ModMain.instance.sendToPlayer("Found " + playersFound + " matching given names.");

            return true;
        }

        public string getDescriptionString() {
            return "Find the IDs of other players based on their names.";
        }
    }
