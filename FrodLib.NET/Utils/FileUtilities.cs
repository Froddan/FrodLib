using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrodLib.Utils
{
    public static class FileUtilities
    {
        public static bool IsFileClosed(string filepath, bool wait, int waitTime = 5000)
        {
            bool fileClosed = false;
            int retries = 10;
            int delay = waitTime / retries; // Max time spent here = retries*delay milliseconds

            if (!File.Exists(filepath))
                return false;

            do
            {
                try
                {
                    // Attempts to open then close the file in RW mode, denying other users to place any locks.
                    using (FileStream fs = File.Open(filepath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        fs.Close();
                        fileClosed = true; // success
                    }
                }
                catch (IOException) { }

                if (!wait) break;

                retries--;

                if (!fileClosed)
                    Thread.Sleep(delay);
            }
            while (!fileClosed && retries > 0);

            return fileClosed;
        }

        public static string GenerateMD5CheckSum(string filepath)
        {
            using (FileStream fileStream = File.OpenRead(filepath))
            {
                return GenerateMD5CheckSum(fileStream);
            }
        }

        public static string GenerateMD5CheckSum(this Stream stream)
        {
            long currPosition = stream.Position;
            stream.Position = 0;

            try
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] hash = md5.ComputeHash(stream);
                    string stringhash = System.BitConverter.ToString(hash).Replace("-", "");
                    return stringhash;
                }
            }
            finally
            {
                stream.Position = currPosition;
            }
        }
    }
}
