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

        private string FilePath { get; set; } // Path to the file to be encrypted
        private string Key { get; set; } // Encryption key

        public FileManager(string filePath, string key)
        {
            FilePath = filePath;
            Key = key;
        }

        /// <summary>
        /// Checks if the file exists before attempting encryption.
        /// </summary>
        /// <returns>True if the file exists, otherwise false.</returns>
        private bool CheckFile()
        {
            if (File.Exists(FilePath))
                return true;

            Console.WriteLine("File not found.");
           Thread.Sleep(1000);
            return false;
        }

        /// <summary>
        /// Encrypts or decrypts the file using XOR encryption.
        /// </summary>
        /// <returns>Time taken in milliseconds, or -1 if an error occurs.</returns>
        public int TransformFile()
        {
            try
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
            catch (IOException ex)
            {
                Console.WriteLine("An IO exception occurred: " + ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Converts a string into a byte array.
        /// </summary>
        /// <param name="text">The string to convert.</param>
        /// <returns>Byte array representation of the string.</returns>
        private static byte[] ConvertToByte(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        /// <summary>
        /// Applies XOR encryption to a byte array using a given key.
        /// </summary>
        /// <param name="fileBytes">Bytes of the file to encrypt or decrypt.</param>
        /// <param name="keyBytes">Encryption key in bytes.</param>
        /// <returns>Encrypted or decrypted byte array.</returns>
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