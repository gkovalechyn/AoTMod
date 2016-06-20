using UnityEngine;
using System.Collections;

public class BanCmd : ICommand{
	//ban <id>
	//ban -r <index>
	//ban -s <name>
	//ban
    private string[] help = {
                              "/ban <id> - Bans the player with that ID.",
                              "/ban -r <index> - Removes the banned player on that index. (See /ban index)",
                              "/ban -s <name> - Bans a string, any player with that string in its name is banned.",
                              "/ban index - Shows the banned players/names"

                          };
	public bool cmd(string[] args, FengGameManagerMKII gm){
        if (args.Length == 0) {
            ModMain.instance.sendToPlayer(this.help);
            return true;
        }

		if (args[0].Equals("index", System.StringComparison.OrdinalIgnoreCase)){
			int i = 0;
			ModMain.instance.sendToPlayer("Banned players: ");
			Hashtable bans = ModMain.instance.getModMainThread().getBannedNames();

			foreach(string s in bans.Keys){
				ModMain.instance.sendToPlayer("" + (i++) + "- " + s + "#" + bans[s]);
			}

			return true;
		}

		if (args[0].Equals("-r")){
			ModMain.instance.getModMainThread().unban(int.Parse(args[1]));
			ModMain.instance.sendToPlayer("Player unbanned.");
			return true;
		}else if (args[0].Equals("-s")){
			ModMain.instance.getModMainThread().banName(args[1], MainModThread.BanType.CONTAINS_PART);
			return true;
		}else{
			try{
				string name = (string) PhotonPlayer.Find(int.Parse(args[0])).customProperties[PhotonPlayerProperty.name];
				ModMain.instance.getModMainThread().banName(ModMain.stripColorCodes(name), MainModThread.BanType.FULL_NAME);
				return true;
			}catch(System.Exception){
				return false;
			}
		}
	}

    public string getDescriptionString() {
        return "Ban/unban players.";
    }

}

