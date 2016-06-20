using UnityEngine;
using System.Reflection;
using ExitGames.Client.Photon;

public class PersonaCmd : ICommand{
	//persona <1-3>
	public bool cmd(string[] args, FengGameManagerMKII gm){
		if (args.Length == 0){
			ModMain.instance.sendToPlayer("/persona <1-N>");
			return true;
		}
        Hashtable table = new Hashtable();
		int i = int.Parse(args[0]);

		if (i < 1 || i > 3){
			return true;
		}

		string name = ModMain.instance.getConfig().get("persona" + i);
		string displayName = ModMain.instance.getConfig().get("persona" + i + "DN");
		string guild = ModMain.instance.getConfig().get("persona" + i + "Guild");
        string set = ModMain.instance.getConfig().get("persona" + i + "Set");

        if (int.Parse(set) > 0) {
            IN_GAME_MAIN_CAMERA.singleCharacter = "SET " + set;
            updateCostume(i);
        }

		ModMain.instance.getNameManager().setDisplayName(displayName);
		ModMain.instance.getNameManager().setName(name);
		ModMain.instance.getNameManager().setGuild(guild);

		LoginFengKAI.player.name = displayName;
        LoginFengKAI.player.guildname = guild;

		table[PhotonPlayerProperty.name] = displayName;
		table[PhotonPlayerProperty.guildName] = guild;

        PhotonNetwork.player.SetCustomProperties(table);

		setHeroField(gm, i);

        ModMain.instance.getModMainThread().updateInternalPlayerProperties();

		return true;
	}

	private void setHeroField(FengGameManagerMKII gm, int i){
		FieldInfo lastCharacterField = gm.GetType().GetField("myLastHero", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase);

		lastCharacterField.SetValue(gm, "SET " + i);

	}

	private void updateCostume(int id){
		IN_GAME_MAIN_CAMERA component = GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>();
		HeroCostume costume2 = CostumeConeveter.LocalDataToHeroCostume("SET " + id);
		component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().init();

		if (costume2 != null){
			component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = costume2;
			component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = costume2.stat;
		}else{
			costume2 = HeroCostume.costumeOption[3];
			component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume = costume2;
			component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().myCostume.stat = HeroStat.getInfo(costume2.name.ToUpper());
		}

		component.main_object.GetComponent<HERO>().GetComponent<HERO_SETUP>().setCharacterComponent();
		component.main_object.GetComponent<HERO>().setStat();
		component.main_object.GetComponent<HERO>().setSkillHUDPosition();
	}

    public string getDescriptionString() {
        return "Switches persona. (Persona1...N in the config)";
    }
}

