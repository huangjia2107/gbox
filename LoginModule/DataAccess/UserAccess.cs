using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LoginModule.Models;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace LoginModule.DataAccess
{
    public class UserAccess
    {
        //E:\\项目文件\\WPF\\G-Box\\G-Box\\bin\\Debug\\
        string _user_path = AppDomain.CurrentDomain.BaseDirectory +"Data\\users.data";
        public event EventHandler<UserAddedEventArgs> UserAdded;
        public event EventHandler<UserDeletedEventArgs> UserDeleted; 
        List<UserModel> _userModels;

        #region 构造函数

        public UserAccess()
        {
            IsExist(_user_path);
            _userModels = LoadFromUser(_user_path);
        }

        #endregion

        #region 处理用户记录信息相关操作

        void IsExist(string path)
        {
            if(!File.Exists(path))
                using(FileStream fs=new FileStream(path,FileMode.Create)){}               
        }

        public static List<UserModel> LoadFromUser(string user_path) //反序列化
        {
            List<UserModel> list = new List<UserModel>();
            using (FileStream fs = new FileStream(user_path, FileMode.OpenOrCreate))
            {
                FileInfo fi = new FileInfo(user_path);
                if (fi.Length != 0)
                {
                    try
                    { 
                        BinaryFormatter formatter = new BinaryFormatter();
                        list = (List<UserModel>)formatter.Deserialize(fs);
                    }
                    catch (SerializationException e)
                    {
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        fs.Close();
                    }
                }
            }
            return list;
        }

        void SaveToUser(List<UserModel> collect)
        {
            FileStream fs = new FileStream(_user_path, FileMode.OpenOrCreate);
            BinaryFormatter bf = new BinaryFormatter();

            try
            {
                bf.Serialize(fs, collect);
            }
            catch (SerializationException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                fs.Close();
            }
        }

        public List<UserModel> GetUser()
        {
            return new List<UserModel>(_userModels);
        }

        public bool IsAlreadyExists(UserModel userModel)
        {
            foreach (UserModel model in _userModels)
            {
                if (model.CardWord == userModel.CardWord)
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdateUser(UserModel userModel)
        {
            foreach (UserModel model in _userModels)
            {
                if (model.CardWord == userModel.CardWord)
                {
                    model.Password = userModel.Password;
                    model.IsRemPass = userModel.IsRemPass;

                    model.NowTime = userModel.NowTime;

                    SaveToUser(_userModels);

                    break;
                }
            }
        }

        public void AddUser(UserModel userModel)
        {
            if (!IsAlreadyExists(userModel))
            {
                _userModels.Add(userModel);

                SaveToUser(_userModels);

                if (this.UserAdded != null)
                {
                    this.UserAdded(this, new UserAddedEventArgs(userModel));
                }
            }
        }

        public void DeleteUser(string cardword)
        {
            foreach (UserModel model in _userModels)
            {
                _userModels.RemoveAll(user => user.CardWord == cardword);

                SaveToUser(_userModels);

                if (this.UserDeleted != null)
                {
                    this.UserDeleted(this,new UserDeletedEventArgs(cardword));
                }

                break;
            }
        }

        public void SortUser()
        {
            _userModels = _userModels.OrderByDescending(s => s.NowTime).ToList();  //降序
            SaveToUser(_userModels);
        }

        #endregion

    }

    #region 外部类

    //--添加记录事件
    public class UserAddedEventArgs : EventArgs
    {
        public UserAddedEventArgs(UserModel userModel)
        {
            this.NewUserModel = userModel;
        }

        public UserModel NewUserModel { get; private set; }
    }

    //--删除记录事件
    public class UserDeletedEventArgs : EventArgs
    {
        public UserDeletedEventArgs(string cardword)
        {
            this.CardWord = cardword;
        }

        public string CardWord { get; private set; }
    }

    #endregion
}
