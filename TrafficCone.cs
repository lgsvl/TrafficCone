/**
 * Copyright (c) 2020 LG Electronics, Inc.
 *
 * This software contains code licensed as described in LICENSE.
 *
 */

using System.Collections.Generic;
using UnityEngine;

namespace Simulator.Controllable
{
    public class TrafficCone : MonoBehaviour, IControllable
    {
        public bool Spawned { get; set; }
        public string UID { get; set; }
        public string ControlType { get; set; } = "cone";
        public string CurrentState { get; set; }
        public string[] ValidStates { get; set; } = new string[] { };
        public string[] ValidActions { get; set; } = new string[] { };
        public string DefaultControlPolicy { get; set; } = "";
        public string CurrentControlPolicy { get; set; }

        public string GUID => UID;

        private void Awake()
        {
            CurrentControlPolicy = DefaultControlPolicy;
            CurrentState = "";
        }

        protected void OnDestroy()
        {
            Resources.UnloadUnusedAssets();
        }

        public void Control(List<ControlAction> controlActions)
        {
            for (int i = 0; i < controlActions.Count; i++)
            {
                var action = controlActions[i].Action;
                var value = controlActions[i].Value;

                switch (action)
                {
                    default:
                        Debug.LogError($"'{action}' is an invalid action for '{ControlType}'");
                        break;
                }
            }
        }
    }
}
