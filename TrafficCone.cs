/**
 * Copyright (c) 2020 LG Electronics, Inc.
 *
 * This software contains code licensed as described in LICENSE.
 *
 */

using System.Net;
using System.Collections.Generic;
using UnityEngine;
using Simulator.Network.Core.Shared.Connection;
using Simulator.Network.Core.Shared.Messaging;
using Simulator.Network.Core.Shared.Messaging.Data;

namespace Simulator.Controllable
{
    public class TrafficCone : MonoBehaviour, IControllable, IMessageSender, IMessageReceiver
    {
        public bool Spawned { get; set; }
        public string UID { get; set; }
        public string Key => UID;
        public string ControlType { get; set; } = "cone";
        public string CurrentState { get; set; }
        public string[] ValidStates { get; set; } = new string[] { };
        public string[] ValidActions { get; set; } = new string[] { };
        public string DefaultControlPolicy { get; set; } = "";
        public string CurrentControlPolicy { get; set; }

        private MessagesManager messagesManager;

        private void Awake()
        {
            CurrentControlPolicy = DefaultControlPolicy;
            CurrentState = "";
        }

        private void Start()
        {
            messagesManager = SimulatorManager.Instance?.Network.MessagesManager;
            messagesManager?.RegisterObject(this);
        }

        private void OnDestroy()
        {
            messagesManager?.UnregisterObject(this);
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

            if (SimulatorManager.Instance.Network.IsMaster && controlActions.Count > 0)
            {
                //Forward control actions to clients
                var serializedControlActions = new BytesStack();
                for (var i = 0; i < controlActions.Count; i++)
                {
                    var controlAction = controlActions[i];
                    serializedControlActions.PushString(controlAction.Action);
                    serializedControlActions.PushString(controlAction.Value);
                }
                BroadcastMessage(new Message(Key, serializedControlActions, MessageType.ReliableOrdered));
            }
        }

        public void ReceiveMessage(IPeerManager sender, Message message)
        {
            var controlActions = new List<ControlAction>();
            while (message.Content.Count > 0)
            {
                var action = message.Content.PopString();
                var value = message.Content.PopString();
                controlActions.Add(new ControlAction()
                {
                    Action = action,
                    Value = value
                });
            }
            if (controlActions.Count > 0)
                Control(controlActions);
        }

        public void UnicastMessage(IPEndPoint endPoint, Message message)
        {
            messagesManager?.UnicastMessage(endPoint, message);
        }

        public void BroadcastMessage(Message message)
        {
            messagesManager?.BroadcastMessage(message);
        }

        void IMessageSender.UnicastInitialMessages(IPEndPoint endPoint)
        {
            //TODO support reconnection - send instantiation messages to the peer
        }
    }
}
