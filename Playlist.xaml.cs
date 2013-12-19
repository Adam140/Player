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
using System.Collections;
namespace Player
{
    /// <summary>
    /// Interaction logic for Playlist.xaml
    /// </summary>
    public partial class Playlist : UserControl, ISwitchable
    {
        String[] audioList;
        String[] coverList;
        ArrayList chosenList = new ArrayList();

        public Playlist()
        {
        }

        public Playlist(string mainDir)
        {
            InitializeComponent();
            audioList = Directory.GetFiles(mainDir);
            coverList = Directory.GetFiles(mainDir + "/Covers");
            //aint i = 0;
            updateAudioList();
            //TagLib.File tagFile = TagLib.File.Create(audioList[i]);
            //String artist = tagFile.Tag.FirstAlbumArtist;
            //String album = tagFile.Tag.Album;
            //String title = tagFile.Tag.Title;


            
        }

        private void updateAudioList()
        {
            for (int i = 0; i < audioList.Length; i++)
            {
                KinectTileButton btn = presentAudioFile(audioList[i]);
                //KinectTileButton btn = new KinectTileButton{Name="audio" + i.ToString(), Cl}
                btn.Name = "audio" + i.ToString();
                //btn.Height = 200;
                //{
                //    Name = "audio" + i.ToString(),
                //    Content = new Label { Content = audioList[i], FontSize=50 },
                //    Height = 200,
                //    //FontSize = 36
                //};
                //btn.Click
                btn.Click += songClicked; 
                btn.MouseEnter += songHover;
                //btn.GotFocus += songHover;
                //btn.TouchEnter += songHover;
                KinectRegion.AddHandPointerEnterHandler(btn, this.songHover);
                //KinectRegion.AddHandPointerLeaveHandler(btn, this.songClicked);
                //kinectRegion.Ad
                scrollList.Children.Add(btn);
                //scrollList.Children.Add(presentAudioFile(audioList[i]));
            }

        }

        private void songHover(object sender, RoutedEventArgs args)
        {
            string str = (sender as KinectTileButton).Name.ToString();
            this.updateInfo(str);
        }

        private void songClicked(object sender, RoutedEventArgs args)
        {
            string str = (sender as KinectTileButton).Name.ToString();
            string tmp = str.Replace("audio","");
            
            int i = Convert.ToInt32(tmp);
            chosenList.Add(audioList[i]);
                var btn = new KinectCircleButton
                {
                    Name = "chosen" + str,
                    Content = new Label { Content = audioList[i], FontSize=50 },
                    Height = 200,
                    //FontSize = 36
                };
                scrollChosenList.Children.Add(presentAudioFile(i));
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
            //imgPreview.Source = new BitmapImage(new Uri(@getAudioCover(path), UriKind.Relative));
            imgPreview.Source = getAudioCoverBitmap(path);


        }

        public KinectTileButton presentAudioFile(int i)
        {
            return presentAudioFile(audioList[i]);
        }

        public KinectTileButton presentAudioFile(string path)
        {
            TagLib.File tagFile = TagLib.File.Create(path);
            string artist = tagFile.Tag.FirstArtist;
            //string album = tagFile.Tag.Album;
            string album = "zxc";
            string title = tagFile.Tag.Title;
            //string genre = tagFile.Tag.FirstGenre;
            Grid grid = new Grid();

            // Create column definitions.
            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            ColumnDefinition columnDefinition2 = new ColumnDefinition();
            columnDefinition1.Width = new GridLength(0.4, GridUnitType.Star);
            columnDefinition2.Width = new GridLength(0.6, GridUnitType.Star);

            // Create row definitions.
            RowDefinition rowDefinition1 = new RowDefinition();
            rowDefinition1.Height = new GridLength(1, GridUnitType.Star);

            // Attached definitions to grid.
            grid.ColumnDefinitions.Add(columnDefinition1);
            grid.ColumnDefinitions.Add(columnDefinition2);
            grid.RowDefinitions.Add(rowDefinition1);

            Image img = new Image { Source = getAudioCoverBitmap(path) };
            grid.Children.Add(img);
            Grid.SetColumn(img, 0);
            Grid.SetRow(img, 0);
            grid.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

            StackPanel panel = new StackPanel();
            panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            panel.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            panel.Children.Add(new Label{ Content = artist });
            panel.Children.Add(new Label { Content = title });
            panel.Children.Add(new Label { Content = album });
            //panel.Background = Brushes.Blue;
            grid.Children.Add(panel);
            Grid.SetColumn(panel, 1);
            Grid.SetRow(panel, 0);
            //grid.Background = Brushes.Bisque;
            KinectTileButton kin = new KinectTileButton { Content = grid };
            kin.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            kin.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            kin.Width = panel.Width;
            return kin;
            //return artist + " - " + title + "(" + album + ")";
            
        }

        public string getAudioCover(string path)
        {
            TagLib.File tagFile = TagLib.File.Create(path);
            //string artist = tagFile.Tag.FirstArtist;
            string album = tagFile.Tag.Album;
            string cover;
            for (int i = 0; i < coverList.Length; i++)
            {
                if (coverList[i].Contains(album))
                    return coverList[i];
            }
            return "";

        }

        public BitmapImage getAudioCoverBitmap(string path)
        {
            return new BitmapImage(new Uri(@getAudioCover(path), UriKind.RelativeOrAbsolute));
        }

        public String[] getChosenSongs()
        {
            return (string[])chosenList.ToArray(typeof(string));
        }


        public void Destroy()
        {

        }

        public void UtilizeState(object mainDir)
        {

        }

        private void KinectCircleButton_Click_1(object sender, RoutedEventArgs e)
        {
            songInformationGrid.Visibility = System.Windows.Visibility.Collapsed;
            songCover.Visibility = System.Windows.Visibility.Collapsed;
            allSongsGrid.Visibility = System.Windows.Visibility.Collapsed;
            playerContainer.Children.Add(new player(getChosenSongs()));
            playerContainer.Visibility = System.Windows.Visibility.Visible;

        }
    }
}
