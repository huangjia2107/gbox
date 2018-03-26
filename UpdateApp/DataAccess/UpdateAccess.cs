using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace UpdateApp
{
    public class UpdateAccess
    {
        #region 变量

        string  path;
        List<UpdateModel> _updateModels;

        #endregion

        public UpdateAccess(string path)
        {
            this.path = path;
        }

        #region 方法

        /// <summary>
        /// 暴露于外部的方法
        /// </summary>
        /// <returns></returns>
        public List<UpdateModel> GetConfig()
        {
            return LoadFromFile();
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <returns></returns>
        List<UpdateModel> LoadFromFile()
        {
            if (!File.Exists(path))
            {
                return null;
            }

            return (from config in XElement.Load(path).Descendants("File")
                    select UpdateModel.CreateModel(
                       config.Element("FileName").Value,
                       config.Element("MoveToPath").Value
                       )
            ).ToList();
        }

        #endregion
    }
}
