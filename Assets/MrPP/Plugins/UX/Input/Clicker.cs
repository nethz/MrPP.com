#if UNITY_WSA
using HoloToolkit.Unity.InputModule;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace MrPP
{
    namespace UX
    {
        public class Clicker : MonoBehaviour, GDGeek.IExecute
#if UNITY_WSA
            ,IInputClickHandler, IInputHandler
#endif
        {

            public UnityEvent OnClicked;
            public void execute()
            {
                if (OnClicked != null)
                {
                    OnClicked.Invoke();
                }
            }
#if UNITY_WSA
            public void OnInputClicked(InputClickedEventData eventData)
            {

                if (ready_) { 
                    execute();
                    ready_ = false;
                }

            }

            private bool ready_ = false;
            public void OnInputDown(InputEventData eventData)
            {
                ready_ = true;
            }
            public void OnInputUp(InputEventData eventData)
            {
            }
#else
            void OnMouseUpAsButton()
            {
                Debug.Log("click");
                execute();
            }
#endif
        }
    }
}