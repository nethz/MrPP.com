// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
/*
#if !UNITY_EDITOR && UNITY_WSA
using Windows.Networking;
using Windows.Networking.Connectivity;
#endif
*/
namespace MrPP.Helper
{
    /// <summary>
    /// Inherits from UNet's NetworkDiscovery script. 
    /// Adds automatic anchor management on discovery.
    /// If the script detects that it should be the server then
    /// the script starts the anchor creation and export process.
    /// If the script detects that it should be a client then the 
    /// script kicks off the anchor ingestion process.
    /// </summary>
    /// 
    /*
    public class MrNetworkDiscovery : NetworkDiscovery
    {
        /// <summary>
        /// Enables the Singleton pattern for this script.
        /// </summary>
        private static MrNetworkDiscovery instance_;
        public static MrNetworkDiscovery Instance
        {
            get
            {
                MrNetworkDiscovery[] objects = FindObjectsOfType<MrNetworkDiscovery>();
                if (objects.Length != 1)
                {
                    Debug.LogFormat("Expected exactly 1 {0} but found {1}", typeof(MrNetworkDiscovery).ToString(), objects.Length);
                }
                else
                {
                    instance_ = objects[0];
                }
                return instance_;
            }
        }

        /// <summary>
        /// Class to track discovered session information.
        /// </summary>
        public class SessionInfo
        {
            public string name;
            public string ip;
        }

        /// <summary>
        /// Tracks if we are currently connected to a session.
        /// </summary>
        public bool connected
        {
            get
            {
                // We are connected if we are the server or if we aren't running discovery
                return (isServer || !running);
            }
        }

        /// <summary>
        /// Event raised when the list of sessions changes.
        /// </summary>
        public event EventHandler<EventArgs> SessionListChanged;

        /// <summary>
        /// Keeps track of current remote sessions.
        /// </summary>
        [HideInInspector]
        private Dictionary<string, SessionInfo> remoteSessions_ = new Dictionary<string, SessionInfo>();

        public Dictionary<string, SessionInfo> remoteSessions {
            get {
                return remoteSessions_;
            }
        }

        /// <summary>
        /// Event raised when connected or disconnected.
        /// </summary>
        public event EventHandler<EventArgs> ConnectionStatusChanged;

        /// <summary>
        /// Controls how often a broadcast should be sent to clients
        /// looking to join our session.
        /// </summary>
        /// 
        [SerializeField]
        private int _broadcastInterval = 1000;

        /// <summary>
        /// Keeps track of the IP address of the system that sent the 
        /// broadcast.  We will use this IP address to connect and 
        /// download anchor data.
        /// </summary>
        public string serverIp { get; private set; }

       

        /// <summary>
        /// Sanity checks that our scene has everything we need to proceed.
        /// </summary>
        /// <returns>true if we have what we need, false otherwise.</returns>
        private bool checkComponents()
        {

            if (NetworkManager.singleton == null)
            {
                Debug.Log("Need a NetworkManager in the scene");
                return false;
            }

            return true;
        }

        private void Start()
        {

            // Initializes NetworkDiscovery.
            Initialize();
            
            if (!checkComponents())
            {
                Debug.Log("Invalid configuration detected. Network Discovery disabled.");
                Destroy(this);
                return;
            }

            broadcastInterval = _broadcastInterval;
            // Add our computer name to the broadcast data for use in the session name.
            broadcastData = Platform.LocalComputerName + '\0';
            NetworkServer.Reset();
            // Start listening for broadcasts.
            StartAsClient();
        }
        

       

        /// <summary>
        /// If we haven't received a broadcast by the time this gets called
        /// we will start broadcasting and start creating an anchor.
        /// </summary>
        private void maybeInitAsServer()
        {
            StartCoroutine(initAsServer());
        }
        
        private IEnumerator initAsServer()
        {

            NetworkManager.singleton.serverBindToIP = true;
            NetworkManager.singleton.serverBindAddress = Platform.LocalIp;


            // StopBroadcast will also 'StopListening'
            StopBroadcast();

            // Work-around when building to the HoloLens with "Compile with .NET Native tool chain".
            // Need a frame of delay after StopBroadcast() otherwise clients won't connect.
            yield return null;

            // Starting as a 'host' makes us both a client and a server.
            // There are nuances to this in UNet's sync system, so do make sure
            // to test behavior of your networked objects on both a host and a client 
            // device.
            NetworkManager.singleton.StartHost();

            // Work-around when building to the HoloLens with "Compile with .NET Native tool chain".
            // Need a frame of delay between StartHost() and StartAsServer() otherwise clients won't connect.
            yield return null;

            // Start broadcasting for other clients.
            StartAsServer();


        }

        /// <summary>
        /// Called by UnityEngine when a broadcast is received. 
        /// </summary>
        /// <param name="fromAddress">When the broadcast came from</param>
        /// <param name="data">The data in the broad cast. Not currently used, but could
        /// be used for differentiating rooms or similar.</param>
        public override void OnReceivedBroadcast(string fromAddress, string data)
        {
            Debug.Log(data);
            serverIp = fromAddress.Substring(fromAddress.LastIndexOf(':') + 1);
           // SessionInfo sessionInfo;
            if (!remoteSessions_.ContainsKey(serverIp))
            {

                remoteSessions_.Add(serverIp, new SessionInfo() { ip = serverIp, name = data });
                signalSessionListEvent();
            }
        }


        /// <summary>
        /// Call to stop listening for sessions.
        /// </summary>
        public void stopListening()
        {
            StopBroadcast();
            remoteSessions.Clear();
        }

        /// <summary>
        /// Call to start listening for sessions.
        /// </summary>
        public void startListening()
        {
            stopListening();
            StartAsClient();
        }

        /// <summary>
        /// Call to join a session
        /// </summary>
        /// <param name="session">Information about the session to join</param>
        public void joinSession(SessionInfo session)
        {
            stopListening();
            // We have to parse the server IP to make the string friendly to the windows APIs.
            serverIp = session.ip;
            NetworkManager.singleton.networkAddress = serverIp;

            // And join the networked experience as a client.
            NetworkManager.singleton.StartClient();
            signalConnectionStatusEvent();
        }

        /// <summary>
        /// Call to create a session 
        /// </summary>
        /// <param name="sessionName">The name of the session if a name can't be calculated</param>
        public void startHosting(string sessionName)
        {
            stopListening();
            NetworkManager.singleton.serverBindToIP = true;
            NetworkManager.singleton.serverBindAddress = Platform.LocalIp;
         
            NetworkManager.singleton.StartHost();
            // Start broadcasting for other clients.
            StartAsServer();



            signalSessionListEvent();
            signalConnectionStatusEvent();
        }
      
        /// <summary>
        /// Called when sessions have been added or removed
        /// </summary>
        void signalSessionListEvent()
        {
            EventHandler<EventArgs> sessionListChanged = SessionListChanged;
            if (sessionListChanged != null)
            {
                sessionListChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when we have joined or left a session.
        /// </summary>
        void signalConnectionStatusEvent()
        {
            EventHandler<EventArgs> connectionEvent = this.ConnectionStatusChanged;
            if (connectionEvent != null)
            {
                connectionEvent(this, EventArgs.Empty);
            }
        }
     

    }
    */
}