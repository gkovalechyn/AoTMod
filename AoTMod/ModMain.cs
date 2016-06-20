using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Diagnostics;
using GITS.Util.Json;

public class ModMain {

    private FengGameManagerMKII fGameManager = null;
    private int lastViewId = -1;

    private StreamWriter logOut;

#if DEBUG
    private StreamWriter debugOut;
#endif

    private NameManager nameManager;
    private ConfigManager config;
    private TitanHealthController thController;
    private CManager cManager;
    private GameModEventInterface gameModInterface;
    private SpawnController spawnController;
    private TaskManager taskManager;
    private NameChanger nameChanger;
    private LagController lagController;
    private CommandBinder commandBinder;

    private float averageDamage = 300f;

    public static ModMain instance;

    private CommandManager commandManager;
    private PlayerGreeter greeter;
    private OtherPlayerCommandsManager opcm;

    private bool godMode = false;

    private bool chatLogEnabled;
    public bool writeChatLogEvents = false;

    private StreamWriter chatLogOut;
    private string chatLogFilename = null;

    private MainModThread bct;

    private static Regex colorRegex = new Regex("(\\[(([0-F]{6})|(\\-))\\])", RegexOptions.IgnoreCase);

    private ModUI modUI = null;

    private bool changeMasterClientByCommand = false;
    private bool wasIMasterClientF = false;
    private bool toAddProperties = true;

    private bool fakingAsOtherMod = false;
    private string[] otherModProperties = null;
    private object[] otherModValues = null;


    private Dictionary<int, string> otherPlayersChatName = new Dictionary<int, string>();
    //Chat messages
    private string messagePrefix;
    private string messageSuffix;


    private JsonObject settingsObject = null;

    //Wether I am hidden in the list
    private bool hidden = false;
#if DEBUG
    public bool debugEnabled = false;
    public DebugMode debugMode = DebugMode.FILE;
#endif

    //returns true should be shown in chat, false otherwise
    public void parseChat(string chatContent) {
        if (chatContent.StartsWith("/")) {
            commandManager.parseCommand(chatContent, this.getGameManager());
        } else {
            this.getGameManager().photonView.RPC("Chat", PhotonTargets.All, this.messagePrefix + chatContent + this.messageSuffix, this.getNameManager().getPlayerName());
        }
    }

    private void eventCallback(byte eventCode, object content, int sender) {
        this.log("Recievent event code: " + eventCode);
        this.log("Sender: " + sender);
        this.log("\tContent type: " + content.GetType());
        this.log("\tContent: " + content);

        if (content is ExitGames.Client.Photon.Hashtable) {
            ExitGames.Client.Photon.Hashtable table = (ExitGames.Client.Photon.Hashtable)content;
            foreach (object key in table.Keys) {
                this.log("\t\tKey: " + key + "=" + table[key]);
            }
        }
    }
    public void sendToPlayer(string[] messages) {
        //InRoomChat chat = GameObject.Find("Chatroom").GetComponent<InRoomChat>();
        //if (chat != null) {
        foreach (string message in messages) {
            FengGameManagerMKII.instance.chatRoom.addLINE(message);
        }
        //}
    }

    public void sendToPlayer(string message) {
        //InRoomChat chat = GameObject.Find("Chatroom").GetComponent<InRoomChat>();
        //if (chat != null) {
            FengGameManagerMKII.instance.chatRoom.addLINE(message);
        //}
    }

    public void onMasterClientChange() {
        this.log("OnMasterClientChange");
        this.log("\tWasIMasterClient? " + this.wasIMasterClientF);
        this.log("\tDidIChangeMC? " + this.didIChangeMasterClient());

        if (this.wasIMasterClientF && !this.didIChangeMasterClient()) {
            this.log("\tSomeone stole the MasterClient from me.");
            //PhotonNetwork.SetMasterClient(PhotonNetwork.player);
        } else {
            this.log("\tEverything\'s fine.");
            this.wasIMasterClientF = false;
            this.setDidIChangeMasterClient(false);
        }
    }

