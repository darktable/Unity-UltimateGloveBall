using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;

namespace Meta.Utilities
{
    public class XRDeviceCameraRigRef : OVRCameraRigRef
    {
        private XRDeviceFpsSimulator m_fpsSimulator;
        private bool m_fpsSimulatorAvailable;
        
        protected override void OnEnable()
        {
            base.OnEnable();

            if (_started && m_fpsSimulatorAvailable)
            {
                m_fpsSimulator.DevicesUpdated += DevicesUpdated;
            }
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            
            if (_started && m_fpsSimulatorAvailable)
            {
                m_fpsSimulator.DevicesUpdated -= DevicesUpdated;
            }
        }

        protected override void Start()
        {
            m_fpsSimulator = XRDeviceFpsSimulator.Instance;
            m_fpsSimulatorAvailable = m_fpsSimulator != null;

            base.Start();
        }
        
        private void DevicesUpdated()
        {
            HandleInputDataDirtied(null);
        }

        public override OVRInput.Controller GetConnectedControllers()
        {
            if (m_fpsSimulatorAvailable && m_fpsSimulator.IsActive)
            {
                return OVRInput.Controller.LTouch | OVRInput.Controller.RTouch;
            }
            
            return base.GetConnectedControllers();
        }

        public override bool GetButton(OVRInput.Button mappingButton, OVRInput.Controller controllerMask)
        {
            if (m_fpsSimulatorAvailable && m_fpsSimulator.IsActive)
            {
                switch (mappingButton)
                {
                    case OVRInput.Button.PrimaryIndexTrigger:
                        if ((controllerMask & OVRInput.Controller.LTouch) > 0)
                        {
                            return m_fpsSimulator.LeftTriggerDown;
                        }

                        if ((controllerMask & OVRInput.Controller.RTouch) > 0)
                        {
                            return m_fpsSimulator.RightTriggerDown;
                        }

                        break;
                    case OVRInput.Button.PrimaryHandTrigger:
                        if ((controllerMask & OVRInput.Controller.LTouch) > 0)
                        {
                            return m_fpsSimulator.LeftGripDown;
                        }

                        if ((controllerMask & OVRInput.Controller.RTouch) > 0)
                        {
                            return m_fpsSimulator.RightGripDown;
                        }

                        break;
                }
            }

            return base.GetButton(mappingButton, controllerMask);
        }
    }
}
