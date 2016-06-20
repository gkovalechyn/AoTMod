using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
class AutoCrashCmd : ICommand{
    private string[] help = {
                                "/ac add -s <name>",
                                "/ac add <id>",
                                "/ac remove <index>",
                                "/ac list"
                            };
    public bool cmd(string[] args, FengGameManagerMKII gm) {
        if (args.Length == 0) {
            ModMain.instance.sendToPlayer(this.help);
            return true;
        }

        if (args[0].Equals("add", StringComparison.OrdinalIgnoreCase)) {
            if (args[1].Equals("-s")) {
                string[] temp = new string[args.Length - 2];
                Array.Copy(args, 2, temp, 0, temp.Length);
                ModMain.instance.getModMainThread().addToCrashMap(string.Join(" ", temp), MainModThread.BanType.CONTAINS_PART);
            } else {
                string name = (string) PhotonPlayer.Find(int.Parse(args[1])).customProperties[PhotonPlayerProperty.name];
                ModMain.instance.getModMainThread().addToCrashMap(ModMain.stripColorCodes(name), MainModThread.BanType.FULL_NAME);
            }
            return true;
        } else if (args[0].Equals("remove", StringComparison.OrdinalIgnoreCase)) {
            int i = int.Parse(args[1]);
            Hashtable table = ModMain.instance.getModMainThread().getCrashTable();
            int j = 0;

            foreach (string s in table.Keys) {
                if (i == j) {
                    ModMain.instance.getModMainThread().removeFromCrash(s);
                    return true;
                }
                j++;
            }

            return true;
        } if (args[0].Equals("list", StringComparison.OrdinalIgnoreCase)) {
            Hashtable table = ModMain.instance.getModMainThread().getCrashTable();
            int i = 0;

            foreach(string s in table.Keys){
                ModMain.instance.sendToPlayer("" + i + " - " + s + " [" + table[s] + "]");
                i++;
            }
            return true;
        } else {
            return false;
        }
    }

    public string getDescriptionString() {
        return "Handles the parameters of auto-crashing players.";
    }
}