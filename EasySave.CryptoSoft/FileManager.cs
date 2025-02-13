using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoSoft
{
    public class FileManager
    {

        private string FilePath { get; set; }
        private string Key { get; set; }

        public FileManager(string filePath, string key)
        {
            FilePath = filePath; 
            Key = key;
        }
        /// <summary>
        /// check if the file exists
        /// </summary>
        private bool CheckFile()
        {
            if (File.Exists(FilePath))
                return true;

            Console.WriteLine("File not found.");
            Thread.Sleep(1000);
            return false;
        }

        /// <summary>
        /// Encrypts the file with xor encryption
        /// </summary>
        public int TransformFile()
        {
            if (!CheckFile()) return -1;
            Stopwatch stopwatch = Stopwatch.StartNew();
            var fileBytes = File.ReadAllBytes(FilePath);
            var keyBytes = ConvertToByte(Key);
            fileBytes = XorMethod(fileBytes, keyBytes);
            File.WriteAllBytes(FilePath, fileBytes);
            stopwatch.Stop();
            return (int)stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Convert a string in byte array
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static byte[] ConvertToByte(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        /// <summary>
        /// </summary>
        /// <param name="fileBytes">Bytes of the file to convert</param>
        /// <param name="keyBytes">Key to use</param>
        /// <returns>Bytes of the encrypted file</returns>
        private static byte[] XorMethod(IReadOnlyList<byte> fileBytes, IReadOnlyList<byte> keyBytes)
        {
            var result = new byte[fileBytes.Count];
            for (var i = 0; i < fileBytes.Count; i++)
            {
                result[i] = (byte)(fileBytes[i] ^ keyBytes[i % keyBytes.Count]);
            }

            return result;
        }
    }
}
