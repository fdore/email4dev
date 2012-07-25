using System.Net;
using System.Text;

namespace email4dev.HttpListener.Web
{
    public class WebResponseInfo
    {
        public string Body { get; set; }
        public string ContentEncoding { get; set; }
        public long ContentLength { get; set; }
        public string ContentType { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("StatusCode {0} StatusDescripton {1}", StatusCode, StatusDescription));
            sb.AppendLine(string.Format("ContentType {0} ContentEncoding {1} ContentLength {2}", ContentType, ContentEncoding, ContentLength));
            sb.AppendLine(string.Format("Body {0}", Body));
            return sb.ToString();
        }
    }
}
