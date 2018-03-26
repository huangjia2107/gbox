using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolClass.String
{
    public class GetMethods
    {
        public int codeCount=4;
        static int rep = 0;

        /// <summary>
        /// 产生随机codeCount位数字组合代码
        /// </summary>
        /// <param name="codeCount">组合位数</param>
        public string GetNum()
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                int num = random.Next();
                str = str + ((char)(0x30 + ((ushort)(num % 10)))).ToString();
            }
            return str;
        }

        /// <summary>
        /// 产生随机codeCount位字母组合代码
        /// </summary>
        /// <param name="codeCount">组合位数</param>
        public string GetStr()
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                int num = random.Next();
                str = str + ((char)(0x41 + ((ushort)(num % 0x1a)))).ToString();
            }
            return str.ToLower();
        }

        /// <summary>
        /// 产生随机codeCount位数字与字母组合代码
        /// </summary>
        /// <param name="codeCount">组合位数</param>
        public string GetStrAndNum()
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str.ToLower();
        }
    }
}
