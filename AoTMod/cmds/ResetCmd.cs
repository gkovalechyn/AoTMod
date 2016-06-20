using UnityEngine;
using System.Collections;
using System.Reflection;

public class ResetCmd : ICommand{
	private string[] help = {
		"/reset help",
		"/reset - Reset stats.",
		"/reset <name|guild|dmg|max_dmg|total_dmg|kills|deaths>",
		"/reset -c <name|guild|chatName> - Reset from the loaded config.",
		"Usage: /reset <option1> [option2] ... [optionN] (/reset name guild)"
	};
	public float originalSpeed = -1f;

	public bool cmd(string[] args, FengGameManagerMKII gm){
		ExitGames.Client.Photon.Hashtable toSet = new ExitGames.Client.Photon.Hashtable();

		if (args.Length == 0){
			toSet.Add(PhotonPlayerProperty.kills, 0);
			toSet.Add(PhotonPlayerProperty.deaths, 0);
			toSet.Add(PhotonPlayerProperty.max_dmg, 0);
			toSet.Add(PhotonPlayerProperty.total_dmg, 0);

			PhotonNetwork.player.SetCustomProperties(toSet);
            ModMain.instance.getModMainThread().updateInternalPlayerProperties();
		}else{
			if (args[0].Equals("help", System.StringComparison.OrdinalIgnoreCase)){
				ModMain.instance.sendToPlayer(this.help);
				return true;
			}

            if (args[0].Equals("-c")) { //Load from the configuration
                ConfigManager cfg = ModMain.instance.getConfig();

                for (int i = 1; i < args.Length; i++) {
                    if (args[i].Equals("name", System.StringComparison.OrdinalIgnoreCase)) {
                        toSet[PhotonPlayerProperty.name] = cfg.get("displayName");
                        LoginFengKAI.player.name = cfg.get("displayName");
                    } else if (args[i].Equals("guild", System.StringComparison.OrdinalIgnoreCase)) {
                        toSet[PhotonPlayerProperty.guildName] = cfg.get("guild");
                        LoginFengKAI.player.guildname = cfg.get("guild");
                    } else if (args[i].Equals("chatName", System.StringComparison.OrdinalIgnoreCase)) {
                        ModMain.instance.getNameManager().setName(cfg.get("name"));
                    }
                }
            } else {
                for (int i = 0; i < args.Length; i++) {
                    if (args[i].Equals("name", System.StringComparison.OrdinalIgnoreCase)) {
                        toSet.Add(PhotonPlayerProperty.name, ModMain.instance.getNameManager().getPlayerDisplayName());
                        LoginFengKAI.player.name = ModMain.instance.getNameManager().getPlayerDisplayName();
                    } else if (args[i].Equals("guild", System.StringComparison.OrdinalIgnoreCase)) {
                        toSet.Add(PhotonPlayerProperty.guildName, ModMain.instance.getNameManager().getPlayerGuild());
                        LoginFengKAI.player.guildname = ModMain.instance.getNameManager().getPlayerGuild();
                    } else if (args[i].Equals("max_dmg", System.StringComparison.OrdinalIgnoreCase)) {
                        toSet.Add(PhotonPlayerProperty.max_dmg, 0);
                    } else if (args[i].Equals("total_dmg", System.StringComparison.OrdinalIgnoreCase)) {
                        toSet.Add(PhotonPlayerProperty.total_dmg, 0);
                    } else if (args[i].Equals("kills", System.StringComparison.OrdinalIgnoreCase)) {
                        toSet.Add(PhotonPlayerProperty.kills, 0);
                    } else if (args[i].Equals("deaths", System.StringComparison.OrdinalIgnoreCase)) {
                        toSet.Add(PhotonPlayerProperty.deaths, 0);
                    }
                }
            }

			PhotonNetwork.player.SetCustomProperties(toSet);
            ModMain.instance.getModMainThread().updateInternalPlayerProperties();
		}

		return true;
	}

    public string getDescriptionString() {
        return "Reset your stats. (/reset help)";
    }

}

