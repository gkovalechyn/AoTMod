using UnityEngine;
using System.Collections.Generic;

public class HelpCmd : ICommand{
    private string[] commands;
    private int commandPages;
    private string[] rcCommandList = {
                                         "/cloth , /pm",
                                         "/aso <kdr|racing>",
                                         "/pause, /unpause",
                                         "/checklevel",
                                         "/isrc, /ignorelist",
                                         "/RCroom <max|time>",
                                         "/resetkd, /resetkdall",
                                         "/RCreset, /detect",
                                         "/RCrestart, /specmode",
                                         "/fov <value>, /spectate",
                                         "/revive <id>, /reviveall",
                                         "/RCban <id>, /RCunban <id>, /RCbanlist",
                                         "/rules, /RCkick"
                                     };
    public void buildHelpCommands(ICollection<string> commands) {
        List<string> customColors = ModMain.instance.getConfig().getStringList("HelpColors");
        Colorizer.Color[] colors = new Colorizer.Color[customColors.Count];
        CommandManager cm = ModMain.instance.getCommandManager();
        int i = 0;

        foreach (string s in customColors) {
            colors[i++] = new Colorizer.Color(s);
        }
        if (colors.Length == 0) {
            colors = new Colorizer.Color[]{
                Colorizer.Color.GREEN,
                Colorizer.Color.YELLOW,
            };
        }
        i = 0;
        this.commands = new string[commands.Count];

        foreach (string s in commands) {
            ICommand cmd = cm.getCommand(s);
            this.commands[i] = "/" + s + " - " + cmd.getDescriptionString();
            i++;
        }

        Colorizer.colorize(this.commands, colors, true);
        this.commandPages = Mathf.CeilToInt(this.commands.Length / 11F);
    }

	public bool cmd(string[] args, FengGameManagerMKII gm){
		if (args.Length < 1){
			ModMain.instance.sendToPlayer("/help <1-" + this.commandPages + " | RC>");
			return true;
		}
        if (args[0].Equals("RC", System.StringComparison.OrdinalIgnoreCase)) {
            ModMain.instance.sendToPlayer(this.rcCommandList);
        } else {
            int commandPage = int.Parse(args[0]);
            if (commandPage < 1 || commandPage > this.commandPages) {
                ModMain.instance.sendToPlayer("/help <1-" + this.commandPages + ">");
            } else {
                int from = (commandPage - 1) * 11;
                int to = commandPage * 11;
                to = (to > this.commands.Length) ? this.commands.Length : to;

                for (int i = from; i < to; i++) {
                    ModMain.instance.sendToPlayer(this.commands[i]);
                }
            }
        }
		return  true;
	}
    public string getDescriptionString() {
        return "Lists commands and shit, u kno?";
    }
}

