 
using DataAccess.Models;
using DataAccess.Servers;
using IBatisNet.DataMapper;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Access
{
    public class Access
    {
        private Access() { }

        #region PreWindow

        /// <summary>
        /// 返回用户信息
        /// </summary>
        /// <param name="cardword">账号</param>
        /// <returns>用户信息</returns>
        public static UserInfo GetUserByCard(string cardword)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();  
            UserInfo userInfo = null;

            //查看账号是否存在
            try
            {
                mapper.BeginTransaction();
                userInfo = mapper.QueryForObject<UserInfo>("GetUserByCard", cardword);
                mapper.CommitTransaction();
            }
            catch { }

            return userInfo;
        }

        /// <summary>
        /// 账号是否存在
        /// </summary>
        /// <param name="cardword">账号</param>
        /// <returns>记录数</returns>
        public static int CardIsExist(string cardword)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper(); 
            int result = 0; 

            try
            {
                mapper.BeginTransaction();
                result = mapper.QueryForObject<int>("Card_IsExist", cardword);
                mapper.CommitTransaction();
            }
            catch { }

            return result;
        }

        /// <summary>
        /// 插入用户信息
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <returns>object</returns>
        public static object InsertUser(UserInfo userInfo)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();  
            object oj=null;

            try
            {
                mapper.BeginTransaction(); 
                oj=mapper.Insert("InsertUser", userInfo); 
                mapper.CommitTransaction();
            }
            catch{ }

            return oj;
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="userInfo">用户信息类</param>
        /// <returns>影响行数</returns>
        public static int UpdateUser(UserInfo userInfo)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper(); 
            int result = 0;

            try
            {
                mapper.BeginTransaction();
                result = mapper.Update("UpdateUser", userInfo);
                mapper.CommitTransaction();
            }
            catch{ }

            return result;
        }

        /// <summary>
        /// 更新用户状态Y
        /// </summary>
        /// <param name="cardword">账号</param>
        /// <returns>影响行数</returns>
        public static int UpdateStatus_Y(string cardword)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            int result = 0;

            //更新状态
            try
            {
                mapper.BeginTransaction();
                result = mapper.Update("UpdateStatusY", cardword);
                mapper.CommitTransaction();
            }
            catch { }

            return result;
        }

        /// <summary>
        /// 更新状态为N
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <returns></returns>
        public static int UpdateStatus_N(int userid)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            int result = 0;

            try
            {
                mapper.BeginTransaction();
                result = mapper.Update("UpdateStatusN", userid);
                mapper.CommitTransaction();
            }
            catch { }

            return result;
        }

        /// <summary>
        /// 邮箱是否已经被注册
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <returns>记录数</returns>
        public static int MailIsExist(string email)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            int result = 0;

            try
            {
                mapper.BeginTransaction();
                result = mapper.QueryForObject<int>("Mail_IsExist", email);
                mapper.CommitTransaction();
            }
            catch { }

            return result;
        }

        #endregion 

        #region MainWindow

        /// <summary>
        /// 根据账号查看状态
        /// </summary>
        /// <param name="cardword">账号</param>
        /// <returns>用户信息</returns>
        public static UserInfo StatusIsY(string cardword)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            UserInfo userInfo = null;

            try
            {
                mapper.BeginTransaction();
                userInfo = mapper.QueryForObject<UserInfo>("Status_IsY", cardword);
                mapper.CommitTransaction();
            }
            catch { }

            return userInfo;
        }

        /// <summary>
        /// 根据用户ID返回好友记录行集合
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <returns>满足记录集合</returns>
        public static List<FriendInfo> GetAllFriendByID(int userid)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            List<FriendInfo> friendList = null;

            try
            {
                mapper.BeginTransaction();
                friendList = mapper.QueryForList<FriendInfo>("GetAllFriendByID", userid).ToList();
                mapper.CommitTransaction();
            }
            catch { }

            return friendList;
        }

        /// <summary>
        /// 根据用户id返回用户信息
        /// </summary>
        /// <param name="listFriend">存放用户id与组id的集合</param>
        /// <returns>包含（用户信息+所在组id）的集合</returns>
        public static List<UserInfo> GetAllUserByID(List<Friend> listFriend)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            List<UserInfo> userList = null;

            try
            {
                mapper.BeginTransaction();
                userList = mapper.QueryForList<UserInfo>("GetAllUserByID", listFriend).ToList();
                mapper.CommitTransaction();
            }
            catch { }

            return userList;
        }

        /// <summary>
        /// 根据用户id返回所玩游戏信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static List<GameInfo> GetAllGameByID(int userid)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            List<GameInfo> gameList = null;

            try
            {
                mapper.BeginTransaction();
                gameList = mapper.QueryForList<GameInfo>("GetAllGameByID", userid).ToList();
                mapper.CommitTransaction();
            }
            catch { }

            return gameList;
        }

        /// <summary>
        /// 根据用户所玩游戏信息获取相应成就信息
        /// </summary>
        /// <param name="gameInfo">包含（用户id,游戏名字,游戏表）的信息</param>
        /// <returns>成就信息</returns>
        public static AchieveInfo GetAllAchieveByID(GameInfo gameInfo)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            AchieveInfo achieveInfo = null;

            try
            {
                mapper.BeginTransaction();
                achieveInfo=mapper.QueryForObject<AchieveInfo>("GetAllAchieveByID", gameInfo);
                mapper.CommitTransaction();
            }
            catch { }

            return achieveInfo;
        }

        /// <summary>
        /// 返回消息集合
        /// </summary>
        /// <returns>系统消息集合</returns>
        public static List<NoticeInfo> GetAllMsg()
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            List<NoticeInfo> noticeList = null;

            try
            {
                mapper.BeginTransaction();
                noticeList = mapper.QueryForList<NoticeInfo>("GetAllMsg", null).ToList();
                mapper.CommitTransaction();
            }
            catch { }

            return noticeList;
        }

        /// <summary>
        /// 根据用户id返回“别人请求添加自己为好友”的该用户的信息集合
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <returns>消息集合</returns>
        public static List<UserInfo> GetAddFromByAddTo(int userid)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            List<UserInfo> userList = null;
            try
            {
                mapper.BeginTransaction();
                userList = mapper.QueryForList<UserInfo>("GetAddFromByAddTo", userid).ToList();
                mapper.CommitTransaction();
            }
            catch { }

            return userList;
        }

        /// <summary>
        /// 1.根据用户id返回“自己请求别人为好友，然后对方同意了”的用户信息集合
        /// 2.存在自己添加别人为好友的请求且已经被处理了，删除该记录
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <returns></returns>
        public static List<UserInfo> GetAddToByAddFrom(int userid)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            List<UserInfo> userList = null;
            try
            {
                mapper.BeginTransaction();
                userList = mapper.QueryForList<UserInfo>("GetAddToByAddFrom", userid).ToList();
                mapper.Delete("DeleteAgreeByAddFrom", userid); //存在自己添加别人为好友的请求且已经被处理了，删除该记录
                mapper.CommitTransaction();
            }
            catch { }

            return userList;
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="cardword">账号</param>
        /// <param name="PrePass">原密码（加密后）</param>
        /// <param name="NewPass">新密码（加密后）</param>
        /// <returns></returns>
        public static int ResetPassword(string cardword,string PrePass,string NewPass)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            int result = 0;
            Hashtable hashTable = new Hashtable();

            hashTable.Add("CardWord", cardword);
            hashTable.Add("PassWord", PrePass);
            hashTable.Add("NewPassWord", NewPass);

            try
            {
                mapper.BeginTransaction();
                result = mapper.Update("ResetPassword", hashTable);
                mapper.CommitTransaction();
            }
            catch { }

            return result;
        }

        /// <summary>
        /// 根据账号或昵称返回用户信息集合
        /// </summary>
        /// <param name="cardorname">账号或昵称</param>
        /// <returns></returns>
        public static List<UserInfo> GetUserByIDorName(string cardorname)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            List<UserInfo> userList = null;

            try
            {
                mapper.BeginTransaction();
                userList = mapper.QueryForList<UserInfo>("GetUserByIDorName", cardorname).ToList();
                mapper.CommitTransaction();
            }
            catch { }

            return userList;
        }

        /// <summary>
        /// 判断addfriendinfo表中是否存在相同记录
        /// </summary>
        /// <param name="fromid">请求者id</param>
        /// <param name="toid">被请求者id</param>
        /// <returns>记录数</returns>
        public static int IsExistAddFriendInfo(string fromid,string toid)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            int result = 0;

            //判断addfriendinfo表中是否存在相同记录
            Hashtable hashTable = new Hashtable();
            hashTable.Add("AddFrom", fromid);
            hashTable.Add("AddTo", toid);

            try
            {
                mapper.BeginTransaction();
                result = mapper.QueryForObject<int>("IsExistAddFriendInfo", hashTable);
                mapper.CommitTransaction();
            }
            catch { }

            return result;
        }

        /// <summary>
        /// 将记录请求插入addfriendinfo表中
        /// </summary>
        /// <param name="fromid">请求者id</param>
        /// <param name="toid">被请求者id</param>
        /// <returns></returns>
        public static object AddFriendInfo(string fromid, string toid)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            object oj = null;
             
            Hashtable hashTable = new Hashtable();
            hashTable.Add("AddFrom", fromid);
            hashTable.Add("AddTo", toid);

            //将记录请求插入addfriendinfo表中
            try
            {
                mapper.BeginTransaction();
                oj=mapper.Insert("AddFriendInfo", hashTable);
                mapper.CommitTransaction();
            }
            catch { }

            return oj;
        }

        /// <summary>
        /// 被请求者同意后，修改添加表中的记录为“已同意”
        /// </summary>
        /// <param name="fromid">请求者id</param>
        /// <param name="toid">被请求者id</param>
        /// <returns>影响行数</returns>
        public static int UpdateAddFriendInfo(string fromid, string toid)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            int result = 0;

            //修改添加表中的记录为“已同意”。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。
            Hashtable hashTable = new Hashtable();
            hashTable.Add("AddFrom", fromid);
            hashTable.Add("AddTo", toid);

            try
            {
                mapper.BeginTransaction();
                result = mapper.Update("UpdateAddFriendInfo", hashTable);
                mapper.CommitTransaction();
            }
            catch { }

            return result;
        }

        /// <summary>
        /// 查看好友映射表中是否已存在记录
        /// </summary>
        /// <param name="oneid">用户id</param>
        /// <param name="otherid">用户id</param>
        /// <returns>记录数</returns>
        public static int IsExistFriendInfo(string oneid,string otherid)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            int result = 0;

            //查看好友映射表中是否已存在记录。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。
            Hashtable hashTable = new Hashtable();
            hashTable.Add("OneID", oneid);
            hashTable.Add("OtherID", otherid);

            try
            {
                mapper.BeginTransaction();
                result = mapper.QueryForObject<int>("IsExistFriendInfo", hashTable);
                mapper.CommitTransaction();
            }
            catch { }

            return result;
        }

        /// <summary>
        /// 插入好友映射表
        /// </summary>
        /// <param name="oneid">用户id</param>
        /// <param name="otherid">用户id</param>
        /// <returns></returns>
        public static object InsertFriendInfo(string oneid, string otherid)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            object oj = null;
 
            Hashtable hashTable = new Hashtable();
            hashTable.Add("OneID", oneid);
            hashTable.Add("OtherID", otherid);

            //插入好友映射表。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。
            try
            {
                mapper.BeginTransaction();
                oj = mapper.Insert("InsertFriendInfo", hashTable);
                mapper.CommitTransaction();
            }
            catch { }

            return oj;
        }

        /// <summary>
        /// 根据用户id，返回用户信息
        /// </summary>
        /// <param name="oneid">用户id</param>
        /// <param name="otherid">用户id</param>
        /// <returns>两个用户的信息</returns>
        public static List<UserInfo> GetUserInfoByID(string oneid, string otherid)
        { 
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            List<UserInfo> userList = null;

            ArrayList arrayList = new ArrayList();
            arrayList.Add(oneid);
            arrayList.Add(otherid);

            try
            {
                mapper.BeginTransaction();
                userList = mapper.QueryForList<UserInfo>("GetUserInfoByID", arrayList).ToList();
                mapper.CommitTransaction();
            }
            catch { }

            return userList;
        }

        /// <summary>
        /// 删除该好友请求记录
        /// </summary>
        /// <param name="fromid">请求者id</param>
        /// <param name="toid">被请求者id</param>
        /// <returns></returns>
        public static int DeleteAddFriendInfo(string fromid, string toid)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            int result = 0;

            Hashtable hashTable = new Hashtable();
            hashTable.Add("AddFrom", fromid);
            hashTable.Add("AddTo", toid);

            try
            {
                mapper.BeginTransaction();
                result = mapper.Delete("DeleteAddFriendInfo", hashTable);
                mapper.CommitTransaction();
            }
            catch { }

            return result;
        }

        /// <summary>
        /// 删除好友
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <param name="friendid">好友id</param>
        /// <returns></returns>
        public static int DeleteFriend(string userid, string friendid)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            int result = 0;
            Hashtable hashTable = new Hashtable();

            hashTable.Add("UserID", userid);
            hashTable.Add("FriendID", friendid);

            try
            {
                mapper.BeginTransaction();
                result = mapper.Delete("DeleteFriend", hashTable);
                mapper.CommitTransaction();
            }
            catch { }

            return result;
        }

        /// <summary>
        /// 移动好友
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <param name="friendid">好友id</param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public static int MoveFriend(string userid, string friendid, string groupid)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            int result = 0;

            Hashtable hashTable = new Hashtable();

            hashTable.Add("UserID", userid);
            hashTable.Add("FriendID", friendid);
            hashTable.Add("GroupID", groupid);

            try
            {
                mapper.BeginTransaction();
                result = mapper.Update("MoveFriend1", hashTable);
                mapper.CommitTransaction();
            }
            catch { }

            if (result == 0)
            {
                try
                {
                    mapper.BeginTransaction();
                    result = mapper.Update("MoveFriend2", hashTable);
                    mapper.CommitTransaction();
                }
                catch { }
            }

            return result;
        }

        public static object InsertUserToGame(string userid, string gameid)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            int result = 0;
            Hashtable hashTable = new Hashtable();

            hashTable.Add("UserID", userid);
            hashTable.Add("GameID", gameid);

            try
            {
                mapper.BeginTransaction();
                result = mapper.QueryForObject<int>("IsExistUserToGame", hashTable);
                mapper.CommitTransaction();
            }
            catch { }

            if (result == 0)
            {
                object oj = null;

                try
                {
                    mapper.BeginTransaction();
                    oj = mapper.Insert("InsertUserToGame", hashTable);
                    mapper.CommitTransaction();
                }
                catch { }

                return oj;
            }

            return null;
        }

        /// <summary>
        /// 根据游戏ID获取路径
        /// </summary>
        /// <param name="gameid"></param>
        /// <returns></returns>
        public static string GetGamePath(string gameid)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            string gamepath = null;

            try
            {
                mapper.BeginTransaction();
                gamepath = mapper.QueryForObject<string>("GetGamePath", gameid);
                mapper.CommitTransaction();
            }
            catch { }

            return gamepath;
        }

        /// <summary>
        /// 获取游戏版本时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetGameTime(string gameid)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            string gametime = null;

            try
            {
                mapper.BeginTransaction();
                gametime = mapper.QueryForObject<string>("GetGameTime", gameid);
                mapper.CommitTransaction();
            }
            catch { }

            return gametime;
        }

        #endregion

        #region 消息管理

        /// <summary>
        /// 发布消息，插入数据库
        /// </summary>
        /// <param name="noticeInfo"></param>
        /// <returns></returns>
        public static object InsertNotice(NoticeInfo noticeInfo)
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            object oj = null;

            try
            {
                mapper.BeginTransaction();
                oj=mapper.Insert("InsertNotice", noticeInfo);
                mapper.CommitTransaction();
            }
            catch { }

            return oj; 
        }

        /// <summary>
        /// 清空消息
        /// </summary>
        /// <returns></returns>
        public static int ClearNotice()
        {
            ISqlMapper mapper = GetSqlMapper.GetMapper();
            int result=0;

            try
            {
                mapper.BeginTransaction();
                result=mapper.Delete("ClearNotice", null);
                mapper.CommitTransaction();
            }
            catch { }

            return result;
        }

        #endregion
    }
}
