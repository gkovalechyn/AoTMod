using UnityEngine;
using System.Collections;

public class DeathListCmd : ICommand{
	//dl <ID>
	//dl -r <index>
	//dl
    private string[] help = {
                                "/dl <on|off> - Toggles the death list",
                                "/dl add <ID> ... [ID] - Adds id's to the list.",
                                "/dl -r <id> ... [id] - Removes IDs from the list.",
                                "/dl info - Shows information about the death list."
                            };
	public bool cmd(string[] args, FengGameManagerMKII gm){
		if (args.Length == 0){
            ModMain.instance.sendToPlayer(this.help);
			return true;
		}

		if (args[0].Equals("on", System.StringComparison.OrdinalIgnoreCase)){
			ModMain.instance.getModMainThread().setDeathListEnabled(true);
			return true;
		}

		if (args[0].Equals("off", System.StringComparison.OrdinalIgnoreCase)){
			ModMain.instance.getModMainThread().setDeathListEnabled(false);
			return true;
		}

		if (args[0].Equals("-r")){
            for (int i = 1; i < args.Length; i++){
                ModMain.instance.getModMainThread().removeDeathList(int.Parse(args[i]));
            }
			return true;
		}

        if (args[0].Equals("add", System.StringComparison.OrdinalIgnoreCase)){
            for(int i = 1; i < args.Length; i++){
                ModMain.instance.getModMainThread().addToDeathList(int.Parse(args[i]));
            }
            return true;
        }

        if (args[0].Equals("info", System.StringComparison.OrdinalIgnoreCase)){
			ModMain.instance.sendToPlayer("Is death list enabled: " + ModMain.instance.getModMainThread().isDeathListEnabled());
			foreach(int id in ModMain.instance.getModMainThread().getDeathList()){
				ModMain.instance.sendToPlayer("-" + id);
			}
            return true;
        }
        ModMain.instance.sendToPlayer("No such argument: " + args[1]);
        return true;
	}

    public string getDescriptionString() {
        return "Controls the death list, the players who will be killed if they are alive.";
    }
}

