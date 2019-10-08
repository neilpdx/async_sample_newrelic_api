using NewRelic.Api.Agent;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = new CancellationTokenSource();
            var token = source.Token;

            var queue = new Queue(token);
            Console.Write("Hit enter...");
            Console.ReadLine();
            source.Cancel();
        }
    }

    public class Queue
    {
        public Queue(CancellationToken token)
        {
            Task.Run(() => BackgroundProcessQueue(token));
        }

        private async Task BackgroundProcessQueue(object state)
        {
            var token = (CancellationToken) state;
            while (!token.IsCancellationRequested)
            {
                await ProcessQueue(token);
            }
        }

        [Transaction]
        public async Task ProcessQueue(object state)
        {
            var token = (CancellationToken) state;
            if (token.IsCancellationRequested) return;
            await Enqueue();
        }

        [Trace]
        protected async Task Enqueue()
        {
            NewRelic.Api.Agent.NewRelic.AddCustomParameter("Key", "Value");
            await Task.Delay(10);
        }
    }
}
