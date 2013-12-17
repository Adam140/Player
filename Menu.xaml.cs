using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Player
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : UserControl, ISwitchable
    {
        MainWindow mainWindow;
        public Menu(MainWindow main)
        {
            InitializeComponent();
            this.mainWindow = main;
        }

        public void UtilizeState(object state)
        {
            mainWindow.buttonBack.Visibility = Visibility.Visible;
        }

        public void Destroy()
        {
        }

        private void KinectButtonMenu(object sender, RoutedEventArgs e)
        {
            String name = ((Microsoft.Kinect.Toolkit.Controls.KinectTileButton)e.OriginalSource).Name;
            switch (name)
            {
                case "buttonMultimedia":
                    ViewSwitcher.Switch(mainWindow.playerScreen = new player(""));
                    break;
                case "buttonMusic":
                    ViewSwitcher.Switch(new Playlist(@"D:\Studia\player\Multimedia"));
                    break;
            }

        }
    }
}
