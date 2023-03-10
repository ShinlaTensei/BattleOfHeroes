using System.Collections;
using System.Collections.Generic;
using Base.Logging;
using UnityEngine;

namespace Base
{
    public class DebugMono : MonoBehaviour
    {
        [SerializeField] private FPSDisplay fpsDisplay;
        
        #region Debug
        [DebugAction("Show FPS", "Analytics", SceneName.AnyScene)]
        public void ToggleFPS()
        {
            if (!fpsDisplay.ShowFps && fpsDisplay.Debug)
            {
                fpsDisplay.ShowFps = true;
                fpsDisplay.ShowMemory = false;

                return;
            }
            fpsDisplay.Debug = !fpsDisplay.Debug;
            fpsDisplay.ShowFps = true;
            fpsDisplay.ShowMemory = false;
        }
        [DebugAction("Show Memory", "Analytics", SceneName.AnyScene)]
        public void ToggleMemory()
        {
            if (!fpsDisplay.ShowMemory && fpsDisplay.Debug)
            {
                fpsDisplay.ShowFps = false;
                fpsDisplay.ShowMemory = true;

                return;
            }
            fpsDisplay.Debug = !fpsDisplay.Debug;
            fpsDisplay.ShowFps = false;
            fpsDisplay.ShowMemory = true;
        }
        
        [DebugAction("Show runtime hardware info", "Analytics", SceneName.AnyScene)]
        public void ShowDebug()
        {
            if (fpsDisplay.Debug && (!fpsDisplay.ShowFps || !fpsDisplay.ShowMemory))
            {
                fpsDisplay.ShowFps = true;
                fpsDisplay.ShowMemory = true;

                return;
            }
            fpsDisplay.Debug = !fpsDisplay.Debug;
            fpsDisplay.ShowFps = true;
            fpsDisplay.ShowMemory = true;
        }
        
        [DebugAction("TestParam", "Debug", SceneName.AnyScene)]
        public void TestParam(int a = 4)
        {
            
        }

        #endregion
    }
}
