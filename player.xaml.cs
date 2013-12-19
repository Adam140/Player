using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        String[] filesList;
        String[] coversList;
        int currentFileIndex = 0;
        DispatcherTimer timer;

        private static player instance = null;

        static public player getInstance(String[] array)
        {
            if (instance == null)
                instance = new player(array);
            else
                instance.setFileList(array);

            return instance;
        }
        public player(String[] filesList)
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(400);
            timer.Tick += new EventHandler(timer_Tick);
            photoElement.Width = 100;
            photoElement.Height = 100;

            if (filesList == null || filesList.Length == 0 )
               this.filesList = Directory.GetFiles(MainWindow.mainDir + @"\Multimedia");
            else
                this.filesList = filesList;

            coversList = Directory.GetFiles(MainWindow.mainDir + "/Multimedia/covers", "*");
            filesList = (from file in this.filesList let name = System.IO.Path.GetFileNameWithoutExtension(file) where !name.StartsWith("cover_") select file).ToArray();
            typeOfMedia(this.filesList[0], false);
        }

        public void UtilizeState(object mainDir)
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
                    if (mediaElement.Source == null)
                        typeOfMedia(null, false);
                    mediaElement.Play();
                    timer.Start();

                    break;
                case "buttonStop":
                    mediaElement.Stop();
                    passedTime.Content = "00:00";
                    timer.Stop();
                    break;
                case "buttonPause":
                    mediaElement.Pause();
                    timer.Stop();
                    break;
                case "buttonRewind":
                    mediaElement.Position -= new TimeSpan(0, 0, 0, 2, 0); ;
                    break;
                case "buttonForward":
                    mediaElement.Position += new TimeSpan(0, 0, 0, 2, 0); ;
                    break;
                case "buttonPrevious":
                case "buttonFirst":
                    if (mediaElement.IsLoaded && mediaElement != null && mediaElement.Position.TotalSeconds > 2)
                    {
                        mediaElement.Stop();
                        mediaElement.Play();
                    }
                    else
                        nextToPlay(-1);
                    break;
                case "buttonLast":
                case "buttonNext":
                    nextToPlay(1);
                    break;

            }
        }

        //private void SeekToMediaPosition(object sender, RoutedPropertyChangedEventArgs<double> args)
        //{
        //    int SliderValue = (int)slider.Value;
        //    TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
        //    mediaElement.Position = ts;
        //}
            
        private void mediaOpened(object sender, EventArgs e)
        {
            slider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            passedTime.Content = "00:00";
            totalTime.Content = "/" + mediaElement.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = mediaElement.Position;
            slider.Value = ts.TotalMilliseconds;
            passedTime.Content = ts.ToString(@"mm\:ss");
        }

        private void mediaEnded(object sender, RoutedEventArgs e)
        {
            nextToPlay(1);
        }

        public void nextToPlay(int direction)
        {
            currentFileIndex += direction;
            if (currentFileIndex >= filesList.Length)
                currentFileIndex = 0;
            if (currentFileIndex < 0)
                currentFileIndex = filesList.Length - 1;
            typeOfMedia(filesList[currentFileIndex], true);
        }

        private void typeOfMedia(String file, Boolean playNow)
        {
            if (file == null || file.Equals(""))
                file = filesList[currentFileIndex];
            var contentType = System.IO.Path.GetExtension(file);

            if (Regex.Match(contentType, @"mp3").Success)  // musics
            {
                playerControlGrid.Visibility = Visibility.Visible;
                photoControlGrid.Visibility = Visibility.Hidden;
                photoElement.Visibility = Visibility.Visible;
                slider.Visibility = Visibility.Visible;
                timesLabel.Visibility = Visibility.Visible;
                mediaElement.Visibility = Visibility.Hidden;
                mediaElement.Source = new Uri(file, UriKind.Absolute);
                findAudioCover(file);
                if(playNow)
                    mediaElement.Play();
            }
            else if (Regex.Match(contentType, @"(wmv)|(mp4)").Success)    // video
            {
                mediaElement.Visibility = Visibility.Visible;
                playerControlGrid.Visibility = Visibility.Visible;
                photoControlGrid.Visibility = Visibility.Hidden;
                photoElement.Visibility = Visibility.Hidden;
                slider.Visibility = Visibility.Visible;
                timesLabel.Visibility = Visibility.Visible;
                mediaElement.Source = new Uri(file, UriKind.Absolute);
                if (playNow)
                    mediaElement.Play();
            }
            else if (Regex.Match(contentType, @"(jpg)|(gif)|(png)").Success)   // pictures
            {
                mediaElement.Stop();
                mediaElement.Close();
                mediaElement.Visibility = Visibility.Hidden;
                playerControlGrid.Visibility = Visibility.Hidden;
                photoControlGrid.Visibility = Visibility.Visible;
                slider.Visibility = Visibility.Collapsed;
                timesLabel.Visibility = Visibility.Collapsed;
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri(file, UriKind.Absolute);
                bi3.EndInit();
                photoElement.Source = bi3;
                photoElement.Visibility = Visibility.Visible;
            }

        }

        private void findAudioCover(String file)
        {
            TagLib.File tagFile = TagLib.File.Create(filesList[currentFileIndex]);
            string artist = tagFile.Tag.FirstAlbumArtist ?? tagFile.Tag.FirstArtist;
            string album = tagFile.Tag.Album;
            string title = tagFile.Tag.Title;

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            for(int i = 0; i < coversList.Length; i++)
            {
                String cover = coversList[i];
                if(cover.Contains(album))
                {
                    bi3.UriSource = new Uri(cover, UriKind.Absolute);
                    break;
                }
            }

            if(bi3.UriSource == null)
            {
                for (int i = 0; i < coversList.Length; i++)
                {
                    String cover = coversList[i];
                    if (cover.Contains(artist))
                    {
                        bi3.UriSource = new Uri(cover, UriKind.Absolute);
                        break;
                    }
                }
            }

            if (bi3.UriSource == null)
                bi3.UriSource = new Uri(@"Resources/blank_cover.png", UriKind.Relative);

            bi3.EndInit();
            photoElement.Source = bi3;
        }

        public static void Scale(Image img, int maxWidth, int maxHeight)
        {
            var st = (ScaleTransform)img.RenderTransform;
            double zoom = .2;
            st.ScaleX += zoom;
            st.ScaleY += zoom;
        }
        public void setFileList(String[] fileList)
        {
            if (fileList != null)
                this.filesList = filesList;
        }
    }
}
