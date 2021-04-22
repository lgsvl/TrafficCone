/**
 * Copyright (c) 2020-2021 LG Electronics, Inc.
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
        public string GUID => UID;
        public string ControlType { get; set; } = "cone";
        public string CurrentState { get; set; }
        public string[] ValidStates { get; set; } = new string[] { };
        public string[] ValidActions { get; set; } = new string[] { };
        public List<ControlAction> DefaultControlPolicy { get; set; } =
            new List<ControlAction>
            {
                new ControlAction { Action = "state", Value = "" }
            };

        public List<ControlAction> CurrentControlPolicy { get; set; }

        private void Awake()
        {
            CurrentControlPolicy = DefaultControlPolicy;
            Control(CurrentControlPolicy);
        }

        protected void OnDestroy()
        {
            Resources.UnloadUnusedAssets();
        }

        public void Control(List<ControlAction> controlActions)
        {
            foreach (var action in controlActions)
            {
                switch (action.Action)
                {
                    case "state":
                        CurrentState = action.Value;
                        break;
                    default:
                        Debug.LogWarning($"'{action.Action}' is an invalid action for '{ControlType}'");
                        break;
                }
            }
        }
    }
}
