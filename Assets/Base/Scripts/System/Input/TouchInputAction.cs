using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Base
{
    public class TouchInputAction : MonoBehaviour, InputAction
    {
        private bool m_lock;

        public InputPhase Phase
        {
            get
            {
                if (Input.touchCount == 1)
                {
                    if (Lock) return InputPhase.None;
                    
                    var touch = Input.touches[0];
                    if (EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    {
                        if (touch.phase == TouchPhase.Began) return InputPhase.Began;
                        if (touch.phase == TouchPhase.Moved) return InputPhase.Moved;
                        if (touch.phase == TouchPhase.Stationary) return InputPhase.Stationary;
                        if (touch.phase == TouchPhase.Ended) return InputPhase.Ended;
                    }
                }

                return InputPhase.None;
            }
        }

        public Vector3 Position => Input.touchCount == 1 ? (Vector3) Input.touches[0].position : Vector3.zero;

        public Vector3 DeltaPosition
        {
            get => Input.GetTouch(0).deltaPosition;
        }

        public bool Lock => m_lock;

        public void SetLock(bool isLock)
        {
            m_lock = isLock;
        }

        public Touch Touch => Input.GetTouch(0);
    }
}

