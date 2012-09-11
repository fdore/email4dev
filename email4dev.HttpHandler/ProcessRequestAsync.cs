using System;
using System.IO;
using System.Threading;
using System.Web;

namespace email4dev.HttpHandler
{
    internal class ProcessRequestAsync : IAsyncResult
    {
        private bool _completed;
        private readonly AsyncCallback _callback;
        private readonly HttpContext _context;
        private readonly Object _state;
        bool IAsyncResult.IsCompleted { get { return _completed; } }
        WaitHandle IAsyncResult.AsyncWaitHandle { get { return null; } }
        Object IAsyncResult.AsyncState { get { return _state; } }
        bool IAsyncResult.CompletedSynchronously { get { return false; } }
        private readonly string _emailDirectory;
        private readonly RequestHandler _requestHandler;

        public ProcessRequestAsync()
        {
            var smtpClient = BackupSmtpClientBuilder.Build();
            _emailDirectory = smtpClient.PickupDirectoryLocation;
            if (!string.IsNullOrEmpty(_emailDirectory) && !Directory.Exists(_emailDirectory))
            {
                Directory.CreateDirectory(_emailDirectory);
            }
            _requestHandler = new RequestHandler(_emailDirectory);
        }

        public ProcessRequestAsync(AsyncCallback callback, HttpContext context, Object state) : this()
        {
            _callback = callback;
            _context = context;
            _state = state;
            _completed = false;
        }

        public void StartAsyncWork()
        {
            ThreadPool.QueueUserWorkItem(StartAsyncTask, null);
        }

        private void StartAsyncTask(Object workItemState)
        {
            var url = _context.Request.Url;
            var body = _requestHandler.Process(url);
            _context.Response.Write(body);
            _completed = true;
            _callback(this);
        }
    }
}