using System;
using System.IO;
using email4dev.Extensions;

namespace email4dev
{
    public class EmlParser
    {
       
        public CDO.Message Load(Func<FileInfo, int, ADODB.Stream> openStreamFunc, FileInfo file)
        {
            return Load(openStreamFunc, file, 1);
        }

        public CDO.Message Load(Func<FileInfo, int, ADODB.Stream> openStreamFunc, FileInfo file, int retry)
        {
            CDO.Message msg = new CDO.MessageClass();
            ADODB.Stream stream = openStreamFunc(file, retry);
            msg.DataSource.OpenObject(stream, "_Stream");
            msg.DataSource.Save();
            return msg;
        }
    }
}
