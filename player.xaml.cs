using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace Player
{
    /// <summary>
    /// Interaction logic for screen1.xaml
    /// </summary>
    public partial class player : UserControl, ISwitchable
    {
        static String dir = @"C:\Users\Adam\Documents\GitHub\Player\Multimedia";
        String[] filesList = Directory.GetFiles(dir);
        int currentFileIndex = 0;
        Boolean isDragging = false;
        DispatcherTimer timer;

        public player()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += new EventHandler(timer_Tick);
        }

        public void UtilizeState(object state)
        {

        }

        public void Destroy()
        {
            mediaElement.Stop();
            mediaElement.Close();
        }

        private void KinectButtonPlayer(object sender, RoutedEventArgs e)
        {
            String name = ((Microsoft.Kinect.Toolkit.Controls.KinectCircleButton)e.OriginalSource).Name;
            switch (name)
            {
                case "buttonPlay":
                    if(mediaElement.Source == null)
                    {
                        mediaElement.Source = new Uri(filesList[currentFileIndex], UriKind.Relative);
                        slider.Visibility = Visibility.Visible;
                    }
                    mediaElement.Play();
                    timer.Start();
                   
                    break;
                case "buttonStop":
                    mediaElement.Stop();
                    passedTime.Content = mediaElement.Position.ToString(@"mm\:ss");
                    timer.Stop();
                    break;
                case "buttonPause":
                    mediaElement.Pause();
                    timer.Stop();
                    break;
                case "buttonRewind":
                    mediaElement.Position -= new TimeSpan(0, 0, 0, 2, 0);;
                    break;
                case "buttonForward":
                    mediaElement.Position += new TimeSpan(0, 0, 0, 2, 0);;
                    break;
                case "buttonFirst":
                    if (mediaElement.Position.TotalSeconds > 2)
                    {
                        mediaElement.Stop();
                        mediaElement.Play();
                    }
                    else
                        nextToPlay(-1);
                    break;
                case "buttonLast":
                    nextToPlay(1);
                    break;
                        
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            videoControlGrid.Visibility = Visibility.Hidden;
            photoControlGrid.Visibility = Visibility.Visible;
        }

        private void SeekToMediaPosition(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            int SliderValue = (int)slider.Value;
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
            mediaElement.Position = ts;
        }
        private void mediaOpened(object sender, EventArgs e)
        {
            slider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            totalTime.Content = "/" + mediaElement.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
        }

        void timer_Tick(object sender, EventArgs e)
        {
                slider.Value = mediaElement.Position.TotalMilliseconds;
                passedTime.Content = mediaElement.Position.ToString(@"mm\:ss");
        }

        private void mediaEnded(object sender, RoutedEventArgs e)
        {
            nextToPlay(1);
        } 

        private void nextToPlay(int direction)
        {
            currentFileIndex += direction;
            if (currentFileIndex >= filesList.Length)
                currentFileIndex = 0;
            if (currentFileIndex < 0)
                currentFileIndex = filesList.Length - 1;
            mediaElement.Source = new Uri(filesList[currentFileIndex], UriKind.Relative);
            mediaElement.Play();
        }
 
    }
}
