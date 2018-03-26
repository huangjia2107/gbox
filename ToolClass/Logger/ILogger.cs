using log4net;
using log4net.Config;
using System;
using System.IO;

namespace ToolClass.Logger
{
    public class ILogger
    {
        private static ILogger instance = null;

        /// <summary>
        /// log对象，指向日志级别
        /// </summary>
        private readonly ILog _logger;

        private ILogger()
        {
            try
            {
                _logger = LogManager.GetLogger("loginfo");
                //指向log4net.config配置文件
                XmlConfigurator.Configure(new FileInfo(Environment.CurrentDirectory + @"\Config\log4net.config"));
            }
            catch {  }
            
        }

        //单例模式
        public static ILogger GetInstance()
        {
            if (instance == null)
                return new ILogger();

            return instance;
        }

        #region 日志记录

        public void Logger(string msg)
        {
            if (_logger != null)
                _logger.Info(msg);
        }

        #endregion
    }
}
