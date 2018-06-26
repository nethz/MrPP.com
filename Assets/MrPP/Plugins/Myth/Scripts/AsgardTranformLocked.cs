using GDGeek;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace MrPP.Myth
{
    public class AsgardTranformLocked : NetworkBehaviour
    {
        [SerializeField]
        private MonoBehaviour[] _behaviours;

        public void Start() {

            locked();
        }
       

        // Use this for initialization
        public void locked()
        {

            if (Altar.AmIGod)
            {

                foreach (MonoBehaviour b in _behaviours) {
                    b.enabled = false;
                }
               // this.gameObject.enableBehaviour<MrPP.Helper.Billboard>();
               // this.gameObject.enableBehaviour<MrPP.Helper.MrBillboard>();
               // this.gameObject.enableBehaviour<MrPP.Helper.Tagalong>();
               // this.gameObject.enableBehaviour<MrPP.Helper.SimpleTagalong>();
               // this.gameObject.enableBehaviour<TapToPlace>();
               // this.gameObject.enableBehaviour<TapToPlaceOnce>();
               // this.gameObject.enableBehaviour<MrPP.Helper.TapToPlaceScene>();
            }
            else {


                foreach (MonoBehaviour b in _behaviours)
                {
                    b.enabled = true;
                }
                //  this.gameObject.disableBehaviour<MrPP.Helper.Billboard>();
               // this.gameObject.disableBehaviour<MrPP.Helper.MrBillboard>();
              //  this.gameObject.disableBehaviour<MrPP.Helper.Tagalong>();
               // this.gameObject.disableBehaviour<MrPP.Helper.SimpleTagalong>();
               // this.gameObject.disableBehaviour<TapToPlace>();
               // this.gameObject.disableBehaviour<TapToPlaceOnce>();
                //this.gameObject.disableBehaviour<MrPP.Helper.TapToPlaceScene>();

            }
            // LocalPlayer.
        }
    }
}