using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace KDS.Loader
{
    // Load a file and convert it into a byte[] array.
    // Instantiate the class to load several files and then combine into one byte[] array.
    public class FileLoader : ICodeLoader
    {
        List<string> _fileNames = new List<string>();

        public void AddFile(string fileName)
        {
            _fileNames.Add(fileName);
        }

        public byte[] LoadFiles()
        {
            byte[][] buffers = new byte[_fileNames.Count][];
            for (int i = 0; i < _fileNames.Count; i++ )
            {
                buffers[i] = LoadFile(_fileNames[i]);
            }

            return Combine(buffers);
        }

        private static byte[] Combine(params byte[][] arrays)
        {
            byte[] ret = new byte[arrays.Sum(x => x.Length)];
            int offset = 0;
            foreach (byte[] data in arrays)
            {
                Buffer.BlockCopy(data, 0, ret, offset, data.Length);
                offset += data.Length;
            }
            return ret;
        }

        public static byte[] LoadFile(string fileName)
        {
            byte[] buffer;
            FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read);

            try
            {
                int length = (int)fs.Length;
                buffer = new byte[length];
                int bytes_read;
                int total_bytes = 0;

                do
                {
                    bytes_read = fs.Read(buffer, total_bytes, length - total_bytes);
                    total_bytes += bytes_read;
                } while (bytes_read > 0);
            }
            finally
            {
                fs.Close();
            }
            return buffer;
        }
    }
}
