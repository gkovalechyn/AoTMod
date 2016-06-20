using UnityEngine;
using System.Collections;
using System.Reflection;

public class GravityCmd : ICommand{
	private float originalGravity = -1f;
	public bool cmd(string[] args, FengGameManagerMKII gm){
		HERO hero = null;
		int playerId = PhotonNetwork.player.ID;

		foreach(HERO h in Object.FindObjectsOfType<HERO>()){
			if (h.photonView.ownerId == playerId){
				hero = h;
				break;
			}
		}

		FieldInfo gravityField = hero.GetType().GetField("gravity", BindingFlags.Instance | BindingFlags.NonPublic);

		if (this.originalGravity < 0f){
			this.originalGravity = (float) gravityField.GetValue(hero);
		}

		if (args[0].Equals("on", System.StringComparison.OrdinalIgnoreCase)){
			gravityField.SetValue(hero, originalGravity);
		}else{
			gravityField.SetValue(hero, 0f);
		}

		return true;
	}

    public string getDescriptionString() {
        return "Sets the gravity for your character.";
    }
}

