using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using email4dev.Extensions;

namespace email4dev.HttpListener
{
    public sealed class EmailHttpListener : IDisposable
    {
        private RequestHandler _requestHandler;

        private System.Net.HttpListener _httpListener;
        private readonly string PORT_KEY = "EmailHttpListenerPort";
        public IEnumerable<string> UriAddresses { get; private set; }
        private string _emailDirectory;
        public bool IsConfigured { private set; get; }

        private void Configure()
        {
            IList<string> urls;
            if (TryBuildUrl(out urls))
            {
                UriAddresses = urls;
                _httpListener = new System.Net.HttpListener();
                urls.ToList().ForEach(x => _httpListener.Prefixes.Add(x));
                IsConfigured = true;
            }

            var smtpClient = email4dev.BackupSmtpClientBuilder.Build();
            _emailDirectory = smtpClient.PickupDirectoryLocation;
            if (!string.IsNullOrEmpty(_emailDirectory) && !Directory.Exists(_emailDirectory))
            {
                Directory.CreateDirectory(_emailDirectory);
            }
            _requestHandler = new RequestHandler(_emailDirectory);
        }

        private bool TryBuildUrl(out IList<string> urls)
        {
            int port;
            urls = new List<string>();
            if (int.TryParse(ConfigurationManager.AppSettings[PORT_KEY], out port))
            {
                var b = new UriBuilder()
                {
                    Host = Dns.GetHostName(),
                    Port = port
                };
                urls.Add(b.Uri.ToString());

                b = new UriBuilder()
                {
                    Host = "localhost",
                    Port = port
                };
                urls.Add(b.Uri.ToString());

                b = new UriBuilder()
                {
                    Host = Dns.GetHostByName(Dns.GetHostName()).AddressList.First().ToString(),
                    Port = port
                };
                urls.Add(b.Uri.ToString());
                return true;
            }
            return false;
        }

        public void Start()
        {
            Configure();

            if (IsConfigured)
            {
                _httpListener.Start();

                while (_httpListener.IsListening)
                    ProcessRequest();
            }

        }

        public void Stop()
        {
            if (IsConfigured)
            {
                _httpListener.Stop();
            }
        }

        void ProcessRequest()
        {
            var result = _httpListener.BeginGetContext(ListenerCallback, _httpListener);
            result.AsyncWaitHandle.WaitOne();
        }

        void ListenerCallback(IAsyncResult result)
        {
            var context = _httpListener.EndGetContext(result);
            var info = Read(context.Request);
            var body = string.Empty;
            try
            {
                body = _requestHandler.Process(info.Url);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.StatusDescription = HttpStatusCode.OK.ToString();
            }
            catch (Exception e)
            {
                body = e.Message;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.StatusDescription = HttpStatusCode.InternalServerError.ToString();
            }

            context.Response.WriteContent(body);
        }


        public static WebRequestInfo Read(HttpListenerRequest request)
        {
            var info = new WebRequestInfo();
            info.HttpMethod = request.HttpMethod;
            info.Url = request.Url;

            if (request.HasEntityBody)
            {
                Encoding encoding = request.ContentEncoding;
                using (var bodyStream = request.InputStream)
                using (var streamReader = new StreamReader(bodyStream, encoding))
                {
                    if (request.ContentType != null)
                        info.ContentType = request.ContentType;

                    info.ContentLength = request.ContentLength64;
                    info.Body = streamReader.ReadToEnd();
                }
            }

            return info;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
