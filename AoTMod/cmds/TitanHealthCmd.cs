using UnityEngine;
using System.Collections;

public class TitanHealthCmd : ICommand{
	//th set enabled <on|off>
	//th set titanHealth <amount>
	//th modifier size <l|m|s> <amount>
	//th modifier type <n|a|j|c> <amount>
	private string[] defaultMessages = {
		"/th set enabled <on|off>",
		"/th set titanHealth <amount>",
		"/th modifier size <l|m|s> <value> ... [size] [value]",
		"/th modifier type <n|a|j|c|p> <value> ... [type] [value]",
		"/th info",
		"/th health <.size> <type>",
		"/th customNames <on|off>",
		"/th explodeHeads <on|off>",
		"/th sc [-r] <.size> <type> <health>",
		"/th rcExplode <on|off|radius>",
		"/th rcExplode delay <value>",
		"/th showDamage <on|off>"
	};
	public bool cmd(string[] args, FengGameManagerMKII gm){
		if (args.Length < 1){
			ModMain.instance.sendToPlayer(defaultMessages);
			return true;
		}

		if (args[0].Equals("set", System.StringComparison.OrdinalIgnoreCase)){
			return doSet(args);
		}

		if (args[0].Equals("modifier", System.StringComparison.OrdinalIgnoreCase)){
			return doMod(args);
		}

		if (args[0].Equals("customNames", System.StringComparison.OrdinalIgnoreCase)){
			ModMain.instance.getTHController().setCustomNamesEnabled(args[1].Equals("on", System.StringComparison.OrdinalIgnoreCase));
			return true;
		}

		if (args[0].Equals("explodeHeads", System.StringComparison.OrdinalIgnoreCase)){
			ModMain.instance.getTHController().setHeadsExplode(args[1].Equals("on", System.StringComparison.OrdinalIgnoreCase));
			return true;
		}

		if (args[0].Equals("sc", System.StringComparison.OrdinalIgnoreCase)){
			doSpecialCase(args);
			return true;
		}

		if (args[0].Equals("rcExplode", System.StringComparison.OrdinalIgnoreCase)){
			doRcExplode(args);
			return true;
		}

		if (args[0].Equals("info", System.StringComparison.OrdinalIgnoreCase)){
			string[] messages = new string[]{
				"Titan health: " + Colorizer.colorize("" + ModMain.instance.getTHController().getMinDamage(), Colorizer.Color.YELLOW, true),
				"Custom names: " + ModMain.instance.getTHController().isCustomNamesEnabled(),
				"Heads explode: " + ModMain.instance.getTHController().doHeadsExplode(),
				"Show dealt damage: " + ModMain.instance.getTHController().isToShowDamageDealt(), 
				"Head explode extra damage: 1000",
				"Modifiers: (N|A|J|C|P)",
				Colorizer.colorize(
					ModMain.instance.getTHController().getModifier(AbnormalType.NORMAL)+
				        " " + ModMain.instance.getTHController().getModifier(AbnormalType.TYPE_I)+
				        " " + ModMain.instance.getTHController().getModifier(AbnormalType.TYPE_JUMPER)+
				        " " + ModMain.instance.getTHController().getModifier(AbnormalType.TYPE_CRAWLER) +
						" " + ModMain.instance.getTHController().getModifier(AbnormalType.TYPE_PUNK)
				        , Colorizer.Color.YELLOW, true),
				"Modifiers: (S|M|L)",
				Colorizer.colorize(
					ModMain.instance.getTHController().getModifier(Size.SMALL) + 
					" " + ModMain.instance.getTHController().getModifier(Size.MEDIUM) + 
					" " + ModMain.instance.getTHController().getModifier(Size.LARGE),
					Colorizer.Color.YELLOW, true),
				"Special cases: " + getSpecialCasesString(),
				"RC Explode: " + ModMain.instance.getTHController().isRCExplodeEnabled() + ", Radius: " + ModMain.instance.getTHController().getRCExplosionRadius() + ", Delay: " + ModMain.instance.getTHController().getExplosionDelay(),

			};

			ModMain.instance.sendToPlayer(messages);
			return true;
		}

		if (args[0].Equals("health", System.StringComparison.OrdinalIgnoreCase)){
			if (args.Length < 3){
				ModMain.instance.sendToPlayer("/health dn <.size> <type>");
				return true;
			}

			char size = args[1][0];
			char type = args[2][0];
			int health = ModMain.instance.getTHController().getTitanHealth(TitanSize.getTitanType(type), TitanSize.getByChar(size));

			ModMain.instance.sendToPlayer("Titan health: " + Colorizer.colorize("" + health, Colorizer.Color.YELLOW, true));
			return true;
		}

		if (args[0].Equals("showDamage", System.StringComparison.OrdinalIgnoreCase)){
			ModMain.instance.getTHController().setShowDealtDamage(args[1].Equals("on", System.StringComparison.OrdinalIgnoreCase));
			return true;
		}

		ModMain.instance.sendToPlayer("Argument not recognized: " + args[0]);
		return false;
	}

	private bool doSet(string[] args){
		if (args[1].Equals("enabled", System.StringComparison.OrdinalIgnoreCase)){
			ModMain.instance.getTHController().setEnabled(args[2].Equals("on", System.StringComparison.OrdinalIgnoreCase));
			return true;
		}

		if (args[1].Equals("titanHealth", System.StringComparison.OrdinalIgnoreCase)){
			ModMain.instance.getTHController().setMinDamage(int.Parse(args[2]));
			return true;
		}

		return false;
	}

	private bool doMod(string[] args){
		if (args.Length < 4){
			ModMain.instance.sendToPlayer(new string[]{"/th modifier size <l|m|s> <amount>",
				"/th modifier type <n|a|j|c|p> <amount>"});
			return true;
		}
		
		TitanHealthController thc = ModMain.instance.getTHController();

		if (args[1].Equals("size", System.StringComparison.OrdinalIgnoreCase)){
			for(int i = 2; i < args.Length; i+=2){
				thc.setModifier(TitanSize.getByChar(args[i][0]), float.Parse(args[i + 1]));
			}

			return true;
		}

		if (args[1].Equals("type", System.StringComparison.OrdinalIgnoreCase)){
			for(int i = 2; i < args.Length; i+=2){
				thc.setModifier(TitanSize.getTitanType(args[i][0]), float.Parse(args[i + 1]));
			}

			return true;
		}
	
		return false;
	}

	//th rcExplode <on|off|radius>
	private void doRcExplode(string[] args){
		try{
			float f = float.Parse(args[1]);
			ModMain.instance.getTHController().setRCExplosionRadius(f);
		}catch(System.Exception){
			if (args[1].Equals("delay", System.StringComparison.OrdinalIgnoreCase)){
				ModMain.instance.getTHController().setExplosionDelay(int.Parse(args[2]));
			}else{
				ModMain.instance.getTHController().setRCExplode(args[1].Equals("on", System.StringComparison.OrdinalIgnoreCase));
			}
		}
	}

	//th sc [-r] <size> <type> <value>
	//th sc <size> <type> <value>
	//th sc -r <size> <type>
	private void doSpecialCase(string[] args){
		bool remove = args[1].Equals("-r");
		int index = args[1].StartsWith("-") ? 2 : 1;
		string sc = args[index + 0] + args[index + 1];

		if (remove){
			ModMain.instance.getTHController().removeSpecialCase(sc);
			ModMain.instance.sendToPlayer("Scpecial case removed.");
		}else{
			int health = int.Parse(args[index + 2]);
			ModMain.instance.getTHController().setSpecialCase(sc, health);
			ModMain.instance.sendToPlayer("Scpecial case set.");
		}

	}

	private string getSpecialCasesString(){
		string res = "";
		TitanHealthController th = ModMain.instance.getTHController();

		foreach (string s in th.getSpecialCases()){
			res += s + " " + th.getSpecialCase(s) + "| ";
		}

		return res;
	}

    public string getDescriptionString() {
        return "Controls the TitanHealth part of the mod.";
    }
}

