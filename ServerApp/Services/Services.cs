using ServerApp.Models;
using CommonLib.Models;


namespace ServerApp.Services
{
    public static class ServerOperations
    {
        public static void Execute(string payload, List<Channel> channels)
        {
            var command = CommandConverter.Deserialize(payload);

            if (command.ReqType == RequestType.Subscribe)
                Subscribe(command, channels);
            else if (command.ReqType == RequestType.Publish) Publish(command, channels);
        }

        private static void Subscribe(Command command, List<Channel> channels)
        {
            var channelName = command.Attributes.FirstOrDefault(x => x.Item1 == AttributeName.ChannelName).Item2;
            var subscriberName = command.Attributes.FirstOrDefault(x => x.Item1 == AttributeName.SubscriberName).Item2;

            var sub = new Subscriber(subscriberName);

            lock (channels)
            {
                var existingChannel = channels.FirstOrDefault(x => x.Name == channelName);

                if (existingChannel == null)
                {
                    var channel = new Channel(channelName);
                    channel.Subscribers.Add(sub);
                    channels.Add(channel);
                    Console.WriteLine($"Subscriber {subscriberName} subscribed to new channel {channelName}!");
                }
                else
                {
                    var existingSubscriber = existingChannel.Subscribers.FirstOrDefault(x => x.Name == subscriberName);

                    if (existingSubscriber != null)
                    {
                        Console.WriteLine($"Subscriber {subscriberName} is already subscribed to channel {channelName}!");
                    }
                    else
                    {
                        existingChannel.Subscribers.Add(sub);
                        Console.WriteLine($"Subscriber {subscriberName} subscribed to existing channel {channelName}!");
                    }
                }
            }
        }

        private static void Publish(Command command, List<Channel> channels)
        {
            var channelName = command.Attributes.FirstOrDefault(x => x.Item1 == AttributeName.ChannelName).Item2;
            var message = command.Attributes.FirstOrDefault(x => x.Item1 == AttributeName.Message).Item2;

            lock (channels)
            {
                var channel = channels.FirstOrDefault(x => x.Name == channelName);

                if (channel != null)
                    channel.Publish(message);
                else
                    Console.WriteLine($"Channel {channelName} not found!");
            }
        }
    }
}
