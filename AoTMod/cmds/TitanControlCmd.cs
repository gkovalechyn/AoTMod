using UnityEngine;
using System.Collections;

public class TitanControlCmd : ICommand{
	private static string[] help = {
		"Command removed",
	};
	public bool cmd(string[] args, FengGameManagerMKII gm){
		if (args.Length == 0){
			ModMain.instance.sendToPlayer(help);
			return true;
		}

		if (args[0].Equals("info", System.StringComparison.OrdinalIgnoreCase)){
			sendInfo(gm);
			return true;
		}

		ModMain.instance.sendToPlayer(help);
		return true;
	}

	private void sendInfo(FengGameManagerMKII gm){
		ModMain.instance.sendToPlayer("Command removed.");
	}

    public string getDescriptionString() {
        return "Removed";
    }
}

