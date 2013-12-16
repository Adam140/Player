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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ISwitchable currentView;

        public Menu mainMenu;
        public player playerScreen;

        public MainWindow()
        {
            InitializeComponent();
            ViewSwitcher.SetMainWindow(this);
            ViewSwitcher.Switch(mainMenu = new Menu(this));

        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Q)
            {
                currentView.Destroy();
                this.Close();
            }
        }

        public void Navigate(UserControl view)
        {
            if (currentView != null)
                currentView.Destroy();

            if (view.Name == "MainMenu")
                buttonBack.Visibility = Visibility.Collapsed;
            else
                buttonBack.Visibility = Visibility.Visible;

            helpContent.Visibility = Visibility.Hidden;
            currentView = view as ISwitchable;
            mainContent.Children.Clear();
            mainContent.Children.Add(view);
        }

        public void Navigate(UserControl view, object state)
        {
            Navigate(view);

            if (currentView != null)
                currentView.UtilizeState(state);
        }

        private void KinectButtonTopBar(object sender, RoutedEventArgs e)
        {
            String name = ""; 
            try
            {
                name = ((Microsoft.Kinect.Toolkit.Controls.KinectTileButton)e.OriginalSource).Name;
            }
            catch(InvalidCastException e1)
            {
                try
                {
                    name = ((Microsoft.Kinect.Toolkit.Controls.KinectCircleButton)e.OriginalSource).Name;
                }
                catch(InvalidCastException e2)
                {
                    MessageBoxResult result = MessageBox.Show("Event isn't from kinect button");   
                }
            }
            
            switch (name)
            {
                case "buttonBack":
                    ViewSwitcher.Switch(mainMenu);
                    break;
                case "buttonExit":
                    helpContent.Visibility = Visibility.Hidden;
                    break;
                case "buttonHelp":
                    helpContent.Visibility = Visibility.Visible;
                    // TODO depended of current page
                    break;
            }
        }

        private void volumneChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(playerScreen != null)
            {
                playerScreen.mediaElement.Volume = sliderVolumn.Value / 10.0;
            }
        }
     
    }
}
