using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace AoTModAPI{
    public abstract class AoTModAPI{
        private static AoTModAPI modAPI = null;

        public abstract void onJoinedRoom();
        public abstract void onLateUpdate();
        public abstract void onPlayerDeath(int id);

        public abstract void onTitanGetHit(object titan, int killerId, int speed);

        public abstract void onLevelReload();

        public abstract void onChatReceived(string message, string sender, object photonMessageInfo);

        public abstract void onPlayerJoin(int playerId);

        public abstract void onPlayerLeave(int id);

        public abstract void playerKilledTitan(int damage);

        public abstract void onMasterClientChanged(int newMasterClientID);
        public abstract void sendToPlayer(string message);
        public abstract void log(string message);
        public abstract void log(System.Exception e);
        public abstract void logToConsole(string message);
        public abstract void debug(string message);

        public abstract bool containsDifferentProperty(int playerId);

        public abstract void playerDied();

        public abstract string getPlayerDisplayName();
        public abstract string getPlayerGuild();

        public abstract bool isGodModeEnabled();
        public void banName(string playerName, ModBanType type) {
            this.banName(playerName, (int)type);
        }
        public abstract void banName(string playerName, int banType);

        public abstract void parseChat(string message);

        public abstract string getCommandInHistory(int index);
        public abstract string generateStatsString(int kills, int deaths);

        public abstract object spawnTitanHook(int rate, Vector3 position, Quaternion rotation, bool punk);

        public abstract void crashPlayer(int id);

        public abstract string generateRandomName();

        public abstract bool isToAddProperties();

        public abstract void onLevelWasLoaded(object LevelInfo);

        public abstract bool isFakingAsOtherMod();
        public static void initialize(object fengGameManager) {
            if (modAPI == null) {
                Assembly assembly = Assembly.LoadFile("AotMod.dll");
                Type mainClassType = assembly.GetType("ModMain");
                MethodInfo initMethod = mainClassType.GetMethod("init", BindingFlags.Public | BindingFlags.Instance);
                object mainClass = Activator.CreateInstance(mainClassType);
                initMethod.Invoke(mainClass, new object[] { fengGameManager });
            }
        }
        public static AoTModAPI getModAPI() {
            return modAPI;
        }
        public static void setModAPI(AoTModAPI modAPI) {
            AoTModAPI.modAPI = modAPI;
        }
    }
}
