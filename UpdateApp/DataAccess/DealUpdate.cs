using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace UpdateApp
{
    public class DealUpdate
    {

        #region 定义单例

        private static DealUpdate instance = null;

        private DealUpdate() { }

        /// <summary>
        /// 返回唯一实例
        /// </summary> 
        public static DealUpdate GetInstance()
        {
            if (instance == null)
                instance = new DealUpdate();

            return instance;
        }

        #endregion

        #region 变量

        /// <summary>
        /// 当前应用路径
        /// </summary>
        private string currentPath = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 文件更新列表
        /// </summary>
        List<UpdateModel> _updateModels;

        /// <summary>
        /// 数据获取入口
        /// </summary>
        UpdateAccess updateAccess;

        #endregion

        #region 方法

        /// <summary>
        /// 开始更新操作入口点
        /// </summary>
        /// <param name="connectParam"></param>
        /// <param name="viewModel"></param>
        public void ConfigUpdate(ConnectParam connectParam, ViewModel viewModel)
        {
            updateAccess = new UpdateAccess(connectParam.tempPath + "UdConfig.xml");
            _updateModels = updateAccess.GetConfig();

            //版本更新
            if (_updateModels.Count == 1)
            {
                VersionUpdate(connectParam, viewModel, _updateModels);
            }
            //数据更新
            else {
                DataUpdate(connectParam, viewModel, _updateModels);
            } 
        }

        /// <summary>
        /// 版本更新
        /// </summary>
        private void VersionUpdate(ConnectParam connectParam,ViewModel viewModel,List<UpdateModel> _updateModels)
        {
            viewModel.UpdateStatus = "准备启动安装文件...";

            try
            {
                Process.Start(connectParam.tempPath + _updateModels[0].FileName);
            }
            catch { viewModel.UpdateStatus = "启动安装文件失败..."; }
            finally
            {
                viewModel.UpdateStatus = "等待关闭更新程序...";
                Environment.Exit(0);
            } 
        }

        /// <summary>
        /// 数据更新
        /// </summary>
        private void DataUpdate(ConnectParam connectParam, ViewModel viewModel, List<UpdateModel> _updateModels)
        {
            viewModel.UpdateStatus = "备份原文件...";
            try
            {
                BackupFile(connectParam, _updateModels);
            }
            catch { }

            viewModel.UpdateStatus = "更新文件...";
            try
            {
                UpdateFile(connectParam, _updateModels);
            }
            catch
            {
                viewModel.UpdateStatus = "更新失败,进行还原...";
                //还原
                ReStoreFile(connectParam, _updateModels);

                return;
            }

            viewModel.UpdateStatus = "清理无效文件...";
            try
            {
                ClearFile(connectParam);
            }
            catch { }
        }

        /// <summary>
        /// 备份要更新的文件
        /// </summary> 
        private void BackupFile(ConnectParam connectParam, List<UpdateModel> updateModels)
        {
            if (!Directory.Exists(currentPath + connectParam.backupPath))
            {
                Directory.CreateDirectory(currentPath + connectParam.backupPath);
            }

            foreach (UpdateModel model in updateModels)
            {
                try
                {
                    //为空，则代表当前目录
                    if (model.MoveToPath.Trim() == "")
                        File.Copy(currentPath + model.FileName, currentPath + connectParam.backupPath + "\\" + model.FileName, true);
                    else
                        File.Copy(currentPath + model.MoveToPath + "\\" + model.FileName, currentPath + connectParam.backupPath + "\\" + model.FileName, true);
                }
                catch (Exception ex) { MessageBox.Show("备份异常:" + ex.Message); }
            }
        }

        /// <summary>
        /// 还原备份
        /// </summary>
        private void ReStoreFile(ConnectParam connectParam, List<UpdateModel> updateModels)
        {
            foreach (UpdateModel model in updateModels)
            {
                try
                {
                    //为空，则代表当前目录
                    if (model.MoveToPath.Trim() == "")
                        File.Copy(currentPath + connectParam.backupPath + "\\" + model.FileName, currentPath + model.FileName, true);
                    else
                        File.Copy(currentPath + connectParam.backupPath + "\\" + model.FileName, currentPath + model.MoveToPath + "\\" + model.FileName, true);
                }
                catch (Exception ex) { MessageBox.Show("还原异常:" + ex.Message); }
            }
        }

        /// <summary>
        /// 更新文件
        /// </summary> 
        private void UpdateFile(ConnectParam connectParam, List<UpdateModel> updateModels)
        {
            foreach (UpdateModel model in updateModels)
            {
                try
                {
                    if (model.MoveToPath.Trim() == "")
                        File.Copy(connectParam.tempPath + model.FileName, currentPath + model.FileName, true);
                    else
                        File.Copy(connectParam.tempPath + model.FileName, currentPath + model.MoveToPath + "\\" + model.FileName, true);
                }
                catch (Exception ex) { MessageBox.Show("更新异常:" + ex.Message); }
            }
        }

        /// <summary>
        /// 清理文件
        /// </summary>
        /// <param name="connectParam"></param>
        private void ClearFile(ConnectParam connectParam)
        {
            try
            {
                if (Directory.Exists(currentPath + connectParam.backupPath))
                {
                    Directory.Delete(currentPath + connectParam.backupPath, true);
                }

                if (Directory.Exists(connectParam.tempPath))
                {
                    Directory.Delete(connectParam.tempPath, true);
                }
            }
            catch (Exception ex) { MessageBox.Show("清理异常:" + ex.Message); }
        }

        #endregion
    }
}
