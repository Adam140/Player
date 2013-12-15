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

        public MainWindow()
        {
            InitializeComponent();
            ViewSwitcher.SetMainWindow(this);
            ViewSwitcher.Switch(new player());
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

        private void KinectButtonLanguage(object sender, RoutedEventArgs e)
        {
            Console.Out.WriteLine("Pressed the button language ");

        }
        private void KinectButtonHelp(object sender, RoutedEventArgs e)
        {
            Console.Out.WriteLine("Pressed the button help ");

        }
    }
}
