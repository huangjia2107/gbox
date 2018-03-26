using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace G_Box
{
    public class RijndaelProcessor
    {
        static int bufferSize = 128 * 1024;
        static byte[] salt = { 134, 216, 7, 36, 88, 164, 91, 227, 174, 76, 191, 197, 192, 154, 200, 248 };
        static byte[] iv = { 134, 216, 7, 36, 88, 164, 91, 227, 174, 76, 191, 197, 192, 154, 200, 248 };

        static SymmetricAlgorithm CreateRijndael(string password, byte[] salt)
        {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, salt, "SHA256", 1000);
            SymmetricAlgorithm sma = Rijndael.Create();
            sma.KeySize = 256;
            sma.Key = pdb.GetBytes(32);
            sma.Padding = PaddingMode.PKCS7;
            return sma;
        }

        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="input">原文</param>
        /// <param name="password">对称密钥</param>
        /// <returns>密文</returns>
        public static string EncryptString(string input, string password)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (SymmetricAlgorithm algorithm = CreateRijndael(password, salt))
            {
                algorithm.IV = iv;
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, algorithm.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] bytes = Encoding.UTF32.GetBytes(input);
                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.Flush();
                }
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="input">密文</param>
        /// <param name="password">对称密钥</param>
        /// <returns>原文</returns>
        public static string DencryptString(string input, string password)
        {
            using (MemoryStream inputMemoryStream = new MemoryStream(Convert.FromBase64String(input)))
            using (SymmetricAlgorithm algorithm = CreateRijndael(password, salt))
            {
                algorithm.IV = iv;
                using (CryptoStream cryptoStream = new CryptoStream(inputMemoryStream, algorithm.CreateDecryptor(), CryptoStreamMode.Read))
                {

                    StreamReader sr = new StreamReader(cryptoStream, Encoding.UTF32);
                    return sr.ReadToEnd();
                }
            }
        }
    }

}
