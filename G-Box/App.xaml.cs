using System.Windows;

namespace G_Box
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Bootstrapper bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }
    }
}