    public bool otherPlayerHasChatName(int index) {
        return this.otherPlayersChatName.ContainsKey(index);
    }

    public string getOtherPlayersChatName(int index) {
        return this.otherPlayersChatName[index];
    }

    public void setGameManager(object fgm) {
        this.fGameManager = (FengGameManagerMKII)fgm;
    }

    public FengGameManagerMKII getGameManager() {
        if (this.fGameManager == null) {
            this.refindGameManager();
        }

        return this.fGameManager;
    }
    public void log(Exception e) {
        StackTrace stackTrace = new StackTrace(e, true);
        this.log("----Exception----");
        this.log(e.ToString());
        this.log("----Stack trace----");
        this.log(stackTrace.ToString());
        this.log("----End----");
        //this.log(e.ToString());
    }

    public void log(string message) {
        this.logOut.WriteLine(message);
        this.logOut.Flush();
    }

    public void logNoNewline(string message) {
        this.logOut.Write(message);
    }

#if DEBUG
    public void debug(string message) {
        if (this.debugEnabled) {
            switch (this.debugMode) {
                case DebugMode.CHAT:
                    this.sendToPlayer(message);
                    break;
                case DebugMode.FILE:
                    this.debugOut.WriteLine(message);
                    this.debugOut.Flush();
                    break;
                default:
                    return;
            }
        }
    }
#endif

    public NameManager getNameManager() {
        return this.nameManager;
    }

    public void updateFakeModProperties() {
        if (this.otherModProperties != null) {
            ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable();

            for (int i = 0; i < this.otherModProperties.Length; i++) {
                table[this.otherModProperties[i]] = this.otherModValues[i];
            }

            PhotonNetwork.player.SetCustomProperties(table);
        }
    }
    public string generateStatsString(int kills, int damage) {
        string res = "";

        if (kills == 0) {
            res = Colorizer.colorize("0/0", Colorizer.Color.YELLOW);
        } else {
            float totalDamage = kills * this.averageDamage;
            float average = ((float)damage) / kills;

            if (average < this.averageDamage) {
                res += Colorizer.colorize("Average/Overflow: " + average + '(' + this.averageDamage + ") / -" + (totalDamage - damage), Colorizer.Color.RED);
            } else {
                res += Colorizer.colorize("Average/Overflow: " + average + '(' + this.averageDamage + ") / +" + (damage - totalDamage), Colorizer.Color.GREEN);
            }
        }
        global::IN_GAME_MAIN_CAMERA component = GameObject.Find("MainCamera").GetComponent<global::IN_GAME_MAIN_CAMERA>();

        if (component != null && component.main_object != null && component.main_object.rigidbody != null) {
            res += "\n[FFFFFF]Damage: " + (int)(component.main_object.rigidbody.velocity.magnitude * 10f);
        } else {
            res += "\n[FFFFFF]Velocity not available";
        }

        return res;
    }

    public void sendToAll(string message) {
        this.fGameManager.sendChatContentInfo(message);
    }

    private void restart() {
        MethodInfo respawnMethod = this.fGameManager.GetType().GetMethod("restartGame", BindingFlags.Instance | BindingFlags.NonPublic);
        respawnMethod.Invoke(this.fGameManager, new object[] { false });
    }

    public void sendToPlayer(PhotonPlayer player, string message) {
        object[] param = new object[] { message, String.Empty };
        this.fGameManager.photonView.RPC("Chat", player, param);
    }

    public ConfigManager getConfig() {
        return this.config;
    }

    public bool useGas() {
        //return this.gas;
        return HERO.modUseGas;
    }

    public bool useBlades() {
        //return this.blades;
        return HERO.modUseBlades;
    }

    public void setUseGas(bool gas) {
        HERO.modUseGas = gas;
        /*
        foreach(HERO h in GameObject.FindObjectsOfType<HERO>()){
            if (h.photonView.ownerId == PhotonNetwork.player.ID){
                HERO.modUseGas = gas;
            }
        }
        */
    }

