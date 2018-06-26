using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace MrPP.Myth {
    public class WhoIsGod : NetworkBehaviour
    {



        [Command]
        public void CmdIAmGod(uint id) {
            Root.Instance.model.godId = id;
        }
        public void IAmGod()
        {

            CmdIAmGod(this.netId.Value);
        }
        public bool isItMe()
        {
            if (Root.Instance.model.hasGod) {
                return Root.Instance.model.godId == this.netId.Value;
            }
            return true;
            
        }
    }
}