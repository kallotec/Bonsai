using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonsai.Framework
{
    public class EventBusSubscription
    {
        public string SubscriptionId { get; set; }
        public string EventName { get; set; }
        public Action Action { get; set; }
    }

    public class EventBus
    {
        public EventBus()
        {
            subscriptions = new List<EventBusSubscription>();
            notifications = new Queue<string>();
        }

        List<EventBusSubscription> subscriptions;
        Queue<string> notifications;


        public string Subscribe(string eventName, Action action)
        {
            var subId = Guid.NewGuid().ToString();

            subscriptions.Add(new EventBusSubscription
            {
                SubscriptionId = subId,
                EventName = eventName,
                Action = action,
            });

            return subId;
        }

        public void QueueNotification(string eventName)
        {
            if (!subscriptions.Any(s => s.EventName == eventName))
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

                var subsForEvent = subscriptions.Where(s => s.EventName == eventName).ToArray();

                for (var x = 0; x < subsForEvent.Length; x++)
                {
                    try
                    {
                        subsForEvent[x].Action();
                    }
                    catch 
                    {
                        throw;
                    }
                }

                    
            }

        }

        public void Unsubscribe(params string[] subscriptionIds)
        {
            subscriptions.RemoveAll(s => subscriptionIds.Contains(s.SubscriptionId));
        }

        public void Unsubscribe(List<string> subscriptionIds) => Unsubscribe(subscriptionIds.ToArray());

    }
}
