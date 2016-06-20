using UnityEngine;
using System.Collections;

public class OtherPlayerCommandsManager{
	private FengGameManagerMKII gm;

	public OtherPlayerCommandsManager(FengGameManagerMKII gm){
		this.gm = gm;
	}

	public void handleChat(string sender, string message){
		if (!PhotonNetwork.player.isMasterClient){
			return;
		}

		if (message.StartsWith("/thInfo", System.StringComparison.OrdinalIgnoreCase) && ModMain.instance.getTHController().isEnabled()){
			foreach(PhotonPlayer p in PhotonNetwork.otherPlayers){
				if (p.customProperties[PhotonPlayerProperty.name].Equals(sender)){
					sendTHInfo(p);
					break;
				}
			}
		}
	}

	private void sendTHInfo(PhotonPlayer target){
		TitanHealthController thc = ModMain.instance.getTHController();
		ModMain.instance.sendToPlayer(target,
		                              "The titans base health is " + Colorizer.colorize("" + thc.getMinDamage(), Colorizer.Color.YELLOW, true) + "\n" + 
		                              "The titan health is calculated by baseHealth * typeModifier * sizeModifier\n" + 
		                              "Modifiers(Size)(small | medium | large)\n" + 
		                              thc.getModifier(Size.SMALL) + " " + thc.getModifier(Size.MEDIUM) + " " + thc.getModifier(Size.LARGE) + "\n" + 
		                              "Modifiers(type)(normal|aberrant|jumper|crawler|punk)\n" + 
		                              thc.getModifier(AbnormalType.NORMAL) + " " + 
		                              thc.getModifier(AbnormalType.TYPE_I) + " " + 
		                              thc.getModifier(AbnormalType.TYPE_JUMPER) + " " + 
		                              thc.getModifier(AbnormalType.TYPE_CRAWLER) + " " + 
		                              thc.getModifier(AbnormalType.TYPE_PUNK) + "\n" + 
		                              "RC Explosion enabled: " + thc.isRCExplodeEnabled() + ", Radius: " + thc.getRCExplosionRadius() + ", Delay: " + thc.getExplosionDelay()
		                              );
	}
	
}

