using ConfigAccess;
using MessageModule.MessageTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolClass.String;
using ToolClass.MD5;
using ToolClass.Mail;
using DataAccess.Access;
using DataAccess.Models;
using ToolClass.Logger;

namespace G_Box.LoginServer
{
    class DealMessage
    {
        #region 私有方法

        /// <summary>
        /// 发送信息
        /// </summary> 
        private void SendMsg(User user, string msg)
        {
            user.sw.WriteLine(RijndaelProcessor.EncryptString(msg, user.randomKey));
            user.sw.Flush();
        }

        #endregion 

        #region 处理方法

        /// <summary>
        /// 处理更新请求
        /// </summary> 
        /// <param name="message">版本号+更新时间</param>
        public void DealUpdate(User user, string message)
        {
            //版本号+更新时间
            string[] msg = message.Split(MessageTypes.NSP.ToCharArray());
            VersionModel versionModel = VersionData.GetConfig();
            ILogger ilogger = ILogger.GetInstance();

            ilogger.Logger(string.Format("用户:{0}--->比较软件版本...", user.client.Client.RemoteEndPoint));
            Console.WriteLine("用户:{0}--->比较软件版本...", user.client.Client.RemoteEndPoint);
            switch (string.Compare(msg[0], versionModel.AppVs, false))
            {
                //客户端版本低,可以更新
                case -1:
                    ilogger.Logger(string.Format("用户:{0}--->软件可以更新：{1}/{2}", user.client.Client.RemoteEndPoint, msg[0], versionModel.AppVs));
                    Console.WriteLine("用户:{0}--->软件可以更新：{1}/{2}", user.client.Client.RemoteEndPoint, msg[0], versionModel.AppVs);
                    SendMsg(user, MessageTypes.UPD + MessageTypes.PSP + "0" + MessageTypes.PSP + versionModel.AppVs + MessageTypes.NSP + versionModel.UpdateTime); 
                    break;

                //不用更新,进一步查看数据是否已更新
                case 0:
                    ilogger.Logger(string.Format("用户:{0}--->软件不用更新,比较数据更新日期...", user.client.Client.RemoteEndPoint));
                    Console.WriteLine("用户:{0}--->软件不用更新,比较数据更新日期...", user.client.Client.RemoteEndPoint);
                    switch (string.Compare(msg[1], versionModel.UpdateTime, false))
                    {
                        case -1:
                            ilogger.Logger(string.Format("用户:{0}--->数据可以更新：{1}/{2}", user.client.Client.RemoteEndPoint, msg[1], versionModel.UpdateTime));
                            Console.WriteLine("用户:{0}--->数据可以更新：{1}/{2}", user.client.Client.RemoteEndPoint, msg[1], versionModel.UpdateTime);
                            SendMsg(user, MessageTypes.UPD + MessageTypes.PSP + "0" + MessageTypes.PSP + MessageTypes.CUP); 
                            break;
                        case 0:
                            ilogger.Logger(string.Format("用户:{0}--->数据不用更新：{1}/{2}", user.client.Client.RemoteEndPoint, msg[1], versionModel.UpdateTime));
                            Console.WriteLine("用户:{0}--->数据不用更新：{1}/{2}", user.client.Client.RemoteEndPoint, msg[1], versionModel.UpdateTime);
                            SendMsg(user, MessageTypes.UPD + MessageTypes.PSP + "0" + MessageTypes.PSP + MessageTypes.NUP); 
                            break;
                        case 1:
                            ilogger.Logger(string.Format("用户:{0}--->数据更新时间异常：{1}/{2}", user.client.Client.RemoteEndPoint, msg[1], versionModel.UpdateTime));
                            Console.WriteLine("用户:{0}--->数据更新时间异常：{1}/{2}", user.client.Client.RemoteEndPoint, msg[1], versionModel.UpdateTime);
                            SendMsg(user, MessageTypes.UPD + MessageTypes.PSP + "0" + MessageTypes.PSP + MessageTypes.NUP); 
                            break;
                    }
                    break;

                //不用更新
                case 1:
                    ilogger.Logger(string.Format("用户:{0}--->版本异常：{1}/{2}", user.client.Client.RemoteEndPoint, msg[0], versionModel.AppVs));
                    Console.WriteLine("用户:{0}--->版本异常：{1}/{2}", user.client.Client.RemoteEndPoint, msg[0], versionModel.AppVs);
                    SendMsg(user, MessageTypes.UPD + MessageTypes.PSP + "0" + MessageTypes.PSP + MessageTypes.NUP); 
                    break;

            }
        }

