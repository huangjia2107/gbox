
using DataAccess.Access;
using DataAccess.Models;
using MessageModule.MessageTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolClass.Logger;
using ToolClass.MD5;

namespace G_Box.Server
{
    class DealMessage
    {
        #region 变量

        /// <summary>
        /// log对象，指向日志级别
        /// </summary>
        readonly ILogger ilogger;

        /// <summary>
        /// 连接状态ViewModel
        /// </summary>
        ConnViewModel connViewModel;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ilogger">日志</param>
        public DealMessage(ILogger ilogger, ConnViewModel connViewModel)
        {
            this.ilogger = ilogger;
            this.connViewModel = connViewModel;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 发送信息
        /// </summary> 
        private void SendMsg(User user, string msg)
        {
            // ilogger.Logger(string.Format("发送的数据:{0}",msg));

            user.sw.WriteLine(RijndaelProcessor.EncryptString(msg, user.randomKey));
            user.sw.Flush();
        }

        /// <summary>
        /// 获取好友信息并发送
        /// </summary> 
        private void SendFriend(User user, int userid)
        {
            List<FriendInfo> friendList = null;

            try
            {
                friendList=Access.GetAllFriendByID(userid); 
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->发送好友时查询好友异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                SendMsg(user, MessageTypes.FRI + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,好友列表获取异常！");

                return;
            }

            if (friendList.Count == 0)
            {
                ilogger.Logger(string.Format("用户:{0}--->暂时无好友.", user.client.Client.RemoteEndPoint));
                return;
            }

            List<Friend> arraylist = new List<Friend>();

            foreach (FriendInfo friendInfo in friendList)
            {
                if (friendInfo.OneID == userid)
                {
                    arraylist.Add(new Friend { UserID = friendInfo.OtherID, UserGroup = friendInfo.OtherGroup });
                    // ilogger.Logger(string.Format("好友ID:{0}---组ID:{1}", friendInfo.OtherID, friendInfo.OtherGroup));
                }
                else
                {
                    arraylist.Add(new Friend { UserID = friendInfo.OneID, UserGroup = friendInfo.OneGroup });
                    // ilogger.Logger(string.Format("好友ID:{0}---组ID:{1}", friendInfo.OtherID, friendInfo.OtherGroup));
                }
            }

            List<UserInfo> userList = null;

            if (arraylist.Count != 0)
            {
                try
                {
                    userList = Access.GetAllUserByID(arraylist); 
                }
                catch (Exception ex)
                {
                    ilogger.Logger(string.Format("用户:{0}--->获取好友信息时获取异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                    SendMsg(user, MessageTypes.FRI + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,好友列表获取异常！");

                    return;
                }
            }

            if (userList.Count == 0)
                return;

            //发送好友信息给自己
            for (int i = 0; i < userList.Count; i++)
            {
                SendMsg(user, MessageTypes.FRI + MessageTypes.PSP + "1" + MessageTypes.PSP +
                    userList[i].UserID + MessageTypes.NSP + userList[i].CardWord + MessageTypes.NSP + userList[i].UserName + MessageTypes.NSP + userList[i].UserPicture + MessageTypes.NSP + userList[i].UserStatus + MessageTypes.NSP
                + arraylist[i].UserGroup);

                // ilogger.Logger(string.Format("好友信息:{0}-{1}-{2}-{3}-{4}", userList[i].UserID, userList[i].CardWord, userList[i].UserName, userList[i].UserStatus, arraylist[i].UserGroup));
            }

            //将上线信息发给自己的在线好友
            foreach (User u in connViewModel.AllUsers)
            {
                if (userList.Find((UserInfo ui) => { return ui.UserID == u.userID && ui.UserStatus == MessageTypes.Y; }) != null)
                    SendMsg(u, MessageTypes.UPL + MessageTypes.PSP + "1" + MessageTypes.PSP + user.userID);
            }

        }

        /// <summary>
        /// 获取成就信息并发送
        /// </summary> 
        private void SendAchieve(User user, int userid)
        {
            List<GameInfo> gameList = null;

            try
            {
                gameList = Access.GetAllGameByID(userid); 
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->获取用户游戏列表异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                SendMsg(user, MessageTypes.ACH + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,成就列表获取异常！");
                return;
            }

            if (gameList.Count == 0)
            {
                ilogger.Logger(string.Format("用户:{0}--->暂时无游戏成就.", user.client.Client.RemoteEndPoint));
                return;
            }

            List<AchieveInfo> achieveList = new List<AchieveInfo>();

            foreach (GameInfo gameInfo in gameList)
            {
                try
                {
                    achieveList.Add(Access.GetAllAchieveByID(gameInfo)); 
                }
                catch (Exception ex)
                {
                    ilogger.Logger(string.Format("用户:{0}--->获取用户成就异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                    SendMsg(user, MessageTypes.ACH + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,成就列表获取异常！");
                    return;
                }
            }

            if (achieveList.Count != 0)
            {
                for (int i = 0; i < achieveList.Count; i++)
                {
                    SendMsg(user, MessageTypes.ACH + MessageTypes.PSP + "1" + MessageTypes.PSP +
                        gameList[i].GameName + MessageTypes.NSP + achieveList[i].UserLevel + MessageTypes.NSP + achieveList[i].UserRank + MessageTypes.NSP + achieveList[i].UserAllScore + MessageTypes.NSP + achieveList[i].UserSingleScore);
                }
            }
        }

        /// <summary>
        /// 发送系统消息
        /// </summary> 
        private void SendMessage(User user)
        {
            List<NoticeInfo> noticeList = null;

            try
            {
                noticeList = Access.GetAllMsg(); 
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->获取消息时查询异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                SendMsg(user, MessageTypes.MSG + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,消息获取异常！");

                return;
            }

            if (noticeList.Count == 0)
            {
                ilogger.Logger(string.Format("用户:{0}--->暂时无消息.", user.client.Client.RemoteEndPoint));
                return;
            }

            foreach (NoticeInfo notice in noticeList)
            {
                SendMsg(user, MessageTypes.MSG + MessageTypes.PSP + "1" + MessageTypes.PSP +
                    notice.MsgType + MessageTypes.NSP + notice.MsgTitle + MessageTypes.NSP + notice.MsgContent + MessageTypes.NSP + notice.MsgPublish);
            }

        }

        /// <summary>
        /// 查看是否存在有人请求添加自己为好友
        /// </summary> 
        private void DealAddTo(User user, int userid)
        {
            List<UserInfo> userList = null;
            try
            {
                userList = Access.GetAddFromByAddTo(userid); 
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->查询addfriendinfo表记录时异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                // SendMsg(user, MessageTypes.ACH + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,成就列表获取异常！");
                return;
            }

            if (userList.Count != 0)
            {
                foreach (UserInfo userinfo in userList)
                {
                    SendMsg(user, MessageTypes.ASK + MessageTypes.PSP + "1" + MessageTypes.PSP + userinfo.UserID + MessageTypes.NSP + userinfo.CardWord);
                }
            }
        }

        /// <summary>
        /// 查看是否存在自己的请求且是否被处理了
        /// </summary> 
        private void DealAddFrom(User user, int userid)
        {
            List<UserInfo> userList = null;
            try
            {
                userList = Access.GetAddToByAddFrom(userid); 
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->查询addfriendinfo表记录时异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                // SendMsg(user, MessageTypes.ACH + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,成就列表获取异常！");
                return;
            }

            if (userList.Count != 0)
            {
                foreach (UserInfo userinfo in userList)
                {
                    SendMsg(user, MessageTypes.AGR + MessageTypes.PSP + "0" + MessageTypes.PSP + "用户" + userinfo.CardWord + "同意您的好友添加请求！");
                }
            }
        }

        #endregion

        #region 处理方法

        /// <summary>
        /// 处理连接请求
        /// </summary> 
        /// <param name="message">ID+账号</param>
        public void DealCON(User user, string message)
        {
            user.RunStatus = "开始连接大厅服务器.";
            //ID+账号
            string[] msg = message.Split(MessageTypes.NSP.ToCharArray());
             
            UserInfo userInfo = null;

            try
            {
                userInfo = Access.StatusIsY(msg[1]); 
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->连接时查询状态异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                SendMsg(user, MessageTypes.CON + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,服务器异常请重试！");

                return;
            }

            if (userInfo == null)
            {
                ilogger.Logger(string.Format("用户:{0}--->状态未改变.", user.client.Client.RemoteEndPoint));
                SendMsg(user, MessageTypes.CON + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,登陆异常请重试！");

                return;
            }

            user.RunStatus = "连接大厅服务器成功.";
            ilogger.Logger(string.Format("用户:{0}--->连接成功.", user.client.Client.RemoteEndPoint));
            user.cardWord = userInfo.CardWord;
            user.userID = userInfo.UserID;

            SendMsg(user, MessageTypes.CON + MessageTypes.PSP + "1" + MessageTypes.PSP + "恭喜您,连接成功！");

            user.RunStatus = "获取个人信息.";
            //发送个人信息
            SendMsg(user, MessageTypes.PER + MessageTypes.PSP + "1" + MessageTypes.PSP + userInfo.UserID + MessageTypes.NSP + userInfo.CardWord + MessageTypes.NSP + userInfo.UserName + MessageTypes.NSP + userInfo.UserPicture);

            user.RunStatus = "获取好友信息.";
            //发送好友 
            SendFriend(user, userInfo.UserID); //获取并发送好友列表

            user.RunStatus = "获取成就信息.";
            //发送成就信息
            SendAchieve(user, userInfo.UserID);

            user.RunStatus = "获取系统消息.";
            //发送系统消息
            SendMessage(user);

            user.RunStatus = "获取好友请求信息.";
            //查看是否存有人请求添加自己为好友
            DealAddTo(user, userInfo.UserID);

            //查看是否有自己的请求且是否被处理了
            DealAddFrom(user, userInfo.UserID);

        }

        /// <summary>
        /// 处理修改密码请求
        /// </summary> 
        /// <param name="message">账号+原密码+新密码</param>
        public void DealSET(User user, string message)
        {
            user.RunStatus = "修改密码.";

            string[] msg = message.Split(MessageTypes.NSP.ToCharArray());
             
            int result = 0; 

            try
            {
                result = Access.ResetPassword(msg[0], Md5Hash.GetMd5Hash(msg[1]), Md5Hash.GetMd5Hash(msg[2])); 
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->查找好友时查询好友异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                SendMsg(user, MessageTypes.SET + MessageTypes.PSP + "1" + MessageTypes.PSP + "很抱歉,查找好友异常！");

                return;
            }

            if (result == 0)
            {
                SendMsg(user, MessageTypes.SET + MessageTypes.PSP + "1" + MessageTypes.PSP + "很抱歉,原密码输入有误！");
                return;
            }

            SendMsg(user, MessageTypes.SET + MessageTypes.PSP + "1" + MessageTypes.PSP + "恭喜您,密码重置成功！");

        }

        /// <summary>
        /// 处理消息请求
        /// </summary> 
        /// <param name="message">用户ID</param>
        public void DealMSG(User user, string message)
        {
            user.RunStatus = "刷新系统消息."; 

            SendMessage(user);
        }

        /// <summary>
        /// 处理刷新好友列表请求
        /// </summary> 
        /// <param name="message">用户ID</param>
        public void DealFRI(User user, string message)
        {
            user.RunStatus = "刷新好友列表."; 
            //发送好友 
            SendFriend(user, int.Parse(message)); //获取并发送好友列表
        }

        /// <summary>
        /// 处理刷新成就列表请求
        /// </summary> 
        /// <param name="message">用户ID</param>
        public void DealACH(User user, string message)
        {
            user.RunStatus = "刷新成就信息."; 
            //发送成就信息
            SendAchieve(user, int.Parse(message));
        }

        /// <summary>
        /// 处理查找好友请求
        /// </summary> 
        /// <param name="message">账号或昵称</param>
        public void DealSFR(User user, string message)
        {
            user.RunStatus = "查找好友.";
             
            List<UserInfo> userList = null;

            try
            {
                userList = Access.GetUserByIDorName(message); 
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->查找好友时查询好友异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                SendMsg(user, MessageTypes.SFR + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,查找好友异常！");

                return;
            }

            if (userList.Count == 0)
            {
                SendMsg(user, MessageTypes.SFR + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,该用户不存在！");
                return;
            }

            foreach (UserInfo userInfo in userList)
            {
                SendMsg(user, MessageTypes.SFR + MessageTypes.PSP + "1" + MessageTypes.PSP +
                    userInfo.UserID + MessageTypes.NSP + userInfo.CardWord + MessageTypes.NSP + userInfo.UserName + MessageTypes.NSP + userInfo.UserPicture + MessageTypes.NSP + userInfo.UserStatus);
            }
        }

        /// <summary>
        /// 处理添加好友请求(甲添加乙)
        /// </summary>  
        /// <param name="message">甲ID+甲账号+乙ID</param>
        public void DealAFR(User user, string message)
        {
            user.RunStatus = "添加好友.";

            string[] msg = message.Split(MessageTypes.NSP.ToCharArray()); 
            int result = 0; 

            try
            {
                //判断addfriendinfo表中是否存在相同记录
                result = Access.IsExistAddFriendInfo(msg[0],msg[2]); 
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->添加好友时查询是否有重复记录异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                SendMsg(user, MessageTypes.AFR + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,添加好友异常请重试！");

                return;
            }

            if (result != 0)
                return; 
            
            try
            {
                //将记录请求插入addfriendinfo表中
                Access.AddFriendInfo(msg[0], msg[2]); 
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->添加好友时插入记录异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                SendMsg(user, MessageTypes.AFR + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,添加好友异常请重试！");

                return;
            }

            foreach (User u in this.connViewModel.AllUsers)
            {
                if (u.userID.ToString() == msg[2])
                {
                    SendMsg(u, MessageTypes.ASK + MessageTypes.PSP + "1" + MessageTypes.PSP + msg[0] + MessageTypes.NSP + msg[1]);
                    break;
                }
            }
        }

        /// <summary>
        /// 处理同意添加为好友请求（来自乙）
        /// </summary> 
        /// <param name="message">乙ID+甲ID</param>
        public void DealAGR(User user, string message)
        {
            user.RunStatus = "同意好友添加请求.";

            string[] msg = message.Split(MessageTypes.NSP.ToCharArray()); 
            int result = 0; 

            try
            {
                //修改添加表中的记录为“已同意”
                result = Access.UpdateAddFriendInfo(msg[1], msg[0]); 
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->添加好友时同意后插入好友表异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                //SendMsg(user, MessageTypes.AFR + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,添加好友异常请重试！");

                return;
            }

            if (result == 0)
                return;

            result = 0; 

            try
            {
                //查看好友映射表中是否已存在记录
                result = Access.IsExistFriendInfo(msg[0], msg[1]); 
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->添加好友时同意后查询好友表异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                return;
            }

            if (result != 0)
            {
                SendMsg(user, MessageTypes.AGR + MessageTypes.PSP + "0" + MessageTypes.PSP + "好友添加成功！");
                return;
            }
             
            try
            {
                //插入好友映射表
                Access.InsertFriendInfo(msg[0], msg[1]); 
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->添加好友时同意后插入好友表异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                //SendMsg(user, MessageTypes.AGR + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,添加好友异常请重试！");

                return;
            }

            //将好友信息发送至对向。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。
            List<UserInfo> userList = null; 

            try
            {
                userList = Access.GetUserInfoByID(msg[0], msg[1]); 
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->添加好友时获取用户信息异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                //SendMsg(user, MessageTypes.AFR + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,添加好友异常请重试！");

                return;
            }

            if (userList.Count != 0)
            {
                foreach (UserInfo userinfo in userList)
                {
                    if (userinfo.UserID.ToString() == msg[1]) //发给乙
                        SendMsg(user, MessageTypes.AFR + MessageTypes.PSP + "1" + MessageTypes.PSP +
                    userinfo.UserID + MessageTypes.NSP + userinfo.CardWord + MessageTypes.NSP + userinfo.UserName + MessageTypes.NSP + userinfo.UserPicture + MessageTypes.NSP + userinfo.UserStatus + MessageTypes.NSP + MessageTypes.Friend);

                    else //发给甲
                    {
                        foreach (User u in connViewModel.AllUsers)
                        {
                            if (u.userID.ToString() == msg[1])
                            {
                                SendMsg(u, MessageTypes.AGR + MessageTypes.PSP + "1" + MessageTypes.PSP +
                      userinfo.UserID + MessageTypes.NSP + userinfo.CardWord + MessageTypes.NSP + userinfo.UserName + MessageTypes.NSP + userinfo.UserPicture + MessageTypes.NSP + userinfo.UserStatus + MessageTypes.NSP + MessageTypes.Friend);

                                DealNGR(user, message); //清除记录

                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 处理拒绝添加为好友请求（来自乙）
        /// </summary> 
        /// <param name="message">乙ID+甲ID</param>
        public void DealNGR(User user, string message)
        {
            user.RunStatus = "拒绝好友添加请求.";

            string[] msg = message.Split(MessageTypes.NSP.ToCharArray()); 
            int result = 0; 

            try
            {
                result = Access.DeleteAddFriendInfo(msg[1], msg[0]); 
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->添加好友时拒绝后删除记录异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                SendMsg(user, MessageTypes.AFR + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,添加好友异常请重试！");

                return;
            }
        }

        /// <summary>
        /// 处理删除好友请求
        /// </summary> 
        /// <param name="message">自己ID+用户ID</param>
        public void DealDFR(User user, string message)
        {
            user.RunStatus = "删除好友.";

            string[] msg = message.Split(MessageTypes.NSP.ToCharArray());
             
            int result = 0; 

            try
            {
                result = Access.DeleteFriend(msg[0], msg[1]); 
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->删除好友操作异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                //发送好友 
                // SendFriend(user, int.Parse(message), mapper); //获取并发送好友列表

                SendMsg(user, MessageTypes.DFR + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,删除好友操作异常！");

                return;
            }

            if (result == 0)
                return;

            SendMsg(user, MessageTypes.DFR + MessageTypes.PSP + "1" + MessageTypes.PSP + msg[1]);

            foreach (User u in connViewModel.AllUsers)
            {
                if (u.userID == int.Parse(msg[1]))
                {
                    SendMsg(u, MessageTypes.DFR + MessageTypes.PSP + "1" + MessageTypes.PSP + msg[0]);
                    break;
                }
            }
        }

        /// <summary>
        /// 处理移动好友请求
        /// </summary> 
        /// <param name="message">自己ID+好友ID+好友所在组ID+移动到的组ID</param>
        public void DealMFR(User user, string message)
        {
            user.RunStatus = "移动好友.";

            string[] msg = message.Split(MessageTypes.NSP.ToCharArray()); 

            int result = 0; 

            try
            {
                result = Access.MoveFriend(msg[0], msg[1], msg[3]); 
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->移动好友操作异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));

                SendMsg(user, MessageTypes.MFR + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,移动好友操作异常！");
                return;
            }

            if (result == 0)
                return;

            //好友ID+好友原来所在组ID+现在所在组ID
            SendMsg(user, MessageTypes.MFR + MessageTypes.PSP + "1" + MessageTypes.PSP + msg[1] + MessageTypes.NSP + msg[2] + MessageTypes.NSP + msg[3]);
        }

        public void DealIFR(User user, string message)
        {

        }

        /// <summary>
        /// 处理添加游戏请求
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message">游戏ID</param>
        public void DealAGA(User user, string message)
        {
            user.RunStatus = "添加游戏.";

            try
            {
                Access.InsertUserToGame(user.userID.ToString(),message);
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->添加游戏操作异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));

               // SendMsg(user, MessageTypes.AGA + MessageTypes.PSP + "0" + MessageTypes.PSP + "很抱歉,添加游戏操作异常！");
                return;
            }
        }

        /// <summary>
        /// 更新游戏程序
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message">游戏ID+版本时间</param>
        public void DealGAM(User user, string message)
        {
            string[] msg = message.Split(MessageTypes.NSP.ToCharArray());
            string gametime = null;

            try
            {
                gametime = Access.GetGameTime(msg[0]);
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->获取游戏版本时间异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                return;
            }

            if (gametime != null)
            {
                if (string.Compare(msg[1], gametime, false) < 0)
                {
                    //可以更新
                    SendMsg(user, MessageTypes.CGA + MessageTypes.PSP + "1" + MessageTypes.PSP + "可以更新.");
                }
                else 
                {
                    //不用更新
                    SendMsg(user, MessageTypes.NGA + MessageTypes.PSP + "1" + MessageTypes.PSP + "不用更新.");
                }
            }
                
        }

        #endregion

        #region 外部调用方法

        /// <summary>
        /// 处理用户下线
        /// </summary>
        /// <param name="user"></param>
        public void DealOFF(User user)
        { 
            int result = 0;

            try
            {
                result = Access.UpdateStatus_N(user.userID); 
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("用户:{0}--->用户下线更新状态异常：{1}", user.client.Client.RemoteEndPoint, ex.Message));
                return;
            }

            if (result == 0)
                return;

            foreach (User u in connViewModel.AllUsers)
            {
                SendMsg(u, MessageTypes.OFF + MessageTypes.PSP + "1" + MessageTypes.PSP + user.userID);
            }
        }

        #endregion
    }
}
