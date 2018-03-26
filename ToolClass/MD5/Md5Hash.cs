using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ToolClass.MD5
{
    public static class Md5Hash
    {
        /// <summary>
        /// 加密类型：账号与密码
        /// </summary>
        public enum Type { Card, Psw };

        /// <summary>
        /// Key
        /// </summary>
        static string[] hashkey = { "Zx2%^&.,Rtd-}<:6s3V", "Aa1@#$,.Klj+{>.45oP" };

        /// <summary>
        /// 默认密码MD5加密
        /// </summary>
        /// <param name="input">要加密的字符串</param>
        /// <param name="type">要加密的类型：账号或密码</param>
        /// <returns>MD5值</returns>
        public static string GetMd5Hash(string input)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                string hashCode = BitConverter.ToString(md5.ComputeHash(
                    UTF8Encoding.Default.GetBytes(input))).Replace("-", "") +
                    BitConverter.ToString(md5.ComputeHash(
                    UTF8Encoding.Default.GetBytes(hashkey[(int)Type.Psw]))).Replace("-", "");

                return BitConverter.ToString(md5.ComputeHash(
                    UTF8Encoding.Default.GetBytes(hashCode))).Replace("-", "");
            }
        }

        /// <summary>
        /// 账号或密码MD5加密
        /// </summary>
        /// <param name="input">要加密的字符串</param>
        /// <param name="type">要加密的类型：账号或密码</param>
        /// <returns>MD5值</returns>
        public static string GetMd5Hash(string input, Type type)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                string hashCode = BitConverter.ToString(md5.ComputeHash(
                    UTF8Encoding.Default.GetBytes(input))).Replace("-", "") +
                    BitConverter.ToString(md5.ComputeHash(
                    UTF8Encoding.Default.GetBytes(hashkey[(int)type]))).Replace("-", "");

                return BitConverter.ToString(md5.ComputeHash(
                    UTF8Encoding.Default.GetBytes(hashCode))).Replace("-", "");
            }
        }

        /// <summary>
        /// 检查字符MD5值是否匹配
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <param name="hash">MD5值</param>
        /// <param name="type">要比较的类型：密码</param>
        /// <returns>是否匹配</returns>
        public static bool VerifyMd5Hash(string input, string hash)
        {
            string hashOfInput = GetMd5Hash(input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return comparer.Compare(hashOfInput, hash) == 0 ? true : false;
        }

        /// <summary>
        /// 检查字符MD5值是否匹配
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <param name="hash">MD5值</param>
        /// <param name="type">要比较的类型：账号或密码</param>
        /// <returns>是否匹配</returns>
        public static bool VerifyMd5Hash(string input, string hash,Type type)
        {
            string hashOfInput = GetMd5Hash(input,type);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return comparer.Compare(hashOfInput, hash) == 0 ? true : false;
        }

    }
}
