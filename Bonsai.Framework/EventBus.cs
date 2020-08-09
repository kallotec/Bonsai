using Microsoft.Xna.Framework;
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
        public Action<object> Action { get; set; }
    }

    public class QueuedNotification
    {
        public string EventName { get; set; }
        public object Parameter { get; set; }
    }

    public class EventBus : IUpdateable
    {
        public EventBus()
        {
            subscriptions = new List<EventBusSubscription>();
            notifications = new Queue<QueuedNotification>();
        }

        List<EventBusSubscription> subscriptions;
        Queue<QueuedNotification> notifications;
        public bool IsDisabled => false;


        public string Subscribe(string eventName, Action<object> action)
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

        public void QueueNotification(string eventName, object parameter = null)
        {
            if (!subscriptions.Any(s => s.EventName == eventName))
                return;

            notifications.Enqueue(new QueuedNotification
            {
                EventName = eventName,
                Parameter = parameter
            });
        }

        public void Unsubscribe(params string[] subscriptionIds)
        {
            subscriptions.RemoveAll(s => subscriptionIds.Contains(s.SubscriptionId));
        }

        public void Unsubscribe(List<string> subscriptionIds) => Unsubscribe(subscriptionIds.ToArray());

        public void Update(GameTime time)
        {
            flushNotifications();
        }

        void flushNotifications()
        {
            if (!notifications.Any())
                return;

            while (notifications.Any())
            {
                var notif = notifications.Dequeue();

                var subsForEvent = subscriptions.Where(s => s.EventName == notif.EventName).ToArray();

                for (var x = 0; x < subsForEvent.Length; x++)
                {
                    try
                    {
                        subsForEvent[x].Action(notif.Parameter);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

    }
}
