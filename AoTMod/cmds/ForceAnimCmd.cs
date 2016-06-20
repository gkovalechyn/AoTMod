using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
class ForceAnimCmd : ICommand{
    public bool cmd(string[] args, FengGameManagerMKII gm) {
        int id = int.Parse(args[0]);

        foreach (HERO h in GameObject.FindObjectsOfType<HERO>()) {
            if (h.photonView.owner.ID == id) {
                h.photonView.RPC("netPlayAnimation", h.photonView.owner, args[1]);
            }
        }

        return true;
    }

    public string getDescriptionString() {
        return "(Not implemented) Forces a player to do an animation.";
    }
}
