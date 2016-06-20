using UnityEngine;
using System.Collections;

public class ChatLogCmd : ICommand{
	//cl <on|off>
	private string[] help = {
		"/cl <on|off>",
		"/cl events <on|off>",
		"/cl info"
	};
	public bool cmd(string[] args, FengGameManagerMKII gm){
		if (args.Length == 0){
			ModMain.instance.sendToPlayer(this.help);
			return true;
		}

		if (args[0].Equals("on", System.StringComparison.OrdinalIgnoreCase)){
			ModMain.instance.setChatLogEnabled(true);
			return true;
		}else if (args[0].Equals("off", System.StringComparison.OrdinalIgnoreCase)){
			ModMain.instance.setChatLogEnabled(false);
			return true;
		}else if (args[0].Equals("events", System.StringComparison.OrdinalIgnoreCase)){
			if (args[1].Equals("on", System.StringComparison.OrdinalIgnoreCase)){
				ModMain.instance.writeChatLogEvents = true;
				return true;
			}else if (args[1].Equals("on", System.StringComparison.OrdinalIgnoreCase)){
				ModMain.instance.writeChatLogEvents = false;
				return true;
			}else{
				return false;
			}
		}else if (args[0].Equals("info", System.StringComparison.OrdinalIgnoreCase)){
			ModMain.instance.sendToPlayer("Chat log enabled: " + ModMain.instance.isChatLogEnabled() + "\n" + 
				"Write events to chat log: " + ModMain.instance.writeChatLogEvents);
			return true;
		}
		return false;
	}

    public string getDescriptionString() {
        return "Controls the chat logging functions.";
    }
}

