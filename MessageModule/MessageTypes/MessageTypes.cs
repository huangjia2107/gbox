
namespace MessageModule.MessageTypes
{
    public class MessageTypes
    {
        #region 枚举

        #region 登录服务器

        public enum LoginSign {UPD,LOG,RNU,REG,VNU,RES};
        public static string[] LoginSignArray = { "UPD", "LOG", "RNU", "REG", "VNU", "RES" };

        #endregion 

        #region 大厅服务器

        public enum HallSign { CON, SET, MSG, FRI, ACH, SFR, AFR, AGR, NGR, DFR, MFR, IFR, AGA,GAM };
        public static string[] HallSignArray = { "CON","SET", "MSG", "FRI", "ACH", "SFR", "AFR", "AGR", "NGR", "DFR", "MFR", "IFR", "AGA","GAM" };

        #endregion

        #endregion

        #region Public

        /// <summary>
        /// 发送key标志
        /// </summary>
        public const string KEY = "<KEY>";

        /// <summary>
        /// 更新标志
        /// ---------------------------------
        /// ===发向服务器的信息 
        ///    ===UPD+版本号+更新时间
        /// ===从服务器返回的信息 
        ///    ===CUP+版本号+更新时间(更新整个程序) 
        ///    ===CUP+更新时间(更新数据) 
        ///    ===NUP(不用更新)
        /// </summary>
        public const string UPD = "UPD";
        public const string CUP = "CUP";
        public const string NUP = "NUP";

        /// <summary>
        /// 模块标志+Sign+消息内容
        /// 如："LOG"+"&"+1+"&"+账号+"$"+密码+"$"+头像
        /// </summary>
        public const string PSP = "&";

        /// <summary>
        /// 消息内容的分割
        /// 如：账号+"$"+密码+"$"+头像
        /// </summary>
        public const string NSP = "$";

        /// <summary>
        /// 用户状态
        /// </summary>
        public const string N = "N";
        public const string Y = "Y";

        /// <summary>
        /// 消息类型
        /// </summary>
        public const string F = "F"; //好友添加
        public const string M = "M"; //系统信息
        public const string T = "T"; //仅仅为提示

        #endregion

        #region PreWindow

        /// <summary>
        /// 登陆标志
        /// ---------------------------------
        /// ===发向服务器的信息 
        ///    ===LOG+账号+密码
        /// ===从服务器返回的信息 
        ///    ===LOG+账号+昵称+头像 Sign=1 
        ///    ===LOG 只能是失败错误 Sign=0
        /// </summary>
        public const string LOG = "LOG";

        /// <summary>
        /// 重置密码标志
        /// ---------------------------------
        /// ===发向服务器的信息
        ///    ===重置密码
        ///       ===RES+账号+新密码+邮箱+验证码
        ///    ===获取验证码
        ///       ===VNU+账号+邮箱
        /// ===从服务器返回的信息
        ///    ===RES 相应失败或成功信息
        /// </summary>
        public const string VNU = "VNU"; 
        public const string RES = "RES"; 

        /// <summary>
        /// 注册标志
        /// ---------------------------------
        /// ===发向服务器的信息（$）
        ///    ===注册
        ///       ===REG+账号+昵称+密码+头像+邮箱+验证码
        ///    ===获取注册码
        ///       ===RNU+邮箱
        ///===从服务器返回的信息（$）
        ///    ===REG 相应失败或成功信息
        /// </summary>
        public const string RNU = "RNU";
        public const string REG = "REG";

        #endregion

        #region MainWindow

        #region Public

        /// <summary>
        /// 连接大厅服务器标志
        /// ---------------------------------
        /// ===发向服务器的信息（$）
        ///    ===CON+账号
        /// ===服务器返回的信息 
        ///    ===CON 失败信息：相应信息 Sign=0 
        ///    ===CON 成功信息：相应信息 Sign=1 
        ///    ===PER 个人信息：账号+昵称+头像
        ///    ===ACH 游戏成就：游戏名+等级+排名+累积积分+单次最高积分
        ///    ===FRI 好友列表：ID+账号+昵称+头像+状态+组ID（0：我的好友；1：我的家族；2：黑名单）
        /// </summary>
        public const string CON = "CON";

        #endregion

        #region CardModule

        /// <summary>
        /// 我的好友
        /// </summary>
        public const string Friend = "0";

        /// <summary>
        /// 我的家族
        /// </summary>
        public const string Family = "1";

        /// <summary>
        /// 黑名单
        /// </summary>
        public const string Black = "2";

        /// <summary>
        /// 刷新个人信息
        /// --------------------------------
        /// ===发向服务器的信息（$）
        ///    ===PER+账号
        /// ===从服务器返回的信息（$）
        ///    ===PER 个人信息：账号+昵称+头像
        ///    ===ACH 游戏成就：游戏名+等级+排名+累积积分+单次最高积分
        ///    ===FRI 好友列表：ID+账号+昵称+头像+状态+组ID（0：我的好友；1：我的家族；2：黑名单）
        ///    ===MSG 最新信息：主题+发件人+时间+内容
        /// </summary>
        public const string PER = "PER";

