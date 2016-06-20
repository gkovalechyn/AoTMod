using ExitGames.Client.Photon;
using Photon;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InRoomChat : Photon.MonoBehaviour {
    private bool AlignBottom = true;
    public static readonly string ChatRPC = "Chat";
    public static Rect GuiRect = new Rect(0f, 100f, 300f, 470f);
    public static Rect GuiRect2 = new Rect(30f, 575f, 300f, 25f);
    private string inputLine = string.Empty;
    public bool IsVisible = true;
    public static List<string> messages = new List<string>();
    private Vector2 scrollPos = Vector2.zero;
    public static int chatSize = 20;

    private bool isTyping = false;
    private Vector2 chatScroll = Vector2.zero;

    public static int numMessagesToBeDisplayed = 16;
    private int modCommandIndex = 0;

    public void addLINE(string newLine) {
        messages.Add(newLine);

        if (messages.Count > InRoomChat.numMessagesToBeDisplayed) {
            messages.RemoveRange(0, messages.Count - numMessagesToBeDisplayed);
        }
    }

    public void AddLine(string newLine) {
        this.addLINE(newLine);
    }


    private void modOnGUI() {
        if (this.IsVisible && global::PhotonNetwork.connectionStatesDetailed == PeerStates.Joined) {
            if (Event.current.type == EventType.KeyDown) {
                if (Event.current.keyCode == KeyCode.Escape && this.isTyping) {
                    this.inputLine = string.Empty;
                    GUI.FocusControl(string.Empty);
                    this.isTyping = false;
                }else if ((Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return) && !Input.GetKey(KeyCode.LeftShift)) {
                    if (this.isTyping) {
                        if (!string.IsNullOrEmpty(this.inputLine)) {
                            AoTModAPI.AoTModAPI.getModAPI().parseChat(this.inputLine);
                            this.modCommandIndex = 0;
                            this.inputLine = string.Empty;
                            GUI.FocusControl(string.Empty);
                        }

                        this.isTyping = false;
                        GUI.FocusControl(string.Empty);
                    } else {
                        this.isTyping = true;
                        GUI.FocusControl("ChatInput");
                    }
                    
                } else if (Event.current.keyCode == KeyCode.UpArrow) {
                    this.modCommandIndex++;
                    if (this.modCommandIndex > 16) {
                        this.modCommandIndex = 16;
                    }
                    this.inputLine = AoTModAPI.AoTModAPI.getModAPI().getCommandInHistory(this.modCommandIndex);
                } else if (Event.current.keyCode == KeyCode.DownArrow) {
                    this.modCommandIndex--;
                    if (this.modCommandIndex < 0) {
                        this.modCommandIndex = 0;
                    }
                    this.inputLine = AoTModAPI.AoTModAPI.getModAPI().getCommandInHistory(this.modCommandIndex);
                }
            }

            GUI.SetNextControlName(string.Empty);
            GUILayout.BeginArea(global::InRoomChat.GuiRect);
            GUILayout.FlexibleSpace();
            string text = string.Empty;

            if (global::InRoomChat.messages.Count < global::InRoomChat.numMessagesToBeDisplayed) {
                for (int i = 0; i < global::InRoomChat.messages.Count; i++) {
                    text = text + global::InRoomChat.messages[i] + "\n";
                }
            } else {
                for (int j = global::InRoomChat.messages.Count - global::InRoomChat.numMessagesToBeDisplayed; j < global::InRoomChat.messages.Count; j++) {
                    text = text + global::InRoomChat.messages[j] + "\n";
                }
            }
            GUILayout.Label(text, new GUILayoutOption[0]);
            GUILayout.EndArea();
            GUILayout.BeginArea(global::InRoomChat.GuiRect2);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUI.SetNextControlName("ChatInput");
            this.inputLine = GUILayout.TextField(this.inputLine, 999, new GUILayoutOption[]{
				GUILayout.MaxWidth(global::InRoomChat.GuiRect2.width)
			});
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
    public void OnGUI() {
		//Fuck the shitty code that was here before.
        this.modOnGUI();
        /*
        int num4;
        if (!this.IsVisible || (PhotonNetwork.connectionStatesDetailed != PeerStates.Joined)) {
            return;
        }
        if (Event.current.type == EventType.KeyDown) {
            if ((((Event.current.keyCode != KeyCode.Tab) && (Event.current.character != '\t')) || IN_GAME_MAIN_CAMERA.isPausing) || (FengGameManagerMKII.inputRC.humanKeys[InputCodeRC.chat] == KeyCode.Tab)) {
                goto Label_00E1;
            }
            Event.current.Use();
            goto Label_013D;
        }
        if ((Event.current.type == EventType.KeyUp) && (((Event.current.keyCode != KeyCode.None) && (Event.current.keyCode == FengGameManagerMKII.inputRC.humanKeys[InputCodeRC.chat])) && (GUI.GetNameOfFocusedControl() != "ChatInput"))) {
            this.inputLine = string.Empty;
            this.modCommandIndex = 0;
            GUI.FocusControl("ChatInput");
            goto Label_013D;
        }
Label_00E1:
        if (Event.current.type == EventType.KeyDown) {
            if (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return) {
                if (!string.IsNullOrEmpty(this.inputLine)) {
                    string str2;

                    if (this.inputLine == "\t") {
                        this.inputLine = string.Empty;
                        GUI.FocusControl(string.Empty);
                        return;
                    }

                    this.modCommandIndex = 0;
                    if (FengGameManagerMKII.RCEvents.ContainsKey("OnChatInput")) { //Wut?
                        string key = (string)FengGameManagerMKII.RCVariableNames["OnChatInput"];
                        if (FengGameManagerMKII.stringVariables.ContainsKey(key)) {
                            FengGameManagerMKII.stringVariables[key] = this.inputLine;
                        } else {
                            FengGameManagerMKII.stringVariables.Add(key, this.inputLine);
                        }
                        ((RCEvent)FengGameManagerMKII.RCEvents["OnChatInput"]).checkEvent();
                    }

                    Don't give a fuck about RC's chat
                    if (!this.inputLine.StartsWith("/")) {
                        str2 = RCextensions.returnStringFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]).hexColor();
                        if (str2 == string.Empty) {
                            str2 = RCextensions.returnStringFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]);
                            if (PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCteam] != null) {
                                if (RCextensions.returnIntFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCteam]) == 1) {
                                    str2 = "<color=#00FFFF>" + str2 + "</color>";
                                } else if (RCextensions.returnIntFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCteam]) == 2) {
                                    str2 = "<color=#FF00FF>" + str2 + "</color>";
                                }
                            }
                        }

                        str2 = "<i>" + str2 + "</i>";
                        object[] parameters = new object[] { this.inputLine, str2 };
                        FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, parameters);
                    } else 
                        

                    if (this.inputLine.StartsWith("/")) {
                        
                } else {
                    this.inputLine = "\t";
                    GUI.FocusControl("ChatInput");
                }
            } else if (Event.current.keyCode == KeyCode.UpArrow) {
                this.modCommandIndex++;

                if (this.modCommandIndex > 16) {
                    this.modCommandIndex = 16;
                }

                this.inputLine = global::ModMain.instance.getCommandManager().getCommandInHistory(this.modCommandIndex);
            } else if (Event.current.keyCode == KeyCode.DownArrow) {
                this.modCommandIndex--;

                if (this.modCommandIndex < 0) {
                    this.modCommandIndex = 0;
                }

                this.inputLine = global::ModMain.instance.getCommandManager().getCommandInHistory(this.modCommandIndex);
            }

            this.inputLine = "\t";
            GUI.FocusControl("ChatInput");
        }
Label_013D:
        GUI.SetNextControlName(string.Empty);
        GUILayout.BeginArea(GuiRect);
        GUILayout.FlexibleSpace();
        string text = string.Empty;
        if (messages.Count < chatSize) {
            for (num4 = 0; num4 < messages.Count; num4++) {
                text = text + messages[num4] + "\n";
            }
        } else {
            for (int i = messages.Count - chatSize; i < messages.Count; i++) {
                text = text + messages[i] + "\n";
            }
        }
        GUILayout.Label(text, new GUILayoutOption[0]);
        GUILayout.EndArea();
        GUILayout.BeginArea(GuiRect2);
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        GUI.SetNextControlName("ChatInput");
        this.inputLine = GUILayout.TextField(this.inputLine, 999, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(global::InRoomChat.GuiRect2.width)
			});
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
         * */
    }

    public void setPosition() {
        if (this.AlignBottom) {
            GuiRect = new Rect(0f, (float)(Screen.height - 500), 300f, 470f);
            GuiRect2 = new Rect(30f, (float)((Screen.height - 300) + 0x113), 300f, 25f);
        }
    }

    private void writeRules() {
        this.addLINE("<color=#FFCC00>Currently activated gamemodes:</color>");
        if (RCSettings.bombMode > 0) {
            this.addLINE("<color=#FFCC00>Bomb mode is on.</color>");
        }
        if (RCSettings.teamMode > 0) {
            if (RCSettings.teamMode == 1) {
                this.addLINE("<color=#FFCC00>Team mode is on (no sort).</color>");
            } else if (RCSettings.teamMode == 2) {
                this.addLINE("<color=#FFCC00>Team mode is on (sort by size).</color>");
            } else if (RCSettings.teamMode == 3) {
                this.addLINE("<color=#FFCC00>Team mode is on (sort by skill).</color>");
            }
        }
        if (RCSettings.pointMode > 0) {
            this.addLINE("<color=#FFCC00>Point mode is on (" + Convert.ToString(RCSettings.pointMode) + ").</color>");
        }
        if (RCSettings.disableRock > 0) {
            this.addLINE("<color=#FFCC00>Punk Rock-Throwing is disabled.</color>");
        }
        if (RCSettings.spawnMode > 0) {
            this.addLINE("<color=#FFCC00>Custom spawn rate is on (" + RCSettings.nRate.ToString("F2") + "% Normal, " + RCSettings.aRate.ToString("F2") + "% Abnormal, " + RCSettings.jRate.ToString("F2") + "% Jumper, " + RCSettings.cRate.ToString("F2") + "% Crawler, " + RCSettings.pRate.ToString("F2") + "% Punk </color>");
        }
        if (RCSettings.explodeMode > 0) {
            this.addLINE("<color=#FFCC00>Titan explode mode is on (" + Convert.ToString(RCSettings.explodeMode) + ").</color>");
        }
        if (RCSettings.healthMode > 0) {
            this.addLINE("<color=#FFCC00>Titan health mode is on (" + Convert.ToString(RCSettings.healthLower) + "-" + Convert.ToString(RCSettings.healthUpper) + ").</color>");
        }
        if (RCSettings.infectionMode > 0) {
            this.addLINE("<color=#FFCC00>Infection mode is on (" + Convert.ToString(RCSettings.infectionMode) + ").</color>");
        }
        if (RCSettings.damageMode > 0) {
            this.addLINE("<color=#FFCC00>Minimum nape damage is on (" + Convert.ToString(RCSettings.damageMode) + ").</color>");
        }
        if (RCSettings.moreTitans > 0) {
            this.addLINE("<color=#FFCC00>Custom titan # is on (" + Convert.ToString(RCSettings.moreTitans) + ").</color>");
        }
        if (RCSettings.sizeMode > 0) {
            this.addLINE("<color=#FFCC00>Custom titan size is on (" + RCSettings.sizeLower.ToString("F2") + "," + RCSettings.sizeUpper.ToString("F2") + ").</color>");
        }
        if (RCSettings.banEren > 0) {
            this.addLINE("<color=#FFCC00>Anti-Eren is on. Using Titan eren will get you kicked.</color>");
        }
        if (RCSettings.waveModeOn == 1) {
            this.addLINE("<color=#FFCC00>Custom wave mode is on (" + Convert.ToString(RCSettings.waveModeNum) + ").</color>");
        }
        if (RCSettings.friendlyMode > 0) {
            this.addLINE("<color=#FFCC00>Friendly-Fire disabled. PVP is prohibited.</color>");
        }
        if (RCSettings.pvpMode > 0) {
            if (RCSettings.pvpMode == 1) {
                this.addLINE("<color=#FFCC00>AHSS/Blade PVP is on (team-based).</color>");
            } else if (RCSettings.pvpMode == 2) {
                this.addLINE("<color=#FFCC00>AHSS/Blade PVP is on (FFA).</color>");
            }
        }
        if (RCSettings.maxWave > 0) {
            this.addLINE("<color=#FFCC00>Max Wave set to " + RCSettings.maxWave.ToString() + "</color>");
        }
        if (RCSettings.horseMode > 0) {
            this.addLINE("<color=#FFCC00>Horses are enabled.</color>");
        }
        if (RCSettings.ahssReload > 0) {
            this.addLINE("<color=#FFCC00>AHSS Air-Reload disabled.</color>");
        }
        if (RCSettings.punkWaves > 0) {
            this.addLINE("<color=#FFCC00>Punk override every 5 waves enabled.</color>");
        }
        if (RCSettings.endlessMode > 0) {
            this.addLINE("<color=#FFCC00>Endless Respawn is enabled (" + RCSettings.endlessMode.ToString() + " seconds).</color>");
        }
        if (RCSettings.globalDisableMinimap > 0) {
            this.addLINE("<color=#FFCC00>Minimaps are disabled.</color>");
        }
        if (RCSettings.motd != string.Empty) {
            this.addLINE("<color=#FFCC00>MOTD:" + RCSettings.motd + "</color>");
        }
        if (RCSettings.deadlyCannons > 0) {
            this.addLINE("<color=#FFCC00>Cannons will kill humans.</color>");
        }
    }

    private void doTeamCommand() {
        if (RCSettings.teamMode == 1) {
            if ((this.inputLine.Substring(6) == "1") || (this.inputLine.Substring(6) == "cyan")) {
                FengGameManagerMKII.instance.photonView.RPC("setTeamRPC", PhotonNetwork.player, new object[] { 1 });
                this.addLINE("<color=#00FFFF>You have joined team cyan.</color>");
                foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("Player")) {
                    if (obj2.GetPhotonView().isMine) {
                        obj2.GetComponent<HERO>().markDie();
                        obj2.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, new object[] { -1, "Team Switch" });
                    }
                }
            } else if ((this.inputLine.Substring(6) == "2") || (this.inputLine.Substring(6) == "magenta")) {
                FengGameManagerMKII.instance.photonView.RPC("setTeamRPC", PhotonNetwork.player, new object[] { 2 });
                this.addLINE("<color=#FF00FF>You have joined team magenta.</color>");
                foreach (GameObject obj3 in GameObject.FindGameObjectsWithTag("Player")) {
                    if (obj3.GetPhotonView().isMine) {
                        obj3.GetComponent<HERO>().markDie();
                        obj3.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, new object[] { -1, "Team Switch" });
                    }
                }
            } else if ((this.inputLine.Substring(6) == "0") || (this.inputLine.Substring(6) == "individual")) {
                FengGameManagerMKII.instance.photonView.RPC("setTeamRPC", PhotonNetwork.player, new object[] { 0 });
                this.addLINE("<color=#00FF00>You have joined individuals.</color>");
                foreach (GameObject obj4 in GameObject.FindGameObjectsWithTag("Player")) {
                    if (obj4.GetPhotonView().isMine) {
                        obj4.GetComponent<HERO>().markDie();
                        obj4.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, new object[] { -1, "Team Switch" });
                    }
                }
            } else {
                this.addLINE("<color=#FFCC00>error: invalid team code. Accepted values are 0,1, and 2.</color>");
            }
        } else {
            this.addLINE("<color=#FFCC00>error: teams are locked or disabled. </color>");
        }
    }


    private void doResetCommand() {
        Hashtable hashtable;
        if (this.inputLine == "/resetkdall") {
            foreach (PhotonPlayer player in PhotonNetwork.playerList) {
                hashtable = new Hashtable();
                hashtable.Add(PhotonPlayerProperty.kills, 0);
                hashtable.Add(PhotonPlayerProperty.deaths, 0);
                hashtable.Add(PhotonPlayerProperty.max_dmg, 0);
                hashtable.Add(PhotonPlayerProperty.total_dmg, 0);
                player.SetCustomProperties(hashtable);
            }
            if (PhotonNetwork.isMasterClient) {
                FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, "<color=#FFCC00>All stats have been reset.</color>", "");
            }
        } else {
            hashtable = new Hashtable();
            hashtable.Add(PhotonPlayerProperty.kills, 0);
            hashtable.Add(PhotonPlayerProperty.deaths, 0);
            hashtable.Add(PhotonPlayerProperty.max_dmg, 0);
            hashtable.Add(PhotonPlayerProperty.total_dmg, 0);
            PhotonNetwork.player.SetCustomProperties(hashtable);
            this.addLINE("<color=#FFCC00>Your stats have been reset. </color>");
        }
    }
    public void Start() {
        this.setPosition();
    }
}

