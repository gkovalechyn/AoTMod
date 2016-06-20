using ExitGames.Client.Photon;
using System.Text;

class FakeCmd : ICommand{

    private string[] help = {
        "/fake <id> - Fakes you as that player.",
        "/fake -c <id> - Fakes you as that player, but converting their scoreboard name as your chat name."
    };
      public bool cmd(string[] args, FengGameManagerMKII gm) {
        if (args.Length == 0) {
            ModMain.instance.sendToPlayer(this.help);
            return true;
        }

        if (args[0].Equals("-c")) {
            PhotonPlayer player = PhotonPlayer.Find(int.Parse(args[1]));

            if (player == null) {
                ModMain.instance.sendToPlayer("That player is not online.");
                return true;
            }

            string name = (string)player.customProperties[PhotonPlayerProperty.name];
            string guild = (string)player.customProperties[PhotonPlayerProperty.guildName];
            string chatName = this.convertScoreboardName(name);

            this.setValues(name, guild, chatName);
            return true;
        } else {
            PhotonPlayer player = PhotonPlayer.Find(int.Parse(args[0]));

            if (player == null) {
                ModMain.instance.sendToPlayer("That player is not online.");
                return true;
            }

            if (!ModMain.instance.otherPlayerHasChatName(player.ID)) {
                ModMain.instance.sendToPlayer("That player hasn't said anything in chat yet. If you want to convert his scoreboard name, use the -c option.");
                return true;
            } else {
                this.setValues((string)player.customProperties[PhotonPlayerProperty.name], (string)player.customProperties[PhotonPlayerProperty.guildName], ModMain.instance.getOtherPlayersChatName(player.ID));
                return true;
            }
        }
    }

    private void setValues(string scoreboardName, string guild, string chatName) {
        Hashtable table = new Hashtable();

        table[PhotonPlayerProperty.name] = scoreboardName;
        table[PhotonPlayerProperty.guildName] = guild;

        PhotonNetwork.player.SetCustomProperties(table);

        LoginFengKAI.player.guildname = guild;
        LoginFengKAI.player.name = scoreboardName;

        ModMain.instance.getNameManager().setDisplayName(scoreboardName);
        ModMain.instance.getNameManager().setName(chatName);
        ModMain.instance.getNameManager().setGuild(guild);

        ModMain.instance.getModMainThread().updateInternalPlayerProperties();
    }

    private string convertScoreboardName(string name) {
        if (!string.IsNullOrEmpty(name)) {
            StringBuilder builder = new StringBuilder();
            int colorTags = 0;

            for (int i = 0; i < name.Length; i++) {
                if (name[i] == '[') {

                    if (name.Length - i >= 2) {
                        if (name[i + 1] == '-' && name[i + 2] == ']') {
                            if (colorTags > 0) {
                                builder.Append("</color>");
                                colorTags--;
                            }

                            i += 2;
                            continue;
                        }
                    }

                    if (name.Length - i >= 7) { //Has space for 6 Hex characters + ']'
                        bool isColorCode = true;

                        if (name[i + 7] != ']') {
                            isColorCode = false;
                        } else { 
                            for (int j = i + 1; j < i + 7; j++) {
                                if (!isHexadecimalCharacter(name[j])) {
                                    isColorCode = false;
                                    break;
                                }
                            }
                        }

                        if (isColorCode) {
                            builder.Append("<color=#");
                            for (int j = i + 1; j < i + 7; j++) {
                                builder.Append(name[j]);
                            }
                            builder.Append(">");

                            i += 7; //Will be increased by one at the end of the loop
                            colorTags++;
                            continue;
                        }
                    }

                    builder.Append(name[i]);
                } else {
                    builder.Append(name[i]);
                }
            }

            for(int i = 0; i < colorTags; i++) {
                builder.Append("</color>");
            }

            return builder.ToString();
        } else {
            return name;
        }
    }

    private bool isHexadecimalCharacter(char c) {
        c = char.ToLower(c);

        return c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8'
                 || c == '0' || c == 'a' || c == 'b' || c == 'c' || c == 'd' || c == 'e' || c == 'f';
    }
    public string getDescriptionString() {
        return "Fakes you as someone else on the player list.";
    }
}
