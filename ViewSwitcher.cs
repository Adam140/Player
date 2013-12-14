using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Player
{
    public static class ViewSwitcher
    {

        private static MainWindow window;

        /* Set the main window whose content will change */
        public static void SetMainWindow(MainWindow w)
        {
            window = w;
        }

        /* Change to the new screen without taking any state into account */
        public static void Switch(UserControl newPage)
        {
            window.Navigate(newPage);
        }

        /* Change to the new screen and also take into account a certain state */
        public static void Switch(UserControl newPage, object state)
        {
            window.Navigate(newPage, state);
        }
    }
}
