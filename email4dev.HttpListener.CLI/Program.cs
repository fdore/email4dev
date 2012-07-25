using System;
using System.Threading.Tasks;
using email4dev.HttpListener;

namespace Email.Diagnostic.CLI
{
    class Program
    {
        private static EmailHttpListener _server;

        static void Main(string[] args)
        {
            _server = new EmailHttpListener();
            Task.Factory.StartNew(_server.Start);

            Console.ReadLine();
            _server.Stop();
        }

    }
}
