using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolClass.String
{
    public class GetString
    {
        public enum Code{ NUM,STR,StrAndNum }

		// 重载,指定位数
        public static string GetRandomCode(Code codeType,int codeCount)
        {
            GetMethods getMethod=new GetMethods();
            getMethod.codeCount = codeCount;
            var addMethod = typeof(GetMethods).GetMethod(ActionInTable(codeType));
            return (string)addMethod.Invoke(getMethod,null);
        }
		
		//重载,默认4位
		public static string GetRandomCode(Code codeType)
        {
            GetMethods getMethod=new GetMethods();
            var addMethod = typeof(GetMethods).GetMethod(ActionInTable(codeType));
            return (string)addMethod.Invoke(getMethod,null);
        }

        static string ActionInTable(Code code)
        {
            string[] methods = { "GetNum", "GetStr", "GetStrAndNum" };
            return methods[(int)code];
        }
    }
}