        /// <summary>
        /// 处理登陆
        /// </summary> 
        /// <param name="message">账号+密码</param>
        public void DealLogin(User user, string message)
        {
            //账号+密码
            string[] msg = message.Split(MessageTypes.NSP.ToCharArray());
            ILogger ilogger = ILogger.GetInstance();

            UserInfo userInfo = null;

            //查看账号是否存在
            try
            {
                userInfo = Access.GetUserByCard(msg[0]);
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->登陆时查询账号异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                Console.WriteLine("用户:{0}--->登陆时查询账号异常：{1}", user.client.Client.RemoteEndPoint, ex.Message);
                SendMsg(user, MessageTypes.LOG + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,操作异常请重试！");

                return;
            }

            if (userInfo == null)
            {
                ilogger.Logger(string.Format("用户:{0}--->登陆账号不存在.", user.client.Client.RemoteEndPoint));
                Console.WriteLine("用户:{0}--->登陆账号不存在.", user.client.Client.RemoteEndPoint);
                SendMsg(user, MessageTypes.LOG + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,该账号不存在！");

                return;
            }

            if (userInfo.UserStatus == "Y")
            {
                ilogger.Logger(string.Format("用户:{0}--->该账号已登录.", user.client.Client.RemoteEndPoint));
                Console.WriteLine("用户:{0}--->该账号已登录.", user.client.Client.RemoteEndPoint);
                SendMsg(user, MessageTypes.LOG + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,该账号已登录！");

                return;
            }

            //查看密码是否匹配
            if (Md5Hash.VerifyMd5Hash(msg[1], userInfo.PassWord))
            {
                ilogger.Logger(string.Format("用户:{0}--->登陆成功.", user.client.Client.RemoteEndPoint));
                Console.WriteLine("用户:{0}--->登陆成功.", user.client.Client.RemoteEndPoint);

                //更新状态
                try
                {
                    Access.UpdateStatus_Y(msg[0]);
                }
                catch (Exception ex)
                {
                    ilogger.Logger(string.Format("用户:{0}--->登陆时修改状态异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                    Console.WriteLine("用户:{0}--->登陆时修改状态异常：{1}", user.client.Client.RemoteEndPoint, ex.Message);
                    SendMsg(user, MessageTypes.LOG + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,操作异常请重试！");

                    return;
                }

                ilogger.Logger(string.Format("用户:{0}--->更新在线状态为：Y.", user.client.Client.RemoteEndPoint));
                Console.WriteLine("用户:{0}--->更新在线状态为：Y.", user.client.Client.RemoteEndPoint);
                SendMsg(user, MessageTypes.LOG + MessageTypes.PSP + "1" + MessageTypes.PSP
                    + userInfo.UserID + MessageTypes.NSP + userInfo.CardWord + MessageTypes.NSP + userInfo.UserName + MessageTypes.NSP + userInfo.UserPicture);
            }
            else
            {
                ilogger.Logger(string.Format("用户:{0}--->密码错误.", user.client.Client.RemoteEndPoint));
                Console.WriteLine("用户:{0}--->密码错误.", user.client.Client.RemoteEndPoint);
                SendMsg(user, MessageTypes.LOG + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,密码输入有误！");
            }
        }

        /// <summary>
        /// 处理注册时所执行的请求注册码
        /// </summary> 
        /// <param name="message">邮箱</param>
        public void DealRNU(User user, string message)
        {
            ILogger ilogger = ILogger.GetInstance();
            int result = 0;

            try
            {
                result = Access.MailIsExist(message);
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->申请注册码时查询邮箱异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                Console.WriteLine("用户:{0}--->申请注册码时查询邮箱异常：{1}", user.client.Client.RemoteEndPoint, ex.Message);
                SendMsg(user, MessageTypes.REG + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,操作异常请重试！");

                return;
            }

            //该邮箱不存在，可以用于注册
            if (result == 0)
            {
                //得到邮箱
                user.userRMail = message;
                //得到注册码
                user.regNum = GetString.GetRandomCode(GetString.Code.StrAndNum, 4);

                Mail mail = Mail.GetInstance();
                if (mail.SendMail("G_Box_注册_注册码", user.userRMail, "你在G-Box中注册时所申请到的注册码为：" + user.regNum + ",请于本次程序运行期间完成注册, 关闭程序即失效!\n\n注：该邮件为系统自动发送,请勿回复."))
                {
                    ilogger.Logger(string.Format("用户:{0}--->已获取注册码:{1},并发送成功.", user.client.Client.RemoteEndPoint, user.regNum));
                    Console.WriteLine("用户:{0}--->已获取注册码:{1},并发送成功.", user.client.Client.RemoteEndPoint, user.regNum);
                    SendMsg(user, MessageTypes.REG + MessageTypes.PSP + "0" + MessageTypes.PSP + "注册码已经发到您的邮箱,请注意查收.");
                }
                else
                {
                    ilogger.Logger(string.Format("用户:{0}--->已获取注册码:{1},发送失败", user.client.Client.RemoteEndPoint, user.regNum));
                    Console.WriteLine("用户:{0}--->已获取注册码:{1},发送失败", user.client.Client.RemoteEndPoint, user.regNum);
                    SendMsg(user, MessageTypes.REG + MessageTypes.PSP + "0" + MessageTypes.PSP + "发送注册码到您的邮箱出现异常,请重试.");
                }

                // SendMsg(user, MessageTypes.REG + MessageTypes.PSP + "0" + MessageTypes.PSP + "您的注册码为：" + user.regNum);  

                return;
            }

            ilogger.Logger(string.Format("用户:{0}--->邮箱已被注册.", user.client.Client.RemoteEndPoint));
            Console.WriteLine("用户:{0}--->邮箱已被注册.", user.client.Client.RemoteEndPoint);
            SendMsg(user, MessageTypes.REG + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,当前邮箱已被注册！");
        }

        /// <summary>
        /// 处理注册
        /// </summary> 
        /// <param name="message">账号+昵称+密码+头像+邮箱+验证码</param>
        public void DealRegister(User user, string message)
        {
            //账号+昵称+密码+头像+邮箱+验证码
            string[] msg = message.Split(MessageTypes.NSP.ToCharArray());
            ILogger ilogger = ILogger.GetInstance();

            //邮箱和注册码匹配
            if (user.userRMail == msg[4] && user.regNum == msg[5])
            {
                //进行注册
                ilogger.Logger(string.Format("用户:{0}--->开始注册...", user.client.Client.RemoteEndPoint));
                Console.WriteLine("用户:{0}--->开始注册...", user.client.Client.RemoteEndPoint);

                int result = 0;

                try
                {
                    result = Access.CardIsExist(msg[0]);
                }
                catch (Exception ex)
                {
                    ilogger.Logger(string.Format("用户:{0}--->注册时查询账号异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                    Console.WriteLine("用户:{0}--->注册时查询账号异常：{1}", user.client.Client.RemoteEndPoint, ex.Message);
                    SendMsg(user, MessageTypes.REG + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,操作异常请重试！");

                    return;
                }

                //账号被注册
                if (result != 0)
                {
                    ilogger.Logger(string.Format("用户:{0}--->账号已被注册.", user.client.Client.RemoteEndPoint));
                    Console.WriteLine("用户:{0}--->账号已被注册.", user.client.Client.RemoteEndPoint);
                    SendMsg(user, MessageTypes.REG + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,该账号已被注册！");

                    return;
                }

                try
                {
                    UserInfo userInfo = new UserInfo() { CardWord = msg[0], UserName = msg[1], PassWord = Md5Hash.GetMd5Hash(msg[2]), UserPicture = msg[3], UserMail = msg[4], RegTime = DateTime.Now };
                    Access.InsertUser(userInfo);
                }
                catch (Exception ex)
                {
                    ilogger.Logger(string.Format("用户:{0}--->注册异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                    Console.WriteLine("用户:{0}--->注册异常：{1}", user.client.Client.RemoteEndPoint, ex.Message);
                    SendMsg(user, MessageTypes.REG + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,操作异常请重试！");

                    return;
                }

                ilogger.Logger(string.Format("用户:{0}--->注册成功...", user.client.Client.RemoteEndPoint));
                Console.WriteLine("用户:{0}--->注册成功...", user.client.Client.RemoteEndPoint);
                SendMsg(user, MessageTypes.REG + MessageTypes.PSP + "0" + MessageTypes.PSP + "恭喜您,注册成功！");

                return;
            }

            ilogger.Logger(string.Format("用户:{0}--->邮箱与注册码不匹配.", user.client.Client.RemoteEndPoint));
            Console.WriteLine("用户:{0}--->邮箱与注册码不匹配.", user.client.Client.RemoteEndPoint);
            SendMsg(user, MessageTypes.REG + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,邮箱与注册码不匹配！");
        }

        /// <summary>
        /// 处理重置密码时所执行的请求验证码
        /// </summary> 
        /// <param name="message">账号+邮箱</param>
        public void DealVNU(User user, string message)
        {
            //账号+邮箱
            string[] msg = message.Split(MessageTypes.NSP.ToCharArray());
            ILogger ilogger = ILogger.GetInstance();

            UserInfo userInfo = null;

            //查看账号是否存在
            try
            {
                userInfo = Access.GetUserByCard(msg[0]);
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->申请验证码时查询账号异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                Console.WriteLine("用户:{0}--->申请验证码时查询账号异常：{1}", user.client.Client.RemoteEndPoint, ex.Message);
                SendMsg(user, MessageTypes.RES + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,操作异常请重试！");

                return;
            }

            if (userInfo == null)
            {
                ilogger.Logger(string.Format("用户:{0}--->申请验证码时账号不存在.", user.client.Client.RemoteEndPoint));
                Console.WriteLine("用户:{0}--->申请验证码时账号不存在.", user.client.Client.RemoteEndPoint);
                SendMsg(user, MessageTypes.RES + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,该账号不存在！");

                return;
            }

            //查看邮箱是否匹配
            if (userInfo.UserMail == msg[1])
            {
                //得到账号
                user.cardWord = msg[0];
                //得到邮箱
                user.userVMail = msg[1];
                //得到验证码
                user.verNum = GetString.GetRandomCode(GetString.Code.StrAndNum, 4);

                Mail mail = Mail.GetInstance();
                if (mail.SendMail("G_Box_重置密码_验证码", user.userVMail, "你在G-Box中重置密码时所申请到的验证码为：" + user.verNum + ",请于本次程序运行期间完成重置密码, 关闭程序即失效!\n\n注：该邮件为系统自动发送,请勿回复."))
                { 
                    ilogger.Logger(string.Format("用户:{0}--->已获取验证码:{1},并发送成功.", user.client.Client.RemoteEndPoint, user.verNum));
                    Console.WriteLine("用户:{0}--->已获取验证码:{1},并发送成功.", user.client.Client.RemoteEndPoint, user.verNum);
                    SendMsg(user, MessageTypes.REG + MessageTypes.PSP + "0" + MessageTypes.PSP + "验证码已经发到您的邮箱,请注意查收.");
                }
                else
                {
                    ilogger.Logger(string.Format("用户:{0}--->已获取验证码:{1},发送失败", user.client.Client.RemoteEndPoint, user.verNum));
                    Console.WriteLine("用户:{0}--->已获取验证码:{1},发送失败", user.client.Client.RemoteEndPoint, user.verNum);
                    SendMsg(user, MessageTypes.REG + MessageTypes.PSP + "0" + MessageTypes.PSP + "发送验证码到您的邮箱出现异常,请重试.");
                }

                // SendMsg(user, MessageTypes.RES + MessageTypes.PSP + "0" + MessageTypes.PSP + "您的验证码为：" + user.verNum); 

                return;
            }

            ilogger.Logger(string.Format("用户:{0}--->该邮箱与账号不匹配.", user.client.Client.RemoteEndPoint));
            Console.WriteLine("用户:{0}--->该邮箱与账号不匹配.", user.client.Client.RemoteEndPoint);
            SendMsg(user, MessageTypes.RES + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,邮箱与账号不匹配！");
        }

        /// <summary>
        /// 重置密码
        /// </summary> 
        /// <param name="message">账号+新密码+邮箱+验证码</param>
        public void DealReset(User user, string message)
        {
            //账号+新密码+邮箱+验证码
            string[] msg = message.Split(MessageTypes.NSP.ToCharArray());
            ILogger ilogger = ILogger.GetInstance();

            if (user.cardWord != msg[0] || user.userVMail != msg[2] || user.verNum != msg[3])
            {
                ilogger.Logger(string.Format("用户:{0}--->重置密码时账号、邮箱、验证码三者不匹配.", user.client.Client.RemoteEndPoint));
                Console.WriteLine("用户:{0}--->重置密码时账号、邮箱、验证码三者不匹配.", user.client.Client.RemoteEndPoint);
                SendMsg(user, MessageTypes.RES + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,账号、邮箱、验证码三者不匹配！");

                return;
            }

            UserInfo userInfo = null;

            //查询账号,将该账号信息映射到类UserInfo
            try
            {
                userInfo = Access.GetUserByCard(msg[0]);
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->重置密码时查询账号异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                Console.WriteLine("用户:{0}--->重置密码时查询账号异常：{1}", user.client.Client.RemoteEndPoint, ex.Message);
                SendMsg(user, MessageTypes.RES + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,操作异常请重试！");

                return;
            }

            //修改密码,然后将UserInfo映射回数据库
            if (userInfo != null)
            {
                userInfo.PassWord = Md5Hash.GetMd5Hash(msg[1]);

                try
                {
                    Access.UpdateUser(userInfo);
                }
                catch (Exception ex)
                {
                    ilogger.Logger(string.Format("用户:{0}--->重置密码更新异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                    Console.WriteLine("用户:{0}--->重置密码更新异常：{1}", user.client.Client.RemoteEndPoint, ex.Message);
                    SendMsg(user, MessageTypes.RES + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,操作异常请重试！");

                    return;
                }

                ilogger.Logger(string.Format("用户:{0}--->重置密码成功.", user.client.Client.RemoteEndPoint));
                Console.WriteLine("用户:{0}--->重置密码成功.", user.client.Client.RemoteEndPoint);
                SendMsg(user, MessageTypes.RES + MessageTypes.PSP + "0" + MessageTypes.PSP + "恭喜您,重置密码成功！");

                return;
            }
        }

        #endregion
    }
}
