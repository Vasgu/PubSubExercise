using ClientApp.Services;
using CommonLib.Models;
using CommonLib.Resources;

namespace ClientApp
{
    internal static class Program
    {
        private static void Main()
        {
            while (true)
                try
                {
                    var choice = GetChoice();

                    switch (choice)
                    {
                        case (int)RequestType.Subscribe:
                            {
                                Console.WriteLine($"Please write the {AttributeName.ChannelName}");
                                var channelName = Console.ReadLine() ?? string.Empty;
                                Console.WriteLine($"Please write the {AttributeName.SubscriberName}");
                                var subName = Console.ReadLine() ?? string.Empty;
                                if (string.IsNullOrWhiteSpace(channelName) || string.IsNullOrWhiteSpace(subName))
                                {
                                    Console.WriteLine("Empty strings are not allowed. Retry.");
                                    continue;
                                }

                                using var op = new ClientOperations();
                                op.Connect(Configurations.LocalIp, Configurations.Port);
                                op.Execute(new SubscribeCommand(channelName, subName));
                                continue;
                            }
                        case (int)RequestType.Publish:
                            {
                                Console.WriteLine($"Please write the {AttributeName.ChannelName}");
                                var channelName = Console.ReadLine() ?? string.Empty;
                                Console.WriteLine($"Please write the {AttributeName.Message}");
                                var message = Console.ReadLine() ?? string.Empty;
                                if (channelName == string.Empty || message == string.Empty)
                                {
                                    Console.WriteLine("Empty strings are not allowed. Retry.");
                                    continue;
                                }

                                using var op = new ClientOperations();
                                op.Connect(Configurations.LocalIp, Configurations.Port);
                                op.Execute(new PublishCommand(channelName, message));
                                continue;
                            }
                        case (int)RequestType.Exit:
                            Console.WriteLine("Exiting...");
                            return;
                        default:
                            Console.WriteLine($"'{choice}' is not a valid choice.");
                            continue;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
        }

        public static int GetChoice()
        {
            Console.WriteLine("Select an operation:");
            Console.WriteLine($" {(int)RequestType.Subscribe} for subscribe");
            Console.WriteLine($" {(int)RequestType.Publish} for pubilsh");
            Console.WriteLine($" {(int)RequestType.Exit} for exit");

            var input = Console.ReadKey();
            Console.WriteLine();

            if (char.IsDigit(input.KeyChar))
            {
                return int.Parse(input.KeyChar.ToString());
            }

            Console.WriteLine($"'{input.KeyChar}' is not a valid choice.");
            return GetChoice();
        }
    }
}