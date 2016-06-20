using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
    class DetectCmd : ICommand{
        public static HashSet<string> commonProperties = new HashSet<string>();
        public DetectCmd() {
            foreach (FieldInfo field in typeof(PhotonPlayerProperty).GetFields(BindingFlags.Static | BindingFlags.Public)) {
                commonProperties.Add((string) field.GetValue(null));
            }
            commonProperties.Add("sender");
            /*
            this.commonProperties.Add(PhotonPlayerProperty.beard_texture_id);
            this.commonProperties.Add(PhotonPlayerProperty.body_texture);
            this.commonProperties.Add(PhotonPlayerProperty.cape);
            this.commonProperties.Add(PhotonPlayerProperty.character);
            this.commonProperties.Add(PhotonPlayerProperty.costumeId);
            this.commonProperties.Add(PhotonPlayerProperty.currentLevel);
            this.commonProperties.Add(PhotonPlayerProperty.customBool);
            this.commonProperties.Add(PhotonPlayerProperty.customFloat);
            this.commonProperties.Add(PhotonPlayerProperty.customInt);
            this.commonProperties.Add(PhotonPlayerProperty.customString);
            this.commonProperties.Add(PhotonPlayerProperty.beard_texture_id);
             * */
        }

        public bool cmd(string[] args, FengGameManagerMKII gm) {
            bool showPropertyValue = args.Length > 0 && args[0].Equals("-s");
            int index = showPropertyValue ? 1 : 0;

            if (index >= args.Length) {
                foreach (PhotonPlayer player in PhotonNetwork.otherPlayers) {
                    foreach (string key in player.customProperties.Keys) {
                        if (!commonProperties.Contains(key)) {
                            ModMain.instance.sendToPlayer(Colorizer.colorize("Player #" + player.ID + " has hidden property: " + key + ((showPropertyValue) ? (" Value: " + player.customProperties[key]) : ""), Colorizer.Color.YELLOW, true));
                        }
                    }
                }
            } else {
                for (int i = ((showPropertyValue) ? 1 : 0); i < args.Length; i++) {
                    PhotonPlayer player = PhotonPlayer.Find(int.Parse(args[i]));

                    foreach (string key in player.customProperties.Keys) {
                        if (!commonProperties.Contains(key)) {
                            ModMain.instance.sendToPlayer(Colorizer.colorize("Player #" + player.ID + " has hidden property: " + key + ((showPropertyValue) ? (" Value: " + player.customProperties[key]) : ""), Colorizer.Color.YELLOW, true));
                        }
                    }
                }
            }

            return true;
        }

        public string getDescriptionString() {
            return "Shows the hidden properties of a player.";
        }
    }
