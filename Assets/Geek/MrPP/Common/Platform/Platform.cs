using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


#if !UNITY_EDITOR && UNITY_WSA
using Windows.Networking;
using Windows.Networking.Connectivity;
#endif


namespace MrPP {
    public class Platform : GDGeek.Singleton<Platform> {
        public enum Type {
            HoloLens,
            ARKit,
            PC,
        }
        [SerializeField]
        private Type _type = Type.HoloLens;

        public Type type {

            get {
                return _type;
            }
        }

        private string localIp_;
        private string localComputerName_;

        private void Start()
        {
            
        }



        public static string LocalIp {

            get {

                if (string.IsNullOrEmpty(Platform.Instance.localIp_)) {
#if !UNITY_EDITOR && UNITY_WSA
                    // Find our local IP
                    foreach (HostName hostName in NetworkInformation.GetHostNames())
                    {
                        if (hostName.DisplayName.Split(".".ToCharArray()).Length == 4)
                        {
                            Debug.Log("Local IP " + hostName.DisplayName);
                            Platform.Instance.localIp_ =  hostName.DisplayName;
                        }
                    }
#else
                    Platform.Instance.localIp_=  Network.player.ipAddress;
#endif
                }
                return Platform.Instance.localIp_;
            }
        }
       
        public static string LocalComputerName
        {

            get
            {
                if (string.IsNullOrEmpty(Platform.Instance.localComputerName_))
                {
#if !UNITY_EDITOR && UNITY_WSA
                    foreach (HostName hostName in NetworkInformation.GetHostNames())
                    {
                        if (hostName.Type == HostNameType.DomainName)
                        {

                            Debug.Log("My name is " + hostName.DisplayName);
                            Platform.Instance.localComputerName_ = hostName.DisplayName;
                        }
                    }
                    Platform.Instance.localComputerName_ = "NotSureWhatMyNameIs";
#else
                    Platform.Instance.localComputerName_ =  System.Environment.MachineName;
#endif
                }

                return Platform.Instance.localComputerName_;

                

            }
        }


    }
}