using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace OpenBots.Agent.Core.Utilities
{
    public static class DataFormatter
    {
        #region Data Compression
        /// <summary>
        /// Compresses the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string CompressString(string jobExecutionParams)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(jobExecutionParams);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        /// <summary>
        /// Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns></returns>
        public static string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }

        }
        #endregion Data Compression


        #region Data Encryption

        /// <summary>
        /// Encrypt the plain text to un-readable format.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="additionalEntropy">The additional entropy.</param>
        /// <returns></returns>
        public static string EncryptText(string plainText, string additionalEntropy)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            // Getting bytes of the string plainText
            var plainTextBytes = UTF8Encoding.UTF8.GetBytes(plainText);
            var entropyBytes = UTF8Encoding.UTF8.GetBytes(additionalEntropy);

            var encryptedBytes = ProtectedData.Protect(plainTextBytes, entropyBytes, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// Decrypt the encrypted/un-readable text back to the readable format.
        /// </summary>
        /// <param name="encryptedText">The encrypted text.</param>
        /// <param name="additionalEntropy">The additional entropy.</param>
        /// <returns></returns>
        public static string DecryptText(string encryptedText, string additionalEntropy)
        {
            try
            {
                var encryptedBytes = Convert.FromBase64String(encryptedText);
                var entropyBytes = UTF8Encoding.UTF8.GetBytes(additionalEntropy);
                var decryptedBytes = ProtectedData.Unprotect(encryptedBytes, entropyBytes, DataProtectionScope.CurrentUser);
                return UTF8Encoding.UTF8.GetString(decryptedBytes);
            }
            catch
            {
                return encryptedText;
            }
        }
        #endregion Data Encryption
    }
}
