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
using System.Windows.Resources;
using System.Windows.Shapes;

namespace Player
{
    /// <summary>
    /// Interaction logic for screen1.xaml
    /// </summary>
    public partial class player : UserControl, ISwitchable
    {
        public player()
        {
            InitializeComponent();
        }

        public void UtilizeState(object state)
        {

        }

        public void Destroy()
        {

        }

        private void KinectButtonPlayer(object sender, RoutedEventArgs e)
        {

        }
    }
}
