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
using System.Windows.Shapes;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
namespace Player
{
    /// <summary>
    /// Interaction logic for Playlist.xaml
    /// </summary>
    public partial class Playlist : UserControl, ISwitchable
    {
        String[] audioList;

        public Playlist()
        {
        }

        public Playlist(string mainDir)
        {
            InitializeComponent();
            audioList = Directory.GetFiles(mainDir + "/Audio");
            //aint i = 0;
            updateAudioList();
            //TagLib.File tagFile = TagLib.File.Create(audioList[i]);
            //String artist = tagFile.Tag.FirstAlbumArtist;
            //String album = tagFile.Tag.Album;
            //String title = tagFile.Tag.Title;
            //imgPreview.Source = new BitmapImage(new Uri(@"/Multimedia/Covers/Dio - Holy Diver.jpg", UriKind.Relative));

            
        }

        private void updateAudioList()
        {
            for (int i = 0; i < audioList.Length; i++)
            {
                var btn = new KinectCircleButton
                {
                    Name = "audio" + i.ToString(),
                    Content = new Label { Content = audioList[i], FontSize=50 },
                    Height = 200,
                    //FontSize = 36
                };
                btn.Click += songClicked;
                btn.MouseEnter += songHover;
                KinectRegion.AddHandPointerEnterHandler(btn, this.songClicked);
                KinectRegion.AddHandPointerLeaveHandler(btn, this.songClicked);
                //kinectRegion.Ad
                scrollList.Children.Add(btn);
            }

        }

        private void songHover(object sender, RoutedEventArgs args)
        {
            string str = (sender as KinectCircleButton).Name.ToString();
            this.updateInfo(str);
        }

        private void songClicked(object sender, RoutedEventArgs args)
        {
            string str = (sender as KinectCircleButton).Name.ToString();
            string tmp = str.Replace("audio","");
            int i = Convert.ToInt32(tmp);
                var btn = new KinectCircleButton
                {
                    Name = "chosen" + str,
                    Content = new Label { Content = audioList[i], FontSize=50 },
                    Height = 200,
                    //FontSize = 36
                };
                scrollChosenList.Children.Add(btn);
            this.updateInfo(str);
        }

        public void test()
        {
            lbl_album.Content = "123";
            lbl_title.Content = "123";
            lbl_type.Content =  "123";
        }

        public void updateInfo(string path)
        {
            path = path.Replace("audio", "");
            path = audioList[Convert.ToInt32(path)];
            TagLib.File tagFile = TagLib.File.Create(path);
            string album = tagFile.Tag.Album;
            lbl_album.Content = album;
            lbl_title.Content = tagFile.Tag.Title;
            lbl_type.Content = tagFile.Tag.FirstGenre;
            string artist = tagFile.Tag.FirstArtist;
            imgPreview.Source = new BitmapImage(new Uri(@"/Multimedia/Covers/" + artist + " - " + album + ".jpg", UriKind.Relative));

        }

        public string presentAudioFile(string path)
        {
            TagLib.File tagFile = TagLib.File.Create(path);
            string artist = tagFile.Tag.FirstArtist;
            //string album = tagFile.Tag.Album;
            string album = "zxc";
            string title = tagFile.Tag.Title;
            //string genre = tagFile.Tag.FirstGenre;
            return artist + " - " + title + "(" + album + ")";
            
        }


        public void Destroy()
        {

        }

        public void UtilizeState(object mainDir)
        {

        }
    }
}
