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
        }

        Dictionary<string, List<Action>> subscriptions;


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

        public void Notify(string eventName)
        {
            if (!subscriptions.ContainsKey(eventName))
                return;

            var actions = subscriptions[eventName];

            foreach (var action in actions)
                action();
        }

    }
}
