using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.IO;

public class ConfigManager {
    private Dictionary<string, object> configValues;

    private ConfigManager() {
        this.configValues = new Dictionary<string, object>();
    }
    public string get(string path) {
        object res = null;
        configValues.TryGetValue(path, out res);
        return (string)res;
    }

    public bool getBool(string path) {
        object res = null;
        configValues.TryGetValue(path, out res);

        if (res != null && !string.IsNullOrEmpty((string)res)) {
            return ((string)res).Equals("true", StringComparison.OrdinalIgnoreCase);
        } else {
            return false;
        }
    }

    public void set(string path, string value) {
        try {
            this.configValues.Add(path, value);
        } catch (ArgumentException) {
            this.configValues.Remove(path);
            this.configValues.Add(path, value);
        }
    }

    public void set(string path, List<object> list) {
        try {
            this.configValues.Add(path, list);
        } catch (ArgumentException) {
            this.configValues.Remove(path);
            this.configValues.Add(path, list);
        }
    }

    public static ConfigManager load(string path) {
        ConfigManager result = new ConfigManager();

        if (!File.Exists(path)) {
            ConfigManager.saveDefaultConfig(path);
        } else {
            ConfigManager.compareAndUpdateConfig(path);
        }

        ConfigManager.fileToDictionary(path, result.configValues);

        return result;
    }