/*RC shit code
 * if (this.inputLine == "/cloth") {
                            this.addLINE(ClothFactory.GetDebugInfo());
                        } else if (this.inputLine.StartsWith("/aso")) {
                            if (PhotonNetwork.isMasterClient) {
                                switch (this.inputLine.Substring(5)) {
                                    case "kdr":
                                        if (RCSettings.asoPreservekdr == 0) {
                                            RCSettings.asoPreservekdr = 1;
                                            this.addLINE("<color=#FFCC00>KDRs will be preserved from disconnects.</color>");
                                        } else {
                                            RCSettings.asoPreservekdr = 0;
                                            this.addLINE("<color=#FFCC00>KDRs will not be preserved from disconnects.</color>");
                                        }
                                        break;

                                    case "racing":
                                        if (RCSettings.racingStatic == 0) {
                                            RCSettings.racingStatic = 1;
                                            this.addLINE("<color=#FFCC00>Racing will not end on finish.</color>");
                                        } else {
                                            RCSettings.racingStatic = 0;
                                            this.addLINE("<color=#FFCC00>Racing will end on finish.</color>");
                                        }
                                        break;
                                }
                            }
                        } else if (this.inputLine == "/pause") {

                            if (PhotonNetwork.isMasterClient) {
                                FengGameManagerMKII.instance.photonView.RPC("pauseRPC", PhotonTargets.All, true);
                                FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, "<color=#FFCC00>MasterClient has paused the game.</color>", "");
                            } else {
                                this.addLINE("<color=#FFCC00>error: not master client</color>");
                            }
                        } else if (this.inputLine == "/unpause") {
                            if (PhotonNetwork.isMasterClient) {
                                FengGameManagerMKII.instance.photonView.RPC("pauseRPC", PhotonTargets.All, false);
                                FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, "<color=#FFCC00>MasterClient has unpaused the game.</color>", "");
                            } else {
                                this.addLINE("<color=#FFCC00>error: not master client</color>");
                            }
                        } else if (this.inputLine == "/checklevel") {
                            foreach (PhotonPlayer player in PhotonNetwork.playerList) {
                                this.addLINE(RCextensions.returnStringFromObject(player.customProperties[PhotonPlayerProperty.currentLevel]));
                            }
                        } else if (this.inputLine == "/isrc") {
                            if (FengGameManagerMKII.masterRC) {
                                this.addLINE("is RC");
                            } else {
                                this.addLINE("not RC");
                            }
                        } else if (this.inputLine == "/ignorelist") {
                            foreach (int num2 in FengGameManagerMKII.ignoreList) {
                                this.addLINE(num2.ToString());
                            }
                        } else if (this.inputLine.StartsWith("/RCroom")) {
                            if (PhotonNetwork.isMasterClient) {
                                if (this.inputLine.Substring(6).StartsWith("max")) {
                                    int num3 = Convert.ToInt32(this.inputLine.Substring(12));
                                    FengGameManagerMKII.instance.maxPlayers = num3;
                                    PhotonNetwork.room.maxPlayers = num3;
                                    FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, "<color=#FFCC00>Max players changed to " + this.inputLine.Substring(12) + "!</color>", "");
                                } else if (this.inputLine.Substring(6).StartsWith("time")) {
                                    FengGameManagerMKII.instance.addTime(Convert.ToSingle(this.inputLine.Substring(11)));
                                    FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, "<color=#FFCC00>" + this.inputLine.Substring(13) + " seconds added to the clock.</color>", "");
                                }
                            } else {
                                this.addLINE("<color=#FFCC00>error: not master client</color>");
                            }
                        } else if (this.inputLine.StartsWith("/resetkd")) {
                            this.doResetCommand();
                        } else if (this.inputLine.StartsWith("/RCreset")) {
                            this.addLINE("Last run: ");
                            this.addLINE(PhotonNetwork.player.customProperties[PhotonPlayerProperty.max_dmg] + "/" + PhotonNetwork.player.customProperties[PhotonPlayerProperty.total_dmg] + "/" + PhotonNetwork.player.customProperties[PhotonPlayerProperty.kills] + "/" + GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().roundTime + "s");
                            Hashtable hashtable;
                            hashtable = new Hashtable();
                            hashtable.Add(PhotonPlayerProperty.kills, 0);
                            hashtable.Add(PhotonPlayerProperty.deaths, 0);
                            hashtable.Add(PhotonPlayerProperty.max_dmg, 0);
                            hashtable.Add(PhotonPlayerProperty.total_dmg, 0);
                            PhotonNetwork.player.SetCustomProperties(hashtable);
                            FengGameManagerMKII.instance.addTime(600);
                            FengGameManagerMKII.instance.restartRC();
                            this.addLINE("Have a nice run.");
                        } else if (this.inputLine.StartsWith("/detect")) {
                            PhotonPlayer player = PhotonPlayer.Find(Convert.ToInt32(this.inputLine.Remove(0, 8)));
                            ExitGames.Client.Photon.Hashtable hisProperties = player.customProperties;
                            ExitGames.Client.Photon.Hashtable myProperties = PhotonNetwork.player.customProperties;
                            string istring;
                            string pstring = "null";
                            foreach (string s in hisProperties.Keys) {
                                if (!myProperties.ContainsKey(s)) {
                                    istring = "<color=yellow>ID: [" + player.ID + "] has a hidden property: " + s + "</color>";
                                    if (istring != pstring) {
                                        this.addLINE(istring);
                                        pstring = istring;
                                    }
                                } else {
                                    istring = "<color=yellow>ID: [" + player.ID + "] has no hidden property.</color>";
                                    if (istring != pstring) {
                                        this.addLINE(istring);
                                        pstring = istring;
                                    }
                                }
                            }
                        } else if (this.inputLine.StartsWith("/pm")) {
                            string[] strArray = this.inputLine.Split(new char[] { ' ' });
                            PhotonPlayer targetPlayer = PhotonPlayer.Find(Convert.ToInt32(strArray[1]));
                            string str2 = RCextensions.returnStringFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]).hexColor();
                            if (str2 == string.Empty) {
                                str2 = RCextensions.returnStringFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]);
                                if (PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCteam] != null) {
                                    if (RCextensions.returnIntFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCteam]) == 1) {
                                        str2 = "<color=#00FFFF>" + str2 + "</color>";
                                    } else if (RCextensions.returnIntFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCteam]) == 2) {
                                        str2 = "<color=#FF00FF>" + str2 + "</color>";
                                    }
                                }
                            }
                            string str3 = RCextensions.returnStringFromObject(targetPlayer.customProperties[PhotonPlayerProperty.name]).hexColor();
                            if (str3 == string.Empty) {
                                str3 = RCextensions.returnStringFromObject(targetPlayer.customProperties[PhotonPlayerProperty.name]);
                                if (targetPlayer.customProperties[PhotonPlayerProperty.RCteam] != null) {
                                    if (RCextensions.returnIntFromObject(targetPlayer.customProperties[PhotonPlayerProperty.RCteam]) == 1) {
                                        str3 = "<color=#00FFFF>" + str3 + "</color>";
                                    } else if (RCextensions.returnIntFromObject(targetPlayer.customProperties[PhotonPlayerProperty.RCteam]) == 2) {
                                        str3 = "<color=#FF00FF>" + str3 + "</color>";
                                    }
                                }
                            }
                            string str4 = string.Empty;
                            for (int num4 = 2; num4 < strArray.Length; num4++) {
                                str4 = str4 + strArray[num4] + " ";
                            }
                            FengGameManagerMKII.instance.photonView.RPC("ChatPM", targetPlayer, new object[] { str2, str4 });
                            this.addLINE("<color=#FFC000>TO [" + targetPlayer.ID.ToString() + "]</color> " + str3 + ":" + str4);
                        } else if (this.inputLine.StartsWith("/team")) {
                            this.doTeamCommand();
                        } else if (this.inputLine == "/RCrestart") {
                            if (PhotonNetwork.isMasterClient) {
                                FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, "<color=#FFCC00>MasterClient has restarted the game!</color>", "");
                                FengGameManagerMKII.instance.restartRC();
                            } else {
                                this.addLINE("<color=#FFCC00>error: not master client</color>");
                            }
                        } else if (this.inputLine.StartsWith("/specmode")) {
                            if (((int)FengGameManagerMKII.settings[0xf5]) == 0) {
                                FengGameManagerMKII.settings[0xf5] = 1;
                                FengGameManagerMKII.instance.EnterSpecMode(true);
                                this.addLINE("<color=#FFCC00>You have entered spectator mode.</color>");
                            } else {
                                FengGameManagerMKII.settings[0xf5] = 0;
                                FengGameManagerMKII.instance.EnterSpecMode(false);
                                this.addLINE("<color=#FFCC00>You have exited spectator mode.</color>");
                            }
                        } else if (this.inputLine.StartsWith("/fov")) {
                            int num6 = Convert.ToInt32(this.inputLine.Substring(5));
                            Camera.main.fieldOfView = num6;
                            this.addLINE("<color=#FFCC00>Field of vision set to " + num6.ToString() + ".</color>");
                        } else if (this.inputLine == "/colliders") {
                            int num7 = 0;
                            foreach (TITAN titan in FengGameManagerMKII.instance.getTitans()) {
                                if (titan.myTitanTrigger.isCollide) {
                                    num7++;
                                }
                            }
                            FengGameManagerMKII.instance.chatRoom.addLINE(num7.ToString());
                        } else if (this.inputLine.StartsWith("/spectate")) {
                            int num8;

                            num8 = Convert.ToInt32(this.inputLine.Substring(10));
                            foreach (GameObject obj5 in GameObject.FindGameObjectsWithTag("Player")) {
                                if (obj5.GetPhotonView().owner.ID == num8) {
                                    Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(obj5, true, false);
                                    Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(false);
                                }
                            }
                            object[] objArray5;
                            if (this.inputLine.StartsWith("/revive")) {
                                if (PhotonNetwork.isMasterClient) {
                                    if (this.inputLine == "/reviveall") {
                                        objArray5 = new object[] { "<color=#FFCC00>All players have been revived.</color>", string.Empty };
                                        FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, objArray5);
                                        foreach (PhotonPlayer player in PhotonNetwork.playerList) {
                                            if (((player.customProperties[PhotonPlayerProperty.dead] != null) && RCextensions.returnBoolFromObject(player.customProperties[PhotonPlayerProperty.dead])) && (RCextensions.returnIntFromObject(player.customProperties[PhotonPlayerProperty.isTitan]) != 2)) {
                                                FengGameManagerMKII.instance.photonView.RPC("respawnHeroInNewRound", player, new object[0]);
                                            }
                                        }
                                    } else {
                                        num8 = Convert.ToInt32(this.inputLine.Substring(8));
                                        foreach (PhotonPlayer player in PhotonNetwork.playerList) {
                                            if (player.ID == num8) {
                                                this.addLINE("<color=#FFCC00>Player " + num8.ToString() + " has been revived.</color>");
                                                if (((player.customProperties[PhotonPlayerProperty.dead] != null) && RCextensions.returnBoolFromObject(player.customProperties[PhotonPlayerProperty.dead])) && (RCextensions.returnIntFromObject(player.customProperties[PhotonPlayerProperty.isTitan]) != 2)) {
                                                    objArray5 = new object[] { "<color=#FFCC00>You have been revived by the master client.</color>", string.Empty };
                                                    FengGameManagerMKII.instance.photonView.RPC("Chat", player, objArray5);
                                                    FengGameManagerMKII.instance.photonView.RPC("respawnHeroInNewRound", player, new object[0]);
                                                }
                                            }
                                        }
                                    }
                                } else {
                                    this.addLINE("<color=#FFCC00>error: not master client</color>");
                                }
                            } else if (this.inputLine.StartsWith("/RCunban")) {
                                if (FengGameManagerMKII.OnPrivateServer) {
                                    FengGameManagerMKII.ServerRequestUnban(this.inputLine.Substring(9));
                                } else if (PhotonNetwork.isMasterClient) {
                                    int num9 = Convert.ToInt32(this.inputLine.Substring(9));
                                    if (FengGameManagerMKII.banHash.ContainsKey(num9)) {
                                        objArray5 = new object[] { "<color=#FFCC00>" + ((string)FengGameManagerMKII.banHash[num9]) + " has been unbanned from the server. </color>", string.Empty };
                                        FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, objArray5);
                                        FengGameManagerMKII.banHash.Remove(num9);
                                    } else {
                                        this.addLINE("error: no such player");
                                    }
                                } else {
                                    this.addLINE("<color=#FFCC00>error: not master client</color>");
                                }
                            } else if (this.inputLine.StartsWith("/rules")) {
                                this.writeRules();
                            } else if (this.inputLine.StartsWith("/RCkick")) {
                                num8 = Convert.ToInt32(this.inputLine.Substring(8));
                                object[] objArray6;
                                object[] objArray7;
                                bool flag2;
                                if (num8 == PhotonNetwork.player.ID) {
                                    this.addLINE("error:can't kick yourself.");
                                } else if (!(FengGameManagerMKII.OnPrivateServer || PhotonNetwork.isMasterClient)) {
                                    objArray6 = new object[] { "/kick #" + Convert.ToString(num8), LoginFengKAI.player.name };
                                    FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, objArray6);
                                } else {
                                    flag2 = false;
                                    foreach (PhotonPlayer player3 in PhotonNetwork.playerList) {
                                        if (num8 == player3.ID) {
                                            flag2 = true;
                                            if (FengGameManagerMKII.OnPrivateServer) {
                                                FengGameManagerMKII.instance.kickPlayerRC(player3, false, "");
                                            } else if (PhotonNetwork.isMasterClient) {
                                                FengGameManagerMKII.instance.kickPlayerRC(player3, false, "");
                                                objArray7 = new object[] { "<color=#FFCC00>" + RCextensions.returnStringFromObject(player3.customProperties[PhotonPlayerProperty.name]) + " has been kicked from the server!</color>", string.Empty };
                                                FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, objArray7);
                                            }
                                        }
                                    }
                                    if (!flag2) {
                                        this.addLINE("error:no such player.");
                                    }
                                }
                            } else if (this.inputLine.StartsWith("/RCban")) {
                                bool flag2;
                                object[] objArray6;
                                object[] objArray7;

                                if (this.inputLine == "/RCbanlist") {
                                    this.addLINE("<color=#FFCC00>List of banned players:</color>");
                                    foreach (int num10 in FengGameManagerMKII.banHash.Keys) {
                                        this.addLINE("<color=#FFCC00>" + Convert.ToString(num10) + ":" + ((string)FengGameManagerMKII.banHash[num10]) + "</color>");
                                    }
                                } else {
                                    num8 = Convert.ToInt32(this.inputLine.Substring(7));
                                    if (num8 == PhotonNetwork.player.ID) {
                                        this.addLINE("error:can't kick yourself.");
                                    } else if (!(FengGameManagerMKII.OnPrivateServer || PhotonNetwork.isMasterClient)) {
                                        objArray6 = new object[] { "/kick #" + Convert.ToString(num8), LoginFengKAI.player.name };
                                        FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, objArray6);
                                    } else {
                                        flag2 = false;
                                        foreach (PhotonPlayer player3 in PhotonNetwork.playerList) {
                                            if (num8 == player3.ID) {
                                                flag2 = true;
                                                if (FengGameManagerMKII.OnPrivateServer) {
                                                    FengGameManagerMKII.instance.kickPlayerRC(player3, true, "");
                                                } else if (PhotonNetwork.isMasterClient) {
                                                    FengGameManagerMKII.instance.kickPlayerRC(player3, true, "");
                                                    objArray7 = new object[] { "<color=#FFCC00>" + RCextensions.returnStringFromObject(player3.customProperties[PhotonPlayerProperty.name]) + " has been banned from the server!</color>", string.Empty };
                                                    FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, objArray7);
                                                }
                                            }
                                        }
                                        if (!flag2) {
                                            this.addLINE("error:no such player.");
                                        }
                                    }
                                }
                            } else {
                                ModMain.instance.parseChat(this.inputLine);
                                this.modCommandIndex = 0;
                                this.inputLine = string.Empty;
                                GUI.FocusControl(string.Empty);
                                return;
                            }
 * */

