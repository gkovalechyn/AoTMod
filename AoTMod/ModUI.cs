using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
class ModUI : MonoBehaviour{
    private new bool enabled = true;
    private LinkedList<string> messages = new LinkedList<string>();
    private int rectWidth = 300;
    private int rectHeight = 200;
    private Vector2 scrollViewPosition = Vector2.zero;

    int messageWidth = 275;
    int messageHeight = 40;
    const int LINE_HEIGHT = 20;
    public void Start() {

    }
    public void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            this.toggle();
        }
    }
    public void OnGUI() {
        if (this.enabled) {
            Rect rect = new Rect(Screen.width - rectWidth, Screen.height - rectHeight, rectWidth, rectHeight);
            GUI.Window(0x69, rect, this.doConsoleWindow, "Console");
        }
    }

    private void doConsoleWindow(int windowId) {
        //GUI.DragWindow();
        this.scrollViewPosition = GUI.BeginScrollView(new Rect(5, 20, this.rectWidth - 12, this.rectHeight - 30), this.scrollViewPosition, new Rect(0, 0, this.messageWidth, this.messageHeight * this.messages.Count));
        int currentHeight = 0;

        foreach (string s in this.messages){
            GUI.Label(new Rect(0, currentHeight, this.messageWidth, this.messageHeight), s);
            currentHeight += this.messageHeight;
        }

        GUI.EndScrollView(true);
    }

    public void addToLog(string message) {
        this.messages.AddFirst(message);

        if (this.messages.Count > 16) {
            this.messages.RemoveLast();
        }
    }

    public void toggle() {
        this.enabled = !this.enabled;
    }
}