    private static void fileToDictionary(string path, Dictionary<string, object> result) {
        StreamReader fileIn = File.OpenText(path);
        int line = 1;

        while (fileIn.Peek() >= 0) {
            string fullLine = fileIn.ReadLine().Trim();
            string[] parts;

            if (string.IsNullOrEmpty(fullLine) || fullLine.StartsWith(";")) {
                line++;
                continue;
            }

            parts = fullLine.Split('=');

            if (parts.Length == 1) {
                continue;
            } else {
                for (int i = 2; i < parts.Length; i++) {
                    parts[1] += "=" + parts[i];
                }
            }

            if (parts[1].Equals("[")) {
                List<string> list = new List<string>();
                string readLine;

                while (!(readLine = fileIn.ReadLine()).Equals("]")) {
                    list.Add(readLine.Replace(@"\r\n", "\r\n").Replace(@"\n", "\n").Replace(@"\t", "\t").Replace(@"\", "\\"));
                    line++;
                }
                //Increase the line because of the ]
                line++;
                result[parts[0]] = list;
            } else if (parts[1].Equals("\\[")) {
                result[parts[0]] = "[";
            } else {
                result[parts[0]] = parts[1].Replace(@"\r\n", "\r\n").Replace(@"\n", "\n").Replace(@"\t", "\t").Replace(@"\", "\\");
            }
        }
        fileIn.Close();
    }

    public void save(string file) {
        ConfigManager.saveConfigRaw(file, this.configValues);
    }

    private static void saveConfigRaw(string dest, Dictionary<string, object> configValues) {
        try {
            StreamWriter fileOut = new StreamWriter(dest, false, System.Text.Encoding.UTF8);

            foreach (KeyValuePair<string, object> entry in configValues) {
                if (entry.Value is List<string>) {
                    List<string> list = (List<string>)entry.Value;

                    fileOut.WriteLine(entry.Key + "=[");
                    foreach (object o in list) {
                        string item = ((string)o).Replace("\r\n", @"\r\n").Replace("\n", @"\n").Replace("\t", @"\t").Replace("\\", @"\");
                        fileOut.WriteLine(item);
                    }

                    fileOut.WriteLine(']');
                } else {
                    if (entry.Value.Equals("[")) {
                        fileOut.WriteLine(entry.Key + "=\\[");
                    } else {
                        string item = ((string) entry.Value).Replace("\r\n", @"\r\n").Replace("\n", @"\n").Replace("\t", @"\t").Replace("\\", @"\");
                        fileOut.WriteLine(entry.Key + "=" + item);
                    }
                }

            }

            fileOut.Flush();
            fileOut.Close();
        } catch (IOException e) {
            ModMain.instance.log("Could not save configuration.");
            ModMain.instance.log(e);
        }
    }

    public List<string> getStringList(string path) {
        object result;
        this.configValues.TryGetValue(path, out result);

        return result != null ? (List<string>)result : null;
    }

    public Dictionary<string, object>.KeyCollection getKeyList() {
        return this.configValues.Keys;
    }

    private static void saveDefaultConfig(string file) {
        StreamWriter fileOut = File.CreateText(file);
        fileOut.WriteLine(";In-chat name");
        fileOut.WriteLine("name=<color=#000000>Test</color>");
        fileOut.WriteLine(";These will be added to the message you type in chat.");
        fileOut.WriteLine("messagePrefix=<b>");
        fileOut.WriteLine("messageSuffix=</b>");
        fileOut.WriteLine(";Top left name");
        fileOut.WriteLine("displayName=[000000]Test");
        fileOut.WriteLine("guild=[000000]Test guild");
        fileOut.WriteLine(";Target average damage below the names");
        fileOut.WriteLine("averageDamage=1000");
        fileOut.WriteLine("showMessagesToPlayers=false");
        fileOut.WriteLine("enableModMessage=False");
        fileOut.WriteLine("modMessage=[MOTD] Get rekt m8.");
        fileOut.WriteLine("killTitanName=Titan");
        fileOut.WriteLine("chatLogEnabled=false");
        fileOut.WriteLine(";This is the function used to tell how many titans will spawn in the next wave");
        fileOut.WriteLine(";You can use the \"Wave\" and \"PlayerCount\" variables in this function");
        fileOut.WriteLine("TitanWaveAmountFunction=Wave + 2");
        fileOut.WriteLine(";This is the wave message sent once a wave finishes.");
        fileOut.WriteLine(";Again, you can use the {Wave} and {TitanCount} \"Variables\" (These are case-sensitive)");
        fileOut.WriteLine(";And require the {} characters around them WITHOUT SPACES.");
        fileOut.WriteLine("WaveEndMessage=<color=#FFFF00><b>Next wave: {Wave}. Titans: {TitanCount}</b></color>");
        fileOut.WriteLine(";Personas to be used with the /persona command");
        fileOut.WriteLine("persona1=Persona1");
        fileOut.WriteLine("persona1DN=Persona1");
        fileOut.WriteLine("persona1Guild=Guild1");
        fileOut.WriteLine("persona1Set=1");
        fileOut.WriteLine("persona2=Persona2");
        fileOut.WriteLine("persona2DN=Persona2");
        fileOut.WriteLine("persona2Guild=Guild2");
        fileOut.WriteLine("persona2Set=2");
        fileOut.WriteLine("persona3=Persona3");
        fileOut.WriteLine("persona3DN=Persona3");
        fileOut.WriteLine("persona3Guild=Guild3");
        fileOut.WriteLine("persona3Set=3");
        fileOut.WriteLine(";Names of the automatic name changer");
        fileOut.WriteLine("NameChangerNames=[");
        fileOut.WriteLine("Get");
        fileOut.WriteLine("Rekt");
        fileOut.WriteLine("M8");
        fileOut.WriteLine("]");
        fileOut.WriteLine(";The colors of the help command list.");
        fileOut.WriteLine("HelpColors=[");
        fileOut.WriteLine("00FF00");
        fileOut.WriteLine("FFFF00");
        fileOut.WriteLine("]");
        fileOut.Flush();
        fileOut.Close();
    }

    private static void compareAndUpdateConfig(string actual) {
        Dictionary<string, object> defaultConfig = new Dictionary<string, object>();
        Dictionary<string, object> currentConfig = new Dictionary<string, object>();

        ConfigManager.saveDefaultConfig(".tmp");

        ConfigManager.fileToDictionary(actual, currentConfig);
        ConfigManager.fileToDictionary(".tmp", defaultConfig);

        File.Delete(".tmp");

        foreach (string s in currentConfig.Keys) {
            if (defaultConfig.ContainsKey(s)) {
                defaultConfig.Remove(s);
            }
        }

        foreach (string s in defaultConfig.Keys) {
            currentConfig[s] = defaultConfig[s];
        }

        ConfigManager.saveConfigRaw(actual, currentConfig);
    }
}

