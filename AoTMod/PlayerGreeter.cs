using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class PlayerGreeter{
	private HashSet<int> currentPlayers = new HashSet<int>();
	public volatile bool toStop = false;
	public bool running = false;
	private string message;

	public PlayerGreeter(ModMain mod){
		message = Colorizer.colorize(mod.getConfig().get("modMessage"), Colorizer.Color.YELLOW, true);
	}

	public void run(){
		running = true;

		while(!toStop && PhotonNetwork.player.isMasterClient){
			HashSet<int> tmp = new HashSet<int>();

            foreach(int i in this.currentPlayers) {
                tmp.Add(i);
            }

			foreach(PhotonPlayer player in PhotonNetwork.playerList){

				if (!currentPlayers.Contains(player.ID)){
					this.sendGreetingMessage(player);
					currentPlayers.Add(player.ID);
				}

				tmp.Remove(player.ID);

			}

			foreach(int i in tmp){
				this.currentPlayers.Remove(i);
			}

			Thread.Sleep(1000);
		}

		this.currentPlayers.Clear();
		this.running = false;
		this.toStop = false;
	}

	private void sendGreetingMessage(PhotonPlayer player){
		ModMain.instance.sendToPlayer(player, this.message);
	}

	public string getMessage(){
		return this.message;
	}

	public void setMessage(string newMessage){
		this.setMessage(newMessage, true);
	}

	public void add(string message, bool newLine){
		//remove the first and last color tags
		//<color=#FFFFFF> = 15
		//</color> = 8
		string temp = message.Substring(0, 15);
		temp = message.Substring(message.Length - 8, 8);

		if (newLine){
			temp += '\n' + message;
		}else{
			if (temp[temp.Length - 1] != '.'){
				temp += '.' + message;
			}else{
				temp += message;
			}
		}
	
		this.setMessage(temp, true);
	}

	public string[] getLines(){
		return this.message.Split('\n');
	}

	public string[] getPhrases(){
		return this.message.Split('.');
	}

	public string removeLine(int index){
		string[] lines = this.message.Split('\n');
		string[] result = new string[lines.Length - 1];

		lines[0] = lines[0].Substring(0, 15);
		lines[lines.Length - 1] = lines[lines.Length - 1].Substring(lines[lines.Length - 1].Length - 8, 8);

		for(int i = 0; i < index; i++){
			result[i] = lines[i];
		}
		for(int i = index + 1; i < lines.Length; i++){
			result[i - 1] = lines[i];
		}

		this.setMessage(string.Join("\n", result), true);

		return lines[index];
	}

	public string removePhrase(int index){
		string[] phrases = this.message.Split('.');
		string[] result = new string[phrases.Length - 1];

		phrases[0] = phrases[0].Substring(0, 15);
		phrases[phrases.Length - 1] = phrases[phrases.Length - 1].Substring(phrases[phrases.Length - 1].Length - 8, 8);
		
		for(int i = 0; i < index; i++){
			result[i] = phrases[i];
		}
		for(int i = index + 1; i < phrases.Length; i++){
			result[i - 1] = phrases[i];
		}
		
		this.setMessage(string.Join("\n", result), true);

		return phrases[index];
	}

	public void resend(){
		lock(this.currentPlayers){
			this.currentPlayers.Clear();
		}
	}

	public void setMessage(string newMessage, bool colorize){
		if (colorize){
			lock(this.message){
				this.message = Colorizer.colorize(newMessage, Colorizer.Color.YELLOW, true);
			}
		}else{
			lock(this.message){
				this.message = newMessage;
			}
		}
	}
}