    public void setUseBlades(bool blades) {
        HERO.modUseBlades = blades;
        /*
        foreach(HERO h in GameObject.FindObjectsOfType<HERO>()){
            if (h.photonView.ownerId == PhotonNetwork.player.ID){
                h.modUseBlades = blades;
            }
        }
        */
    }

    public PlayerGreeter getGreeter() {
        return this.greeter;
    }

    public void setAverageDamage(float newDamage) {
        this.averageDamage = newDamage;
    }

    public TitanHealthController getTHController() {
        return this.thController;
    }

    public string unencryptServerPassword(string encripted) {
        try {
            return new SimpleAES().Decrypt(encripted);
        } catch (Exception e) {
            this.log(e);
            return "ERROR";
        }
    }

    public void setChatLogEnabled(bool val) {
        this.chatLogEnabled = val;
        if (val) {
            if (this.chatLogOut != null) {
                chatLogOut.Close();
            }
            this.createChatLogWriter();
        } else {
            this.closeChatLogWriter();
        }
    }

    public bool isChatLogEnabled() {
        return this.chatLogEnabled;
    }

    public void onRoomEnter() {
        this.refindGameManager();
        this.log("RoomEnter::GameManagerInstantiationID: " + this.fGameManager.photonView.instantiationId);
        this.log("RoomEnter::GameManagerViewID: " + this.fGameManager.photonView.viewID);
        //this.inRoomChat = GameObject.Find("Chatroom").GetComponent<InRoomChat>();
        if (!Directory.Exists("./ChatLog")) {
            Directory.CreateDirectory("./ChatLog");
        }

        if (this.chatLogOut != null) {
            this.closeChatLogWriter();
        }

        if (this.chatLogEnabled) {
            this.createChatLogWriter();

            this.writeChatLogEvent("Joined room \"" + PhotonNetwork.room.name + '\"');
        }


        this.wasIMasterClientF = PhotonNetwork.isMasterClient;

        if (this.isFakingAsOtherMod()) {
            if (this.otherModProperties == null) {
                ModMain.instance.sendToPlayer("You were supposed to be faking as another player but no properties exist?");
            } else {
                this.updateFakeModProperties();
            }
        }
    }

    private void createChatLogWriter() {
        if (!PhotonNetwork.inRoom) {
            return;
        }

        string roomName = PhotonNetwork.room.name.Split(new char[] { '`' })[0];
        string path = "./ChatLog/" + DateTime.Now.ToLongDateString() + "@" + roomName + ".txt";
        int i = 1;

        while (File.Exists(path)) {
            path = path.Substring(0, path.Length - 4);
            path += i + ".txt";
            i++;
        }

        this.chatLogFilename = path;

        this.chatLogOut = new StreamWriter(path);
        this.chatLogOut.WriteLine("Chat log created at {0} {1}", DateTime.Now.ToLongTimeString(),
                                  DateTime.Now.ToLongDateString());
    }

    private void closeChatLogWriter() {
        if (this.chatLogOut != null) {
            MD5 md5 = MD5.Create();
            SHA1 sha1 = SHA1Managed.Create();
            string md5Res;
            string sha1Res;
            string fileText;
            byte[] fileBytes;

            this.chatLogOut.WriteLine("End of chat log at {0} {1}", DateTime.Now.ToLongTimeString(),
                                      DateTime.Now.ToLongDateString());
            this.chatLogOut.Close();
            this.chatLogOut = null;

            //Write the checksum
            fileText = File.ReadAllText(this.chatLogFilename);
            fileBytes = Encoding.GetEncoding("UTF-8").GetBytes(fileText);

            md5Res = BitConverter.ToString(md5.ComputeHash(fileBytes)).Replace("-", "").ToLowerInvariant();
            sha1Res = BitConverter.ToString(sha1.ComputeHash(fileBytes)).Replace("-", "").ToLowerInvariant();

            this.chatLogOut = new StreamWriter(this.chatLogFilename, false, System.Text.Encoding.UTF8);
            this.chatLogOut.WriteLine("MD5 Hash: " + md5Res);
            this.chatLogOut.WriteLine("SHA-1 Hash: " + sha1Res);
            this.chatLogOut.Write(fileText);

            this.chatLogOut.Close();
            this.chatLogOut = null;
            this.chatLogFilename = null;
            md5.Clear();
            sha1.Clear();
        }
    }

