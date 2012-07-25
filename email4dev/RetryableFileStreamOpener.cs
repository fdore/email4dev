using System;
using System.IO;
using System.Threading;

namespace email4dev
{
    public class RetryableFileStreamOpener
    {
        public static ADODB.Stream OpenFileStream(FileInfo fileInfo, int retry)
        {
            var delay = 0;

            for (var i = 0; i < retry; i++)
            {
                try
                {
                    ADODB.Stream stream = new ADODB.StreamClass();
                    stream.Open(Type.Missing, ADODB.ConnectModeEnum.adModeUnknown, ADODB.StreamOpenOptionsEnum.adOpenStreamUnspecified, String.Empty, String.Empty);
                    stream.LoadFromFile(fileInfo.FullName);
                    stream.Flush();
                    return stream;
                }
                catch (FileNotFoundException)
                {
                    throw;
                }
                catch (IOException)
                {
                    delay += 100;
                    if (i == retry) throw;
                }

                Thread.Sleep(delay);
            }

            //We will never get here
            throw new IOException("Unable to open file - " + fileInfo.FullName);
        }
    }

}