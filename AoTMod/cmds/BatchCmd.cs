using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
class BatchCmd : ICommand{
    private string[] help = {
                                "/batch <file1> ... [fileN]"
                            };
    public bool cmd(string[] args, FengGameManagerMKII gm) {
        if (args.Length == 0) {
            ModMain.instance.sendToPlayer(this.help);
            return true;
        }
        CommandManager cm = ModMain.instance.getCommandManager();

        foreach (string s in args) {
            if (!File.Exists(s)) {
                ModMain.instance.sendToPlayer("The file \"" + s + "\" does not exist.");
            } else {
                StreamReader reader = new StreamReader( File.OpenRead(s));
                string temp;

                while (!string.IsNullOrEmpty((temp = reader.ReadLine()))) {
                    cm.parseCommand(temp, gm);
                }
            }
        }

        return true;
    }

    public string getDescriptionString() {
        return "Runs commands from a file.";
    }
}
