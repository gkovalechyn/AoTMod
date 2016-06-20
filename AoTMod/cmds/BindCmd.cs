using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//bind <key> <cmd>
//bind k /kill *
public class BindCmd : ICommand{
	private static string[] help = new string[]{
		"/bind - Show help and keybinds.",
		"/bind <key> <command> - binds that key to that command.",
		"/bind -r <key> - Removes that keybind.",
        "/bind list - shows bound commands"
	};

	public bool cmd(string[] args, FengGameManagerMKII gm){
        if (args.Length == 0) {
            ModMain.instance.sendToPlayer(help);
            return true;
        }

        if (args[0].Equals("list", StringComparison.OrdinalIgnoreCase)) {
            ModMain.instance.sendToPlayer("Bound commands: ");
            foreach(KeyValuePair<KeyCode, string> pair in ModMain.instance.getCommandBinder().getBoundCommands()){
                ModMain.instance.sendToPlayer("  [" + pair.Key + "] - " + pair.Value);
            }
            return true;
        } else if (args[0].Equals("-r")) {
            KeyCode keyCode;
            try {
                keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), args[1]);
            } catch (ArgumentException) {
                ModMain.instance.sendToPlayer(args[0] + " is not a key code");
                return true;
            }

            ModMain.instance.getCommandBinder().unbind(keyCode);
            ModMain.instance.sendToPlayer("Command unbound.");
            return true;
        } else {
            KeyCode keyCode;
            string[] temp;
            try {
                keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), args[0]);
            } catch (ArgumentException) {
                ModMain.instance.sendToPlayer(args[0] + " is not a key code");
                return true;
            }

            if (!args[1].StartsWith("/")) {
                ModMain.instance.sendToPlayer("Command does not start with /");
                return true;
            }

            temp = new string[args.Length - 1];
            Array.Copy(args, 1, temp, 0, temp.Length);
            ModMain.instance.getCommandBinder().bind(keyCode, string.Join(" ", temp));
            ModMain.instance.sendToPlayer("Command bound to key: " + keyCode);
            return true;
        }
	}

    public string getDescriptionString() {
        return "Binds a command to a key.";
    }
}

