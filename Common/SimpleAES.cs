using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace Common
{
    /// <summary>
    /// 
    /// </summary>
    public class SimpleAES
    {
        // Change these keys
        private readonly byte[] Key = { 54, 51, 18, 121, 87, 61, 211, 42, 4, 163, 215, 61, 137, 12, 29, 156, 134, 77, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53, 209 };
        private readonly byte[] Vector = { 75, 88, 192, 113, 5, 178, 81, 90, 214, 76, 121, 132, 124, 57, 156, 198 };


        private readonly ICryptoTransform EncryptorTransform, DecryptorTransform;
        private readonly UTF8Encoding UTFEncoder;

        /// <summary>
        /// 
        /// </summary>
        public SimpleAES()
        {
            //This is our encryption method
            using (var rm = new RijndaelManaged())
            {
                //Create an encryptor and a decryptor using our encryption method, key, and vector.
                EncryptorTransform = rm.CreateEncryptor(Key, Vector);
                DecryptorTransform = rm.CreateDecryptor(Key, Vector);

                //Used to translate bytes to text and vice versa
                UTFEncoder = new UTF8Encoding();
            }
        }

        /// -------------- Two Utility Methods (not used but may be useful) -----------
        /// Generates an encryption key.
        public static byte[] GenerateEncryptionKey()
        {
            byte[] key;
            //Generate a Key.
            using (var rm = new RijndaelManaged())
            {
                rm.GenerateKey();
                key = rm.Key;
            }

            return key;
        }

        /// Generates a unique encryption vector
        public static byte[] GenerateEncryptionVector()
        {
            byte[] iv;

            //Generate a Vector
            using (var rm = new RijndaelManaged())
            {
                rm.GenerateIV();
                iv = rm.IV;
            }

            return iv;
        }


        /// ----------- The commonly used methods ------------------------------    
        /// Encrypt some text and return a string suitable for passing in a URL.
        public string EncryptToString(string textValue)
        {
            return ByteArrToString(Encrypt(textValue));
        }

        /// Encrypt some text and return an encrypted byte array.
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public byte[] Encrypt(string textValue)
        {
            //Translates our text value into a byte array.
            var bytes = UTFEncoder.GetBytes(textValue);
            byte[] encrypted;

            //Used to stream the data in and out of the CryptoStream.
            using (var memoryStream = new MemoryStream())
            {
                /*
                 * We will have to write the unencrypted bytes to the stream,
                 * then read the encrypted result back from the stream.
                 */
                using (var cs = new CryptoStream(memoryStream, EncryptorTransform, CryptoStreamMode.Write))
                {
                    #region Write the decrypted value to the encryption stream
                    cs.Write(bytes, 0, bytes.Length);
                    cs.FlushFinalBlock();
                    #endregion

                    #region Read encrypted value back out of the stream
                    memoryStream.Position = 0;
                    encrypted = new byte[memoryStream.Length];
                    memoryStream.Read(encrypted, 0, encrypted.Length);
                    #endregion
                }
            }

            return encrypted;
        }

        /// The other side: Decryption methods
        public string DecryptString(string encryptedString)
        {
            return Decrypt(StrToByteArray(encryptedString));
        }

        /// Decryption when working with byte arrays.    
        public string Decrypt(byte[] encryptedValue)
        {
            #region Write the encrypted value to the decryption stream
            var encryptedStream = new MemoryStream();
            var decryptStream = new CryptoStream(encryptedStream, DecryptorTransform, CryptoStreamMode.Write);
            decryptStream.Write(encryptedValue, 0, encryptedValue.Length);
            decryptStream.FlushFinalBlock();
            #endregion

            #region Read the decrypted value from the stream.
            encryptedStream.Position = 0;
            var decryptedBytes = new byte[encryptedStream.Length];
            encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);
            encryptedStream.Close();
            #endregion
            return UTFEncoder.GetString(decryptedBytes);
        }

        /// Convert a string to a byte array.  NOTE: Normally we'd create a Byte Array from a string using an ASCII encoding (like so).
        //      System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
        //      return encoding.GetBytes(str);
        // However, this results in character values that cannot be passed in a URL.  So, instead, I just
        // lay out all of the byte values in a long string of numbers (three per - must pad numbers less than 100).
        public byte[] StrToByteArray(string str)
        {
            if (str.Length == 0)
                throw new Exception("Invalid string value in StrToByteArray");

            var byteArr = new byte[str.Length / 3];
            var i = 0;
            var j = 0;
            do
            {
                var val = byte.Parse(str.Substring(i, 3));
                byteArr[j++] = val;
                i += 3;
            }
            while (i < str.Length);
            return byteArr;
        }

        /// <summary>
        /// Same comment as above.  Normally the conversion would use an ASCII encoding in the other direction:
        ///      System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
        ///      return enc.GetString(byteArr);    
        /// </summary>
        /// <param name="byteArr"></param>
        /// <returns></returns>
        public string ByteArrToString(byte[] byteArr)
        {
            var tempStr = "";
            for (var i = 0; i <= byteArr.GetUpperBound(0); i++)
            {
                var val = byteArr[i];
                if (val < 10)
                    tempStr += "00" + val;
                else if (val < 100)
                    tempStr += "0" + val;
                else
                    tempStr += val.ToString();
            }
            return tempStr;
        }
    }
}
