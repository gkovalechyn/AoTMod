using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class TitanHealthController{
	private int minDamage = 10;
	private bool healthModEnabled = false;

	private float smallModifier = 1f;
	private float mediumModifier = 1f;
	private float largeModifier = 1f;

	private float aberrantModifier = 1f;
	private float normalModifier = 1f;
	private float jumperModifier = 1f;
	private float crawlerModifier = 1f;
	private float punkModifier = 1f;

	private bool customNames = false;
	private bool headExplode = false;
	private bool showDealtDamage = true;
	private int explodeHeadExtraDamage = 1000;

	private bool rcExplode = false;
	private float rcExplosionRadius = 30f;
	private string explosionFX = "FX/Thunder";
	private int explosionDelay = 1000;

	private Dictionary<string, int> specialCases = new Dictionary<string, int>();
	
	public void handleDamage(object titan, object pv, int damage){
		PhotonView playerView = (PhotonView) pv;
		TITAN t = (TITAN) titan;

		if (this.healthModEnabled){
			int health = this.getTitanHealth(t);
			bool dead = damage >= health;

			if (dead){
				if (customNames){
					//colorize the titan's HP based on it's value
					float percentage = ((float)health)/minDamage;
					int hp = 0;
					percentage = percentage > 1f ? 1f : percentage;
					//alternate between RED (max health) and green (lowest health)
					byte r = (byte)((int) 255 * percentage);
					byte g = (byte)((int) 255 * (1f - percentage));

					hp |= ((r << 16) | (g << 8));

					t.name = Colorizer.colorize(health + "HP ", hp.ToString("X6"), false) + t.name;
				}

				if (headExplode && damage > health + explodeHeadExtraDamage && t.abnormalType != AbnormalType.TYPE_CRAWLER){
					t.photonView.RPC("dieHeadBlowRPC", PhotonTargets.All, new object[]{
						playerView.transform.position,
						1f
					});
				}else{
					t.photonView.RPC("netDie", PhotonTargets.AllBuffered, new object[]{});
					if (t.grabbedTarget != null){
						t.grabbedTarget.GetPhotonView().RPC("netUngrabbed", global::PhotonTargets.All, new object[0]);
					}
				}

                t.OnTitanDie(t.photonView); //RC

                ModMain.instance.getGameManager().photonView.RPC("titanGetKill", PhotonNetwork.player, playerView.owner, damage, t.name);

				//SpawnControllerHook
			    ModMain.instance.getSpawnController().onTitanDown(t);

				if (this.rcExplode){
					new Thread(() => this.doExplosion(t.transform.position, this.rcExplosionRadius, explosionDelay)).Start();
				}
			}else if (this.showDealtDamage){
				ModMain.instance.getGameManager().photonView.RPC("netShowDamage", playerView.owner, new object[]{damage});
			}
		}else{
			ModMain.instance.getGameManager().photonView.RPC("titanGetKill", PhotonNetwork.player, playerView.owner, damage, t.name);

            t.OnTitanDie(t.photonView); //RC

			t.photonView.RPC("netDie", PhotonTargets.AllBuffered, new object[]{});
			if (t.grabbedTarget != null){
				t.grabbedTarget.GetPhotonView().RPC("netUngrabbed", global::PhotonTargets.All, new object[0]);
			}

			//SpawnControllerHook
			ModMain.instance.getSpawnController().onTitanDown(t);
		}
		
	}

	public void setEnabled(bool enabled){
		this.healthModEnabled = enabled;
	}

	public bool isEnabled(){
		return this.healthModEnabled;
	}

	public int getMinDamage(){
		return this.minDamage;
	}

	public void setMinDamage(int minDamage){
		this.minDamage = minDamage;
	}

	public void setModifier(Size size, float mod){
		switch(size){
			case Size.SMALL:
				smallModifier = mod;
				break;
			case Size.MEDIUM:
				mediumModifier = mod;
				break;
			case Size.LARGE:
				largeModifier = mod;
				break;
		}
	}

	public void setModifier(AbnormalType type, float mod){
		switch(type){
			case AbnormalType.NORMAL:
				normalModifier = mod;
				break;
			case AbnormalType.TYPE_CRAWLER:
				crawlerModifier = mod;
				break;	
			case AbnormalType.TYPE_I:
				aberrantModifier = mod;
				break;
			case AbnormalType.TYPE_JUMPER:
				jumperModifier = mod;
				break;
			case AbnormalType.TYPE_PUNK:
				punkModifier = mod;
				break;
			default:
				break;
		}
	}

	public float getModifier(AbnormalType type){
		switch(type){
			case AbnormalType.NORMAL:
				return normalModifier;
			case AbnormalType.TYPE_CRAWLER:
				return crawlerModifier;
			case AbnormalType.TYPE_I:
				return aberrantModifier;
			case AbnormalType.TYPE_JUMPER:
				return jumperModifier;
			case AbnormalType.TYPE_PUNK:
				return punkModifier;
			default:
				return 0f;
		}
	}

	public float getModifier(Size size){
		switch(size){
			case Size.SMALL:
				return smallModifier;
			case Size.MEDIUM:
				return mediumModifier;
			case Size.LARGE:
				return largeModifier;
			default:
				return 0f;
		}
	}

	public int getTitanHealth(TITAN titan){
		return this.getTitanHealth(titan.abnormalType, TitanSize.getByScale(titan.transform.localScale));
	}

    public int getTitanHealth(AbnormalType type, float size) {
        return this.getTitanHealth(type, TitanSize.getBySize(size));
    }

	public int getTitanHealth(AbnormalType type, Size size){
		float sizeMod = 1f;
		float typeMod = 1f;
		string name = "" + TitanSize.getRepresentativeChar(size) + TitanSize.getRepresentativeChar(type);
		int health = this.getSpecialCase(name);

		if (health > 0){
			return health;
		}

		switch(size){
			case Size.SMALL:
				sizeMod = smallModifier;
				break;
			case Size.MEDIUM:
				sizeMod = mediumModifier;
				break;
			case Size.LARGE:
				sizeMod = largeModifier;
				break;
			default:
				sizeMod = 1f;
				break;
		}
		
		switch(type){
			case AbnormalType.NORMAL:
				typeMod = normalModifier;
				break;
			case AbnormalType.TYPE_CRAWLER:
				typeMod = crawlerModifier;
				break;	
			case AbnormalType.TYPE_I:
				typeMod = aberrantModifier;
				break;
			case AbnormalType.TYPE_JUMPER:
				typeMod = jumperModifier;
				break;
			case AbnormalType.TYPE_PUNK:
				typeMod = punkModifier;
				break;
		}
		
		return (int)(this.getMinDamage() * sizeMod * typeMod);
	}

	public bool isCustomNamesEnabled(){
		return this.customNames;
	}

	public void setCustomNamesEnabled(bool enabled){
		this.customNames = enabled;
	}

	public bool doHeadsExplode(){
		return this.headExplode;
	}

	public void setHeadsExplode(bool enabled){
		this.headExplode = enabled;
	}

	public int getSpecialCase(string sc){
		int res = -1;
		this.specialCases.TryGetValue(sc, out res);
		return res;
	}

	public void setSpecialCase(string sc, int health){
		this.specialCases[sc] = health;
	}

	public Dictionary<string, int>.KeyCollection getSpecialCases(){
		return this.specialCases.Keys;
	}

	public void removeSpecialCase(string sc){
		this.specialCases.Remove(sc);
	}

	public void setRCExplode(bool val){
		this.rcExplode = val;
	}

	public bool isRCExplodeEnabled(){
		return this.rcExplode;
	}

	public void setRCExplosionRadius(float radius){
		this.rcExplosionRadius = radius;
	}

	public float getRCExplosionRadius(){
		return this.rcExplosionRadius;
	}

	public int getExplosionDelay(){
		return this.explosionDelay;
	}

	public void setExplosionDelay(int mili){
		this.explosionDelay = mili;
	}

	public bool isToShowDamageDealt(){
		return this.showDealtDamage;
	}

	public void setShowDealtDamage(bool val){
		this.showDealtDamage = val;
	}

	private void doExplosion(Vector3 pos, float radius, int delay){
		Thread.Sleep(delay);
		float radiusSquared = radius * radius;

		PhotonNetwork.Instantiate(this.explosionFX, pos, Quaternion.Euler(0, -90, 0), 0);

		foreach(HERO h in GameObject.FindObjectsOfType<HERO>()){
			Vector3 toPlayer = h.transform.position - pos;

			if (toPlayer.sqrMagnitude <= radiusSquared){
				h.photonView.RPC("netDie2", PhotonTargets.All, new object[]{-1, "Explosion"});
			}
		}
	}
}

