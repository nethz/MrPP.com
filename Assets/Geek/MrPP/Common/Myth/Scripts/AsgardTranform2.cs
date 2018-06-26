﻿using GDGeek;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace MrPP.Myth
{

    public class AsgardTranform2 : NetworkBehaviour, IMessageHandler
    {
        [SerializeField]
        private float _networkSendInterval = 0.1f;

        public override float GetNetworkSendInterval()
        {

            return _networkSendInterval;
        }


        [Serializable]
        public struct Data
        {
            [SerializeField]
            public Vector3 asgardPosition;
            [SerializeField]
            public Quaternion asgardRotation;
            [SerializeField]
            public Vector3 asgardScale;
        };
        [SerializeField]
        private bool _interpolation = true;

        [SerializeField]
        private Transform _transform;
        //private Data backup_;
        [SerializeField]
        [SyncVar(hook = "dataChange")]
        private Data _data;

        public void Awake()
        {

            if (_transform == null) {
                _transform = this.transform;
            }
        }

        public void Start()
        {
            if (Altar.AmIGod)
            {
                synchro();
                _transform.hasChanged = false;
                // sweep();
            }
        }

        void dataChange(Data data)
        {

            _data = data;
            if (!amIGod)
            {
               
                Yggdrasil.WorldPose wPose = Yggdrasil.Instance.getWorldPose(new Yggdrasil.AsgardPose(_data.asgardPosition, _data.asgardRotation, _data.asgardScale));
                
                if (_interpolation && Vector3.Distance(wPose.position, this.transform.position) < 0.3f)
                {
                    TweenTransformData.Begin(_transform.gameObject, 0.1f, wPose.position, wPose.rotation, wPose.scale);
                }
                else
                {
                    this._transform.setGlobalScale(wPose.scale);
                    this._transform.position = wPose.position;
                    this._transform.rotation = wPose.rotation;
                }

            }
        }


        private Data data
        {
            get
            {
                return _data;
            }
        }

        //private Data backup_;

        private bool amIGod
        {
            get
            {
                WhoIsGod wig = Altar.LocalComponent<WhoIsGod>();
                if (wig != null && !wig.isItMe())
                {
                    return false;
                }
                return true;

            }
        }

        public IMessageWriter getWriter()
        {
            MessageWriter writer = new MessageWriter();
            writer.onWriteTo += delegate (BinaryWriter bw)
            {
                Yggdrasil.AsgardPose pose = Yggdrasil.Instance.getAsgardPose(new Yggdrasil.WorldPose(this._transform));
                bw.Write(pose.asgardPosition);
                bw.Write(pose.asgardRotation);
                bw.Write(pose.asgardScale);
            };
            return writer;
        }

        public IMessageReader getRreader()
        {
            MessageReader reader = new MessageReader();
            reader.onReadFrom += delegate (BinaryReader br)
            {
                Data data = new Data();
                data.asgardPosition = br.ReadVector3();
                data.asgardRotation = br.ReadQuaternion();
                data.asgardScale = br.ReadVector3();
                _data = data;
            };
            return reader;
        }

        void Update()
        {
            if (amIGod && this._transform.hasChanged)
            {

                synchro();
                _transform.hasChanged = false;
              
            }

        }


        private void synchro()
        {

            BroadcastingStation bs = Altar.LocalComponent<BroadcastingStation>();
            if (bs)
            {
                bs.broadcasting(this);
            }
        }
    }
}