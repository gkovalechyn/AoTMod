using UnityEngine;
using System;
using System.Collections;
using System.Threading;
//greeter <no args> - show greeter info
//greeter <on|off>
public class GreeterCmd : ICommand{
	public bool cmd(string[] args, FengGameManagerMKII gm){
		if (args.Length == 0){
			ModMain.instance.sendToPlayer(new string[]{
				"Greeter Running: " + ModMain.instance.getGreeter().running,
				"Message: " + ModMain.instance.getGreeter().getMessage(),
				"/greeter <on|off>",
				"/greeter message <message>"
			});
			return true;
		}else{
			if (args[0].Equals("on", System.StringComparison.OrdinalIgnoreCase)){
				if (ModMain.instance.getGreeter().running){
					ModMain.instance.sendToPlayer(new string[]{"The greeter is already running."});
				}else{
					new Thread(new ThreadStart(ModMain.instance.getGreeter().run)).Start();
					ModMain.instance.sendToPlayer(new string[]{"Greeter started."});
				}
				return true;
			}else if (args[0].Equals("off", System.StringComparison.OrdinalIgnoreCase)){
				if (!ModMain.instance.getGreeter().running){
					ModMain.instance.sendToPlayer(new string[]{"The greeter is not running."});
				}else{
					ModMain.instance.getGreeter().toStop = true;
					ModMain.instance.sendToPlayer(new string[]{"Greeter Cancelled."});
				}
				return true;
			}else if(args[0].Equals("message", System.StringComparison.OrdinalIgnoreCase)){
				if (args.Length < 2){
					ModMain.instance.sendToPlayer(new string[]{"/greeter message <message>"});
					return true;
				}else{
					//greeter message arg1 ... argN
					string[] words = new string[args.Length - 1];
					Array.Copy(args, 1, words, 0, words.Length);
					ModMain.instance.getGreeter().setMessage(String.Join(" ", words));
					return true;
				}
			}else{
				return false;
			}
		}
	}

    public string getDescriptionString() {
        return "Controls the greeter(Basically the MOTD, hadn't thought of a better name).";
    }
}

