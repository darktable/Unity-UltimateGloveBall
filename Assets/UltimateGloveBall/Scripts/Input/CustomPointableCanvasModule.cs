// Copyright (c) Meta Platforms, Inc. and affiliates.
// Use of the material below is subject to the terms of the MIT License
// https://github.com/oculus-samples/Unity-UltimateGloveBall/tree/main/Assets/UltimateGloveBall/LICENSE

using Oculus.Interaction;
using UnityEngine.EventSystems;
using UnityEngine.XR;

namespace UltimateGloveBall.Input
{
    /// <summary>
    /// Override of the PointableCanvasModule so that we can use the mouse pointer in editor when not headset is
    /// destected instead of using the pointable canvas module.
    /// </summary>
    public class CustomPointableCanvasModule : PointableCanvasModule
    {
        private XRDeviceFpsSimulator m_fpsSimulator;
        private bool m_fpsSimulatorAvailable;
        
        protected override void Start()
        {
            m_fpsSimulator = XRDeviceFpsSimulator.Instance;
            m_fpsSimulatorAvailable = m_fpsSimulator != null;
         
            base.Start();
        }
        
        protected override void ProcessMove(PointerEventData pointerEvent)
        {
            if (m_fpsSimulatorAvailable && m_fpsSimulator.IsActive)
            {
                var targetGO = pointerEvent.pointerCurrentRaycast.gameObject;
                HandlePointerExitAndEnter(pointerEvent, targetGO);
                return;
            }
            
            base.ProcessMove(pointerEvent);
        }

        public override bool IsModuleSupported()
        {
            if (m_fpsSimulatorAvailable && m_fpsSimulator.IsActive)
            {
                return base.IsModuleSupported();
            }

            return XRSettings.isDeviceActive && base.IsModuleSupported();
        }
    }
}