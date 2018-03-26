using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ToolClass.ProcessInstance
{
    public class ProcessInstance
    {
        /// <summary>
        /// 判断某进程是否在运行中
        /// </summary>
        /// <param name="process">进程名字,不包括".exe"扩展名或路径</param>
        /// <returns></returns>
        public static bool IsRuning(string process)
        {
            if (Process.GetProcessesByName(process).ToList().Count > 0)
            {
                return true; //存在
            }
            else
                return false; //不存在
        }

        /// <summary>
        /// 结束进程
        /// </summary>
        /// <param name="process">进程名字,不包括".exe"扩展名或路径</param>
        /// <param name="isWindow">是否为图形界面程序</param>
        public static void CloseProcess(string process, bool isWindow)
        {
            Process[] processes = Process.GetProcessesByName(process);
            foreach (Process pro in processes)
            {
                try
                {
                    if (!pro.HasExited)
                    {
                        switch (isWindow)
                        {
                            case true:
                                try
                                {
                                    pro.CloseMainWindow();
                                }
                                catch { pro.Kill(); }
                                break;

                            case false:
                                try
                                {
                                    pro.Kill();
                                }
                                catch { }
                                break;
                        }
                    }
                }
                catch { }
            }

        }

    }
}
