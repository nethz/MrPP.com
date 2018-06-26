using GDGeek;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.Networking;
namespace MrPP {
    [RequireComponent(typeof(NetworkDiscoveryReceiver), typeof(NetworkManager))]
    public class NetworkSystem : GDGeek.Singleton<NetworkSystem> {



        private FSM fsm_ = new FSM();

        /// <summary>
        /// Class to track discovered session information.
        /// </summary>
        public class SessionInfo
        {
            public string name;
            public string ip;
        }
        /// <summary>
        /// Keeps track of current remote sessions.
        /// </summary>
        private Dictionary<string, SessionInfo> remoteSessions_ = new Dictionary<string, SessionInfo>();

        public Dictionary<string, SessionInfo> remoteSessions
        {
            get
            {
                return remoteSessions_;
            }
        }

        /// <summary>
        /// Called by UnityEngine when a broadcast is received. 
        /// </summary>
        /// <param name="fromAddress">When the broadcast came from</param>
        /// <param name="data">The data in the broad cast. Not currently used, but could
        /// be used for differentiating rooms or similar.</param>
        public void receivedBroadcast(string fromAddress, string data)
        {
            Debug.Log(fromAddress);
            Debug.Log(data);
            string serverIp = fromAddress.Substring(fromAddress.LastIndexOf(':') + 1);
            // SessionInfo sessionInfo;
            if (!remoteSessions_.ContainsKey(serverIp))
            {

                remoteSessions_.Add(serverIp, new SessionInfo() { ip = serverIp, name = data });
                if (SessionListChanged != null) {
                    SessionListChanged.Invoke();
                }
            }
        }


        public UnityEvent SessionListChanged;
        /// <summary>
        /// Event raised when connected or disconnected.
        /// </summary>
        public UnityEvent ConnectionStatusChanged;




        /// <summary>
        /// Keeps track of the IP address of the system that sent the 
        /// broadcast.  We will use this IP address to connect and 
        /// download anchor data.
        /// </summary>
        public SessionInfo server { get; private set; }


        [SerializeField]
        private NetworkDiscoveryReceiver _discovery;
        private void Awake()
        {
            if (_discovery == null)
            {
                _discovery = this.gameObject.GetComponent<NetworkDiscoveryReceiver>();
            }


            _discovery.onReceiver += receivedBroadcast;
        }
        private void OnDestroy()
        {
            _discovery.onReceiver -= receivedBroadcast;
        }
        public void host()
        {
            fsm_.post("host");
        }
        public void client()
        {
            fsm_.post("client");
        }
        private void Start()
        {
            fsm_.addState("host", hostState());
            fsm_.addState("client", clientState());
            fsm_.addState("join", joinState());
            fsm_.init("client");
            // Add our computer name to the broadcast data for use in the session name.
            _discovery.broadcastData = Platform.LocalComputerName + '\0';
           // NetworkServer.Reset();

           
        }

        private StateBase joinState()
        {
            State state = new State();
            state.onStart += delegate
            {
                NetworkManager.singleton.networkAddress = server.ip;

                // And join the networked experience as a client.
                NetworkManager.singleton.StartClient();


                if (ConnectionStatusChanged != null)
                {
                    ConnectionStatusChanged.Invoke();
                }
            };
            state.onOver += delegate
            {

                NetworkManager.singleton.StopClient();
            };
            return state;
        }

        private StateBase clientState()
        {
            State state = new State();
            state.onStart += delegate {
                remoteSessions_.Clear();
                // Initializes NetworkDiscovery.
                _discovery.Initialize();
                // Start listening for broadcasts.
                _discovery.StartAsClient();
            };
            state.addAction("join", delegate (FSMEvent evt)
            {
                SessionInfo session = (SessionInfo)(evt.obj);
                this.server = session;
                return "join";
            });
            state.addAction("host", "host");
            state.onOver += delegate
            {
                _discovery.StopBroadcast();
            };
            return state;
        }

        private StateBase hostState()
        {
            State state = new State();
            state.onStart += delegate {


                NetworkManager.singleton.serverBindToIP = true;
                NetworkManager.singleton.serverBindAddress = Platform.LocalIp;

                NetworkManager.singleton.StartHost();


                // Initializes NetworkDiscovery.
                _discovery.Initialize();
                // Start listening for broadcasts.
                _discovery.StartAsServer();


                if (SessionListChanged != null)
                {
                    SessionListChanged.Invoke();
                }
                if (ConnectionStatusChanged != null)
                {
                    ConnectionStatusChanged.Invoke();
                }


            };

            state.addAction("client", "client");
            state.onOver += delegate
            {
                NetworkManager.singleton.StopHost();
                _discovery.StopBroadcast();
            };
            return state;
        }


        /// <summary>
        /// Tracks if we are currently connected to a session.
        /// </summary>
        public bool connected
        {
            get
            {
                // We are connected if we are the server or if we aren't running discovery
               return (_discovery.isServer || !_discovery.running);
            }
        }

        public bool running {
            get {

                return _discovery.running;
            }

        }

        public void joinFirst() {
            Debug.Log(remoteSessions_.Count);
            if (remoteSessions_.Count > 0) { 
                SessionInfo[] list = remoteSessions_.Values.ToArray<SessionInfo>();
                
                join(list[0]);
            }
            //remoteSessions{ { } }.First().value
        }
        public void join(SessionInfo session)
        {
            fsm_.post("join", session);
        }

        /// <summary>
        /// Call to join a session
        /// </summary>
        /// <param name="session">Information about the session to join</param>
        private void joinSession(SessionInfo session)
        {
            //stopListening();
            // We have to parse the server IP to make the string friendly to the windows APIs.
            server = session;
            NetworkManager.singleton.networkAddress = server.ip;

            // And join the networked experience as a client.
            NetworkManager.singleton.StartClient();


            if (ConnectionStatusChanged != null)
            {
                ConnectionStatusChanged.Invoke();
            }
        }
      

    }
}