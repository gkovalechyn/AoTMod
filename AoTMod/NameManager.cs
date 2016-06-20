using UnityEngine;
using System.Collections;
using System.IO;

public class NameManager{
	private string playerName;
	private string playerGuild;
	private string displayName;

	public NameManager(ConfigManager cm){
		this.loadData(cm);
	}

	public string getPlayerName(){
		return this.playerName;
	}

	public string getPlayerGuild(){
		return this.playerGuild;
	}

	public string getPlayerDisplayName(){
		return this.displayName;
	}

	public void setDisplayName(string ndn){
		this.displayName = ndn;
	}

	public void setName(string nn){
		this.playerName = nn;
	}

	public void setGuild(string guild){
		this.playerGuild = guild;
	}

	private void loadData(ConfigManager config){
        this.displayName = config.get("displayName").Replace("\\n", "\n");
        this.playerName = config.get("name").Replace("\\n", "\n");
        this.playerGuild = config.get("guild").Replace("\\n", "\n");
	}
}

