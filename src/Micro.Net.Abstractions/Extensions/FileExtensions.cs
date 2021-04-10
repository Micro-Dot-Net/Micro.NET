using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Micro.Net.Core.Extensions
{
    public static class FileExtensions
    {
        public static FileStream WaitForFile(string fullPath, FileMode mode, FileAccess access, FileShare share, int retries = 10, int interval = 50)
        {
            for (int numTries = 0; numTries < retries; numTries++)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(fullPath, mode, access, share);
                    return fs;
                }
                catch (IOException)
                {
                    if (fs != null)
                    {
                        fs.Dispose();
                    }
                    Thread.Sleep(interval);
                }
            }

            return null;
        }
    }
}