    public void chatRecieved(string sender, string message, PhotonMessageInfo info) {
        if (!string.IsNullOrEmpty(sender)) {
            this.otherPlayersChatName[info.sender.ID] = sender;
            opcm.handleChat(sender, message);
        }

        if (this.chatLogEnabled) {
            this.chatLogOut.WriteLine("{0} [#" + info.sender.ID + "]{1}",
                                      DateTime.Now.ToLongTimeString(),
                                      message);
            this.chatLogOut.Flush();

        }
    }

    public void writeToChatLog(string message) {
        if (this.chatLogEnabled) {
            lock (this.chatLogOut) {
                this.chatLogOut.WriteLine("{0} {1}",
                                          DateTime.Now.ToLongTimeString(),
                                          message);
                this.chatLogOut.Flush();
            }
        }
    }

    public void writeChatLogEvent(string message) {
        if (this.chatLogEnabled && this.writeChatLogEvents) {
            lock (this.chatLogOut) {
                this.chatLogOut.WriteLine("[EVENT] {0} {1}",
                                          DateTime.Now.ToLongTimeString(),
                                          message);
                this.chatLogOut.Flush();
            }
        }
    }

    public MainModThread getModMainThread() {
        return this.bct;
    }

    public static string stripColorCodes(string src) {
        return ModMain.colorRegex.Replace(src.Trim(), "");
    }

    public bool containsDifferentProperty(PhotonPlayer player) {
        foreach (string s in player.customProperties.Keys) {
            if (!DetectCmd.commonProperties.Contains(s)) {
                return true;
            }
        }

        return false;
    }

    public void setMessageCount(int newAmount) {
        InRoomChat.numMessagesToBeDisplayed = newAmount;
    }

    public int getMessageCount() {
        return InRoomChat.numMessagesToBeDisplayed;
    }

    public CommandManager getCommandManager() {
        return this.commandManager;
    }

    public bool didIChangeMasterClient() {
        return this.changeMasterClientByCommand;
    }

    public void setDidIChangeMasterClient(bool val) {
        this.changeMasterClientByCommand = val;
    }

    public bool wasIMasterClient() {
        return this.wasIMasterClientF;
    }

    public CManager getChampionshipManager() {
        return this.cManager;
    }

    public GameModEventInterface getModInterface() {
        return this.gameModInterface;
    }

    public SpawnController getSpawnController() {
        return this.spawnController;
    }

    public TaskManager getTaskManager() {
        return this.taskManager;
    }

    public CommandBinder getCommandBinder() {
        return this.commandBinder;
    }
    public NameChanger getNameChanger() {
        return this.nameChanger;
    }

    public LagController getLagController() {
        return this.lagController;
    }

    public bool isGodMode() {
        return this.godMode;
    }

    public void setGodMode(bool value) {
        this.godMode = value;
    }

    public void saveSettingsFile() {
        this.settingsObject.save("settings.json");
    }

    public JsonObject getSettings() {
        return this.settingsObject;
    }

