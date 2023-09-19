namespace ServerApp.Models
{
    public class Subscriber : ISubscriber
    {
        public Subscriber(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public void OnMessageReceive(string message)
        {
            Console.WriteLine($"{Name} has received: {message}");
        }
    }

    public interface ISubscriber
    {
        public string Name { get; set; }
        void OnMessageReceive(string message);
    }
}
