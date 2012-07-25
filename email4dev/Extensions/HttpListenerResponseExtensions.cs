using System.Net;
using System.Text;

namespace email4dev.Extensions
{
    public static class HttpListenerResponseExtensions
    {
        public static void WriteContent(this HttpListenerResponse response, string body)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(body);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Flush();
                response.OutputStream.Close();
            } catch(HttpListenerException)
            {
                // if the client disconnect while we are sending the response, a HttpListenerException will be thrown
            }
            
        }

    }
}