using UnityEngine;
using System.Collections;

public class InfoCmd : ICommand{
	public bool cmd(string[] args, FengGameManagerMKII gm){
		string message = "Use gas: ";

		if (ModMain.instance.useGas()){
			message += Colorizer.colorize("True", Colorizer.Color.GREEN, true);
		}else{
			message += Colorizer.colorize("False", Colorizer.Color.RED, true);
		}

		message += "\nUseBlades: ";

		if (ModMain.instance.useBlades()){
			message += Colorizer.colorize("True", Colorizer.Color.GREEN, true);
		}else{
			message += Colorizer.colorize("False", Colorizer.Color.RED, true);
		}
		message+= "\nTitan health mod enabled: ";

		if (ModMain.instance.getTHController().isEnabled()){
			message += Colorizer.colorize("True", Colorizer.Color.GREEN, true);
		}else{
			message += Colorizer.colorize("False", Colorizer.Color.RED, true);
		}

		message += "\nTitan base health: ";
		message += Colorizer.colorize("" + ModMain.instance.getTHController().getMinDamage(), Colorizer.Color.YELLOW, true);

		ModMain.instance.sendToPlayer(new string[]{message});
		return true;
	}

    public string getDescriptionString() {
        return "Shows information about some parts of the mod.";
    }
}