    public void init(object gm) {
        if (File.Exists("log.txt")) {
            logOut = File.AppendText("log.txt");
        } else {
            logOut = File.CreateText("log.txt");
        }
#if DEBUG
        this.debugOut = File.CreateText("debug.log");
#endif

        this.log("====init() was called====");
        try {
            this.fGameManager = (FengGameManagerMKII)gm;
            GameObject go = new GameObject();
            go.name = "ModUI";
            GameObject.DontDestroyOnLoad(go);
            this.modUI = go.AddComponent<ModUI>();

            this.log("----Initializing mod.");
            this.log("Loading the configuration.");
            this.config = ConfigManager.load("config.cfg");
            this.log("Loading settings file.");
            if (File.Exists("settings.json")) {
                try {
                    this.settingsObject = JsonObject.fromFile("settings.json");
                    File.Move("settings.json", "settings.json.corrupted" + UnityEngine.Random.Range(0, 10000));
                }catch(JsonException e) {
                    this.log(e);
                    this.settingsObject = new JsonObject();
                }
            } else {
                this.settingsObject = new JsonObject();
            }

            this.taskManager = new TaskManager();
            gameModInterface = new GameModEventInterface(this);

            this.spawnController = new SpawnController(this.fGameManager, config.get("TitanWaveAmountFunction"), config.get("WaveEndMessage"));

            this.commandBinder = new CommandBinder(this.taskManager);

            this.cManager = new CManager();

            this.nameManager = new NameManager(this.config);
            commandManager = new CommandManager();

            PhotonNetwork.OnEventCall = this.eventCallback;

            this.averageDamage = float.Parse(config.get("averageDamage"));

            this.log("Initializing extras.");

            this.log("Creating the greeter.");
            greeter = new PlayerGreeter(this);
            this.log("Creating the mod thread.");
            bct = new MainModThread(this);
            opcm = new OtherPlayerCommandsManager(this.fGameManager);

            thController = new TitanHealthController();

            this.nameChanger = new NameChanger(this);

            this.lagController = new LagController();

            KillCmd.titanName = this.config.get("killTitanName");
            this.setChatLogEnabled(this.getConfig().getBool("chatLogEnabled"));

            this.setUseGas(true);
            this.setUseBlades(true);

            this.log("Registering mod API");
            AoTModAPI.AoTModAPI.setModAPI(this.gameModInterface);

            this.log("Updating values from the settings file.");
            this.log("Starting mod thread.");
            bct.start();
        } catch (System.Exception e) {
            this.log("Error while initializing mod, who knows what will happen.");
            this.log(e);
        }

        this.messagePrefix = this.getConfig().get("messagePrefix");
        this.messageSuffix = this.getConfig().get("messageSuffix");

        this.log("Mod initialized.");

        ModMain.instance = this;
        this.log("Instance: " + ModMain.instance);
        this.commandManager.buildHelpList();
    }
#if DEBUG
    public enum DebugMode {
        FILE = 1,
        CHAT = 2
    }
#endif

    public void logToConsole(string message) {
        if (this.modUI == null) {
            this.log("Could not find the mod UI");
        } else {
            this.modUI.addToLog(message);
        }
    }

    public bool isFakingAsOtherMod() {
        return this.fakingAsOtherMod;
    }

    public void setFakingAsOtherMod(bool val) {
        this.fakingAsOtherMod = val;
    }

    public void refindGameManager() {
        if (this.fGameManager == null) {
            this.log("Lost the game manager, trying to find.");
            this.fGameManager = GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>();
            
            if (this.fGameManager == null) {
                this.log("Could not find game manager, trying to instantiate.");
                this.fGameManager = new GameObject().AddComponent<FengGameManagerMKII>();

                if (this.lastViewId > 0) {
                    this.fGameManager.photonView.viewID = this.lastViewId;
                } else {
                    this.fGameManager.photonView.viewID = PhotonNetwork.AllocateViewID();
                }
            } else {
                this.lastViewId = fGameManager.photonView.viewID;
            }

            if (this.modUI == null) {
                GameObject go = new GameObject();
                go.name = "ModUI";
                GameObject.DontDestroyOnLoad(go);
                this.modUI = go.GetComponent<ModUI>();
            }
        }
    }

    public void setHidden(bool hidden) {
        this.hidden = hidden;
    }

    public bool isHidden() {
        return this.hidden;
    }
    public void setChatPrefix(string prefix) {
        this.messagePrefix = prefix;
    }

    public void setFakeModProperties(string[] keys, object[] values) {
        this.otherModProperties = keys;
        this.otherModValues = values;
    }

    public void setChatSuffix(string suffix) {
        this.messageSuffix = suffix;
    }

    public void setToAddProperties(bool val) {
        this.toAddProperties = val;
    }

    public bool isToAddProperties() {
        return this.toAddProperties;
    }
}

