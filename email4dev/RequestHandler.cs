using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using email4dev.Extensions;

namespace email4dev
{
    public class RequestHandler
    {
        static RequestHandler()
        {
            typeof(RequestHandler).Assembly.LoadDependencies();
        }

        private readonly string SUBJECT_KEY = "Subject";
        private readonly string RECIPIENT_KEY = "Recipient";
        private readonly string BODY_KEY = "Body";
        private readonly string TIME_SENT_KEY = "TimeSent";
        private readonly string _emailDirectory;

        public RequestHandler(string emailDirectory)
        {
            _emailDirectory = emailDirectory;
        }

        public string Process(Uri url)
        {
            NameValueCollection q = HttpUtility.ParseQueryString(url.Query);

            var subject = q[SUBJECT_KEY];
            var recipient = q[RECIPIENT_KEY];
            var bodyValue = q[BODY_KEY];
            var timeSent = q[TIME_SENT_KEY];
            DateTime dtTimeSent;
            DateTime.TryParse(timeSent, out dtTimeSent);
            Func<CDO.Message, bool> filterFunc = x => (subject != null && x.Subject.Contains(subject)) ||
                                                      (recipient != null && x.To.Contains(recipient)) ||
                                                      (bodyValue != null && x.HTMLBody.Contains(bodyValue));

            string body;
            int retry = 3;
            for (int i = 0; i < retry;i++ )
            {
                if (TryFindEmail(filterFunc, dtTimeSent, out body))
                {
                    return body;
                }
                Thread.Sleep(5000);
            }
            return "Email not found";
        }

        IEnumerable<FileSystemInfo> GetFiles(string emailDirectory)
        {
            var di = new DirectoryInfo(emailDirectory);
            return di.GetFileSystemInfos("*.eml");
        }


        private bool TryFindEmail(Func<CDO.Message, bool> filterEmail, DateTime? sentTime, out string body)
        {
            var files = GetFiles(_emailDirectory);
            var sortedFiles = files.Where(x => !sentTime.HasValue || x.CreationTime > sentTime.Value).OrderByDescending(f => f.CreationTime);
            body = string.Empty;
            foreach (var file in sortedFiles)
            {
                if (file != null)
                {
                    var emlParser = new EmlParser();
                    var msg = emlParser.Load(RetryableFileStreamOpener.OpenFileStream, new FileInfo(file.FullName), 3);
                    if (filterEmail(msg))
                    {
                        body = msg.HTMLBody;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
