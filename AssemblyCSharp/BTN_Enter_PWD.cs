using System;
using UnityEngine;

public class BTN_Enter_PWD : MonoBehaviour
{
    private void OnClick()
    {
        string text = GameObject.Find("InputEnterPWD").GetComponent<UIInput>().label.text;
        SimpleAES eaes = new SimpleAES();
            PhotonNetwork.JoinRoom(PanelMultiJoinPWD.roomName);
    }
}
