﻿using GDGeek;
using MrPP.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace MrPP.Myth { 
    public class Ctrl : MonoBehaviour
    {
        public const string AnchorEvent = "anchor";
        public const string StartEvent = "start";
        public const string TrackingLostEvent = "lost";


        public const string GodEvent = "god";

        public const string TrackingFoundEvent = "found";
        public const string JoinEvent = "join";



        public UnityAction joinAction_ = null;

        public void Awake()
        {
            joinAction_ = new UnityAction(join);
            NetworkSystem.Instance.SessionListChanged.AddListener(joinAction_);
              
           

        }
        private CtrlInterface iface_ = null;
        private CtrlInterface iface {
            get {
                if (iface_ == null) {
                    iface_ = this.gameObject.GetComponentInChildren<CtrlInterface>();
                }
                return iface_;

            }

        }


        private void join()
        {
            
            iface.post(new FSMEvent(Ctrl.JoinEvent));
        }

     
        public void start()
        {
            iface.post(new FSMEvent(StartEvent));
        }
        public void anchor() {
            iface.post(new FSMEvent(Ctrl.AnchorEvent));
        }
        public void trackingLost() {
            iface.post(new FSMEvent(TrackingLostEvent, transform));

        }

        public void god(uint id)
        {
            iface.post(new FSMEvent(GodEvent, id));
        }
        public void trackingFound(Transform transform)
        {

            iface.post(new FSMEvent(TrackingFoundEvent, transform));
        }
    }
}