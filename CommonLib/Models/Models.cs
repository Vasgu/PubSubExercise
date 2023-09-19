using System.Text;

namespace CommonLib.Models
{
    public class PublishCommand : Command
    {
        public PublishCommand(string channelName, string message)
            : base(RequestType.Publish, new List<(string, string)>
            {
            (AttributeName.ChannelName, channelName),
            (AttributeName.Message, message)
            })
        {
        }
    }

    public class SubscribeCommand : Command
    {
        public SubscribeCommand(string channelName, string subscribeName)
            : base(RequestType.Subscribe, new List<(string, string)>
            {
            (AttributeName.ChannelName, channelName),
            (AttributeName.SubscriberName, subscribeName)
            })
        {
        }
    }

    public class Command : ICommand
    {
        public Command()
        {
            Attributes = new List<(string, string)>();
        }

        public Command(RequestType reqType, List<(string, string)> attributes)
        {
            ReqType = reqType;
            Attributes = attributes;
        }

        public RequestType ReqType { get; set; }
        public List<(string, string)> Attributes { get; set; }
    }

    public static class CommandConverter
    {
        public static string Serialize<T>(T command) where T : ICommand
        {
            var request = new StringBuilder($"COMMAND:{command.ReqType};");

            foreach (var attribute in command.Attributes) request.Append($"{attribute.Item1}:{attribute.Item2};");

            return request + "/r/n";
        }

        public static Command Deserialize(string serializedString)
        {
            if (string.IsNullOrWhiteSpace(serializedString)) throw new Exception("serializedString cannot be null.");
            var command = new Command();
            var parts = serializedString.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var keyValue = part.Split(':');

                if (keyValue.Length != 2) continue;

                var key = keyValue[0].Trim();
                var value = keyValue[1].Trim();

                if (key.ToUpper() == "COMMAND")
                {
                    if (Enum.TryParse(value, out RequestType reqType)) command.ReqType = reqType;
                }
                else
                {
                    command.Attributes.Add((key, value));
                }
            }

            return command;
        }
    }

    public interface ICommand
    {
        public RequestType ReqType { get; set; }
        public List<(string, string)> Attributes { get; set; }
    }

    public enum RequestType
    {
        Subscribe,
        Publish,
        Exit
    }

    public static class AttributeName
    {
        public static string ChannelName = "ChannelName";
        public static string SubscriberName = "SubscriberName";
        public static string Message = "Message";
    }
}
