using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrPP.Myth { 
    public class AsgardTranformLockedUpdate : MonoBehaviour {


        public void updateLocked() {
            AsgardTranformLocked[] locks = FindObjectsOfType<AsgardTranformLocked>();
            foreach (AsgardTranformLocked l in locks) {
                l.locked();

            }
        }
    }
}