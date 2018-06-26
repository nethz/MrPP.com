using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace MrPP.Myth { 
    public class Model : NetworkBehaviour
    {
        public UnityEvent _onGodChange;
        private bool hasGod_ = false;
        public bool hasGod {
            get {
                return hasGod_;
            }
        }

        public uint godId
        {
            get
            {
                return _godId;
            }
            set
            {
                _godId = value;
            }
        }
        [SerializeField]
        [SyncVar(hook = "changeGodId")]
        private uint _godId = 0;
        private void changeGodId(uint id)
        {
            _godId = id;
            hasGod_ = true;
            if (_onGodChange != null) {
                _onGodChange.Invoke();
            }

        }
    }
}