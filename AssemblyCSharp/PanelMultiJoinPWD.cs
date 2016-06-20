using System;
using UnityEngine;

public class PanelMultiJoinPWD : MonoBehaviour
{
    public static string Password = string.Empty;
    public static string roomName = string.Empty;

    public Rect windowRect = new Rect(50, 50, 150, 90);

    public Texture btnTexture;

    public void OnGUI()
    {
        windowRect = GUI.Window(0,windowRect,DoMyWindow, "Password");

    }
    void DoMyWindow(int windowID)
    {
        SimpleAES eaes = new SimpleAES();
        if(GUI.Button(new Rect(80,30,20,20), "Click"))
        {
            NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().PanelMultiPWD, false);
            NGUITools.SetActive(GameObject.Find("UIRefer").GetComponent<UIMainReferences>().panelMultiROOM, true);
            GameObject.Find("PanelMultiROOM").GetComponent<PanelMultiJoin>().refresh();
        }
        GUI.Label(new Rect(20,60,150,150), "<color=white>PWD: " + eaes.Decrypt(Password) + "</color>");
        GUI.Label(new Rect(30, 30, 120, 120), "Leave:");
    }
}

