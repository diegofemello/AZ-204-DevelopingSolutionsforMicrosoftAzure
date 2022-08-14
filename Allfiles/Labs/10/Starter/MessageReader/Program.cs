using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace MessageReader
{
    public class Program
    {
        private static string storageConnectionString = "Endpoint=sb://sbnamespacedfm.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=hTOafNwuWdNM5+l36bl6kuqYrFhbKerIG+nlOzIugL4=";
        private static string queueName = "messagequeue";
        private static ServiceBusClient client;
        private static ServiceBusProcessor processor;

        private static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");
            await args.CompleteMessageAsync(args.Message);
        }

        private static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private static async Task Main()
        {
            client = new ServiceBusClient(storageConnectionString);
            processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());
            try
            {
                processor.ProcessMessageAsync += MessageHandler;
                processor.ProcessErrorAsync += ErrorHandler;

                await processor.StartProcessingAsync();
                Console.WriteLine("Wait for a minute and then press any key to end the processing");
                Console.ReadKey();

                Console.WriteLine("\nStopping the receiver...");
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
            finally
            {
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }
    }
}