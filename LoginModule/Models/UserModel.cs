using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.ComponentModel;
using LoginModule.ViewModels;

namespace LoginModule.Models
{
    [Serializable] 
    public class UserModel
    {
        protected UserModel() //构造函数
        { }

        public static UserModel CreateNewModel()
        {
            return new UserModel();
        }

        public static UserModel CreateModel(string cardword, string username, string image, string password, bool isRemPass,System.DateTime nowtime)
        {
            return new UserModel
            {
                CardWord = cardword,
                UserName = username,
                ImageUrl = image,
                Password = password,
                IsRemPass = isRemPass,
                NowTime=nowtime
            };
        }

        public string CardWord { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ImageUrl { get; set; }
        public bool IsRemPass { get; set; }
        public System.DateTime NowTime { get; set; }
    }
}
