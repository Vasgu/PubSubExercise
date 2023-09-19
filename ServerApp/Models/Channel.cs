using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.Models
{
    public class Channel : IChannel<Subscriber>
    {
        public Channel(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public List<Subscriber> Subscribers { get; set; } = new();

        public void Publish(string message)
        {
            Subscribers.ForEach(sub => { sub.OnMessageReceive(message); });
        }
    }

    public interface IChannel<T> where T : ISubscriber
    {
        public string Name { get; set; }
        public List<T> Subscribers { get; set; }
        void Publish(string message);
    }
}
