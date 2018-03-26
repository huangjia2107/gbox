
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using System.Windows.Input;

namespace UpdateApp
{
    public class ViewModel : NotificationObject
    { 
        #region 绑定的属性

        public string connectStatus;
          
        public string fileName;
        public string fileSize;
        public string downSpeed;
        public string leftTime;
        public string pgBarValue;

        public string updateStatus;

        public string ConnectStatus
        {
            get { return connectStatus; }
            set
            {
                if (value == connectStatus)
                    return;

                connectStatus = value;
                base.RaisePropertyChanged("ConnectStatus");
            }
        }

        public string FileName
        {
            get { return fileName; }
            set
            {
                if (value == fileName)
                    return;

                fileName = value;
                base.RaisePropertyChanged("FileName");
            }
        }

        public string FileSize
        {
            get { return fileSize; }
            set
            {
                if (value == fileSize)
                    return;

                fileSize = value;
                base.RaisePropertyChanged("FileSize");
            }
        } 

        public string DownSpeed
        {
            get { return downSpeed; }
            set
            {
                if (value == downSpeed)
                    return;

                downSpeed = value;
                base.RaisePropertyChanged("DownSpeed");
            }
        }

        public string LeftTime
        {
            get { return leftTime; }
            set
            {
                if (value == leftTime)
                    return;

                leftTime = value;
                base.RaisePropertyChanged("LeftTime");
            }
        }

        public string PgBarValue
        {
            get { return pgBarValue; }
            set
            {
                if (value == pgBarValue)
                    return;

                pgBarValue = value;
                base.RaisePropertyChanged("PgBarValue");
            }
        }
         
        public string UpdateStatus
        {
            get { return updateStatus; }
            set
            {
                if (value == updateStatus)
                    return;

                updateStatus = value;
                base.RaisePropertyChanged("UpdateStatus");
            }
        }

        #endregion 

        #region 绑定的命令

        DelegateCommand<Window> dragCommand { get; set; }
        DelegateCommand minedCommand { get; set; }

        public ICommand DragCommand
        {
            get
            {
                if (dragCommand == null)
                    dragCommand = new DelegateCommand<Window>((win) =>
                    {
                        win.DragMove();
                    });

                return dragCommand;
            }
        }

        public ICommand MinedCommand
        {
            get
            {
                if (minedCommand == null)
                    minedCommand = new DelegateCommand(() =>
                    {
                        App.Current.MainWindow.WindowState = WindowState.Minimized;
                    });

                return minedCommand;
            }
        }

        #endregion
    }

}