        /// <summary>
        /// 修改密码
        /// --------------------------------
        /// ===发向服务器的信息（$）
        ///    ===SET+账号+原密码+新密码
        /// ===从服务器返回的信息（$）
        ///    ===SET 相应失败或成功信息
        /// </summary>
        public const string SET = "SET";

        /// <summary>
        /// 信息
        /// --------------------------------
        /// ===发向服务器的信息（$）
        ///    ===MSG+用户ID
        /// ===从服务器返回的信息（$）
        ///    ===MSG 最新信息：类型+主题+内容+发布人
        /// </summary>
        public const string MSG = "MSG";

        /// <summary>
        /// 刷新好友列表
        /// --------------------------------
        /// ===发向服务器的信息（$）
        ///    ===FRI+用户ID
        /// ===从服务器返回的信息（$）
        ///    ===FRI 好友列表：账号+昵称+头像+状态+组ID（0：我的好友；1：我的家族；2：黑名单） Sign=1
        ///    ===失败信息 Sign=0
        /// </summary>
        public const string FRI = "FRI";

        /// <summary>
        /// 上线通知
        /// --------------------------------
        /// ===从服务器返回的信息（$）
        ///    ===UPL 用户ID Sign=1
        /// </summary>
        public const string UPL = "UPL";

        /// <summary>
        /// 下线通知
        /// --------------------------------
        /// ===发向服务器的信息（$）
        ///    ===OFF+用户ID
        /// ===从服务器返回的信息（$）
        ///    ===OFF 下线通知：用户ID Sign=1 
        /// </summary>
        public const string OFF = "OFF";

        /// <summary>
        /// 刷新成就
        /// --------------------------------
        /// ===发向服务器的信息（$）
        ///    ===ACH+用户ID
        /// ===从服务器返回的信息（$）
        ///    ===ACH 游戏成就：游戏名+等级+排名+累积积分+单次最高积分 Sign=1
        ///    ===失败信息 Sign=0
        /// </summary>
        public const string ACH = "ACH";

        /// <summary>
        /// 查找好友
        /// --------------------------------
        /// ===发向服务器的信息（$）
        ///    ===SFR+账号或昵称
        /// ===从服务器返回的信息（$）
        ///    ===SFR 查找结果：账号+昵称+头像
        /// </summary>
        public const string SFR = "SFR";

        /// <summary>
        /// 添加好友（甲===>>>乙）
        /// --------------------------------
        /// ===发向服务器的信息（$）
        ///    ===AFR 添加好友：甲ID+甲账号+乙ID （甲）
        ///    ===AGR 同意添加：乙ID+甲ID （乙）
        ///    ===NGR 拒绝添加：乙ID+甲ID （乙）
        /// ===从服务器返回的信息（$）
        ///    ===ASK 询问添加：甲ID+甲账号 （乙）
        ///    ===AGR 添加提示：乙账号++昵称+头像+状态+组ID （发给甲） 用户 + 乙 + 同意您的好友添加请求！
        ///    ===AFR 直接添加：甲账号++昵称+头像+状态+组ID （发给乙）
        /// </summary>
        public const string AFR = "AFR";
        public const string AGR = "AGR";
        public const string NGR = "NGR";
        public const string ASK = "ASK";

        /// <summary>
        /// 删除好友（成功再删除）
        /// --------------------------------
        /// ===发向服务器的信息（$）
        ///    ===DFR+自己ID+好友ID
        /// ===从服务器返回的信息（$）
        ///    ===DFR 成功：ID
        ///           失败：相应失败信息
        /// </summary>
        public const string DFR = "DFR";

        /// <summary>
        /// 移动好友（成功再移动）
        /// --------------------------------
        /// ===发向服务器的信息（$）
        ///    ===MFR+自己ID+好友ID+好友所在组ID+移动到的组ID（0：我的好友；1：我的家族；2：黑名单）
        /// ===从服务器返回的信息（$）
        ///    ===MFR 成功：好友ID+好友所在组ID+移动到的组ID（0：我的好友；1：我的家族；2：黑名单）
        ///           失败：相应失败信息
        /// </summary>
        public const string MFR = "MFR";

        /// <summary>
        /// 邀请好友
        /// --------------------------------
        /// ===发向服务器的信息（$）
        ///    ===IFR+自己账号+好友账号+移动到的组名
        /// ===从服务器返回的信息（$）
        ///    ===IFR 相应失败或成功信息（成功再移动）
        /// </summary>
        public const string IFR = "IFR";

        #endregion

        #region IntroductionModule

        /// <summary>
        /// 添加游戏
        /// --------------------------------
        /// ===发向服务器的信息（$）
        ///    ===AGA+游戏ID
        /// </summary>
        public const string AGA = "AGA";

        #endregion

        #region MyGameModule

        /// <summary>
        /// 打来游戏，先检查是否存在更新
        /// --------------------------------
        /// ===发向服务器的信息（$）
        ///    ===GAM+游戏ID
        /// </summary>
        public const string GAM = "GAM";
        public const string CGA = "CGA";
        public const string NGA = "NGA";

        #endregion

        #endregion
    }
}
