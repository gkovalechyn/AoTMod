using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class CommandBinder {
    private Dictionary<KeyCode, string> boundCommands = new Dictionary<KeyCode, string>();

    public CommandBinder(TaskManager taskManager) {
        taskManager.addLateUpdateTask(new KeyCheckerTask(this));
    }
    public void bind(KeyCode code, string command) {
        this.boundCommands[code] = command;
    }

    public void unbind(KeyCode code) {
        this.boundCommands.Remove(code);
    }

    public Dictionary<KeyCode, string> getBoundCommands() {
        return this.boundCommands;
    }

    private class KeyCheckerTask : Task{
        private CommandBinder binder;

        public KeyCheckerTask(CommandBinder binder) {
            this.binder = binder;
        }
        public bool execute() {
            if (string.IsNullOrEmpty(GUI.GetNameOfFocusedControl())) {
                Dictionary<KeyCode, string> cmds = this.binder.getBoundCommands();

                lock (cmds) {
                    foreach (KeyValuePair<KeyCode, string> pair in cmds) {
                        if (Input.GetKeyDown(pair.Key)) {
                            ModMain.instance.getCommandManager().parseCommand(pair.Value, ModMain.instance.getGameManager());
                        }
                    }
                }
            }
            return false;
        }
    }
}
