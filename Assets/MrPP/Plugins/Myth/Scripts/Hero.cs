using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MrPP.Helper;
namespace MrPP.Myth {
    public class Hero : NetworkBehaviour
    {

        [SerializeField]
        private uint _netId;

        [SerializeField]
        [SyncVar(hook = "ipChanged")]
        private string _ip = string.Empty;

        
        private void ipChanged(string ip)
        {
            _ip = ip;

            Debug.Log(_ip.ToString());
            refreshMessage();
        }

        [SerializeField]
        [SyncVar(hook = "playerNameChanged")]
        private string _playerName = string.Empty;


        private void playerNameChanged(string playerName)
        {
            _playerName = playerName;
            Debug.Log("!!!!!" + playerName);
            refreshMessage();
        }


        private void refreshMessage() {
            this.gameObject.name = "Hero@" + _playerName ;
        }


        [SyncVar]
        private Vector3 asgardPosition_;

        public Vector3 asgardPosition
        {

            get
            {
                return asgardPosition_;
            }
        }
        [SyncVar]
        private Quaternion asgardRotation_;
        public Quaternion asgardRotation
        {

            get
            {
                return asgardRotation_;
            }
        }

        public override float GetNetworkSendInterval()
        {

            return 0.033f;
        }
        public override void OnStartClient() {
            refreshMessage();
        }

        [Command(channel = 1)]
        public void CmdPlayerMessage(string playerName, string ip)
        {
            
           
            _playerName = playerName;
            _ip = ip;
        }

        // <summary>
        /// Sets the localPosition and localRotation on clients.
        /// </summary>
        /// <param name="postion">the localPosition to set</param>
        /// <param name="rotation">the localRotation to set</param>
        [Command(channel = 1)]
        public void CmdTransform(Vector3 aPosition, Quaternion aRotation)
        {
            //MrPP.HoloDebug.Log("CmdTransform:" + asgardPosition.ToString(), Color.green);
            asgardPosition_ = aPosition;
            asgardRotation_ = aRotation;
        }

        private NetworkSystem networkDiscovery_ = null;
        private void Start()
        {

            networkDiscovery_ = NetworkSystem.Instance;
            if (isLocalPlayer)
            {
                
                string name = Platform.LocalComputerName;
                string ip = Platform.LocalIp;
                Debug.Log("!!!" + ip);
                CmdPlayerMessage(name, ip);
               
            }
            _netId = this.netId.Value;
            Debug.Log(_netId.ToString());
        }
       
        private void Update()
        {

            if (isLocalPlayer)
            {
                transform.position = Camera.main.transform.position;
                transform.rotation = Camera.main.transform.rotation;

                Yggdrasil.AsgardPose aPose = Yggdrasil.Instance.getAsgardPose(new Yggdrasil.WorldPose(this.transform));
                CmdTransform(aPose.asgardPosition, aPose.asgardRotation);
              


            }
            else
            {
                Yggdrasil.WorldPose wPost = Yggdrasil.Instance.getWorldPose(new Yggdrasil.AsgardPose(asgardPosition, asgardRotation, Vector3.one));
                transform.position = Vector3.Lerp(transform.position, wPost.position, 0.3f);
                transform.rotation = wPost.rotation;
            }
        }
    }
}