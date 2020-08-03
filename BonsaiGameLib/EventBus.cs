using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    public class EventBus
    {
        public EventBus()
        {
            subscriptions = new Dictionary<string, List<Action>>();
            notifications = new Queue<string>();
        }

        Dictionary<string, List<Action>> subscriptions;
        Queue<string> notifications;

        public void Subscribe(string eventName, Action action)
        {
            List<Action> actions = null;

            if (subscriptions.ContainsKey(eventName))
            {
                actions = subscriptions[eventName];
            }
            else
            { 
                actions = new List<Action>();
                subscriptions.Add(eventName, actions);
            }

            actions.Add(action);
        }

        public void QueueNotification(string eventName)
        {
            if (!subscriptions.ContainsKey(eventName))
                return;

            notifications.Enqueue(eventName);
        }

        public void FlushNotifications()
        {
            if (!notifications.Any())
                return;

            while (notifications.Any())
            {
                var eventName = notifications.Dequeue();

                if (!subscriptions.ContainsKey(eventName))
                    continue;

                var actions = subscriptions[eventName];

                foreach (var action in actions)
                    action();
            }


        }

    }
}
