using Fizbin.Kinect.Gestures;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
                case "buttonAudio":
                    ViewSwitcher.Switch(new Playlist(@"D:\Studia\player\Multimedia"));
                    break;
            }

        }

      
    }
}
