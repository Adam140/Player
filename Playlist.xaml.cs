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
        KinectTileButton activeElement = null;
        List<AudioFile> audioFiles = new List<AudioFile>();


        public Playlist()
        {
        }

        public Playlist(string mainDir)
        {
            InitializeComponent();
            audioList = Directory.GetFiles(mainDir + "/Audio");
            coverList = Directory.GetFiles(mainDir + "/Covers");
            //aint i = 0;
            updateAudioList();
            for (int i = 0; i < audioList.Length; i++)
            {

                audioFiles.Add(new AudioFile(i, audioList[i], 0));
            }
            //TagLib.File tagFile = TagLib.File.Create(audioList[i]);
            //String artist = tagFile.Tag.FirstAlbumArtist;
            //String album = tagFile.Tag.Album;
            //String title = tagFile.Tag.Title;
            imgPreview.Source = new BitmapImage(new Uri(@"/Multimedia/Covers/ACDC - The Very Best.jpg", UriKind.Relative));
            
        }

        public Playlist(string mainDir, String[] chosenList) : this(mainDir)
        {
            this.chosenList = new ArrayList(chosenList.Length);
            this.chosenList.AddRange(chosenList);
            updateChosenList();
            
        }

        public void updateAudioList()
        {
            centerLabel.Content = "Avaliable Songs";
            this.clearScrollList();
            bool add_new = true;
            //if (audioList.Length != 0)
            //{
            //    add_new = false;
            //}
            for (int i = 0; i < audioList.Length; i++)
            {
                //if(add_new)
                //    audioFiles.Add(new AudioFile(i, audioList[i], 0));
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

        public void updateAudioListTop10()
        {
            centerLabel.Content = "TOP Songs";
            updateChosenList();
            this.clearScrollList();
            string[] tmp = this.getTop10();
            for (int i = 0; i < tmp.Length; i++)
            {
                //audioFiles.Add(new AudioFile(i, audioList[i], 0));
                KinectTileButton btn = presentAudioFile(tmp[i]);
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


        private void clearScrollList()
        {
            scrollList.Children.Clear();

        }

        private void updateChosenList()
        {
            scrollChosenList.Children.Clear();
            for (int i = 0; i < chosenList.Count; i++)
            {
                KinectTileButton btn = presentAudioFile(chosenList[i].ToString());
                btn.Name = "audio" + i.ToString();
                btn.Click += removeSong;
                btn.MouseEnter += songHover;
                KinectRegion.AddHandPointerEnterHandler(btn, this.songHover);
                scrollChosenList.Children.Add(btn);
                //this.updateInfo(btn.Name);

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
            KinectTileButton btn = presentAudioFile(i);
            btn.Click += removeSong;
            btn.MouseEnter += songHover;
            btn.Name = (sender as KinectTileButton).Name.ToString();
            KinectRegion.AddHandPointerEnterHandler(btn, this.songHover);

            AudioFile a = audioFiles.Find(j => j.id == i);
            a.clicked++;
   
            scrollChosenList.Children.Add(btn);
            MainWindow.chosenSongs = this.getChosenSongs();
            this.updateInfo(str);

        }

        private void removeSong(object sender, RoutedEventArgs args)
        {
            scrollChosenList.Children.Remove((sender as KinectTileButton));
            string str = (sender as KinectTileButton).Name.ToString();
            string tmp = str.Replace("audio","");
            
            int i = Convert.ToInt32(tmp);
            chosenList.Remove(audioList[i]);
            MainWindow.chosenSongs = this.getChosenSongs();

        }

        private void activeSong(object sender, RoutedEventArgs args)
        {
            setActiveSong(sender as KinectTileButton);
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
            string album = tagFile.Tag.Album;
            //string album = "zxc";
            string title = tagFile.Tag.Title;
            //string genre = tagFile.Tag.FirstGenre;
            Grid grid = new Grid();
            grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            grid.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            //grid.Width = new GridLength(1, GridUnitType.Star);
            // Create column definitions.
            //ColumnDefinition columnDefinition1 = new ColumnDefinition();
            //ColumnDefinition columnDefinition2 = new ColumnDefinition();
            //columnDefinition1.Width = new GridLength(0.4, GridUnitType.Star);
            //columnDefinition2.Width = new GridLength(0.6, GridUnitType.Star);

            // Create row definitions.
            RowDefinition rowDefinition1 = new RowDefinition();
            rowDefinition1.Height = new GridLength(1.0, GridUnitType.Star);

            // Attached definitions to grid.
            //grid.ColumnDefinitions.Add(columnDefinition1);
            //grid.ColumnDefinitions.Add(columnDefinition2);
            //grid.RowDefinitions.Add(rowDefinition1);
            //grid.Background = Brushes.Green;


            Image img = new Image { Source = getAudioCoverBitmap(path) };
            img.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            img.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

            StackPanel panel2 = new StackPanel();
            //panel.Background = Brushes.Bisque;
            panel2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            panel2.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            panel2.Children.Add(img);
            grid.Children.Add(panel2);
            //Grid.SetColumn(img, 0);
            //Grid.SetRow(img, 0);

            int fontSize = 30;
            var fontColor = Brushes.White;

            StackPanel panel = new StackPanel();
            //panel.Background = Brushes.Bisque;
            panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            panel.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            panel.Children.Add(new Label { Content = artist, Foreground = fontColor, FontSize = fontSize });
            panel.Children.Add(new Label { Content = title, Foreground = fontColor, FontSize = fontSize });
            panel.Children.Add(new Label { Content = album, Foreground = fontColor, FontSize = fontSize });
            //panel.Background.Opacity = 0.7;
            SolidColorBrush c = new SolidColorBrush();
            c.Color = Color.FromRgb(0,0,0);
            c.Opacity = 0.7;
            panel.Background = c;

            //Border myBorder1 = new Border();
            ////myBorder1.Background = Brushes.SkyBlue;
            //myBorder1.BorderBrush = Brushes.SkyBlue;
            //myBorder1.BorderThickness = new Thickness(5);
            //myBorder1.Child = panel;


            //panel.Background = Brushes.Blue;
            grid.Children.Add(panel);
            //Grid.SetColumn(panel, 1);
            //Grid.SetRow(panel, 0);
            //grid.Background = Brushes.Bisque;
            KinectTileButton kin = new KinectTileButton { Content = grid };
            kin.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            kin.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            kin.Width = allSongsGrid.Width;
            kin.Background = Brushes.Black ;
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


        private void KinectTileButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.chosenList.Count > 0)
            {
                this.setPlayerVisible();
                foreach (object child in scrollChosenList.Children)
                {
                    (child as KinectTileButton).Click -= removeSong;
                    (child as KinectTileButton).Click += activeSong;
                }
            }
        }

        private void KinectTileButton_Click_2(object sender, RoutedEventArgs e)
        {
            this.setPlaylistVisible();
            this.updateAudioList();
            ViewSwitcher.Switch(this);
        }

        public void setPlayerVisible()
        {
            songInformationGrid.Visibility = System.Windows.Visibility.Collapsed;
            songCover.Visibility = System.Windows.Visibility.Collapsed;
            allSongsGrid.Visibility = System.Windows.Visibility.Collapsed;
            playerContainer.Children.Add(player.getInstance(getChosenSongs()));
            playerContainer.Visibility = System.Windows.Visibility.Visible;
            playlistPlay.Visibility = System.Windows.Visibility.Collapsed;
            backToPlaylist.Visibility = System.Windows.Visibility.Visible;
        }

        public void setPlaylistVisible()
        {
            songInformationGrid.Visibility = System.Windows.Visibility.Visible;
            songCover.Visibility = System.Windows.Visibility.Visible;
            allSongsGrid.Visibility = System.Windows.Visibility.Visible;
            playerContainer.Children.Add(player.getInstance(getChosenSongs()));
            playerContainer.Visibility = System.Windows.Visibility.Collapsed;
            playlistPlay.Visibility = System.Windows.Visibility.Visible;
            backToPlaylist.Visibility = System.Windows.Visibility.Collapsed;
            foreach (object child in scrollChosenList.Children)
            {
                (child as KinectTileButton).Click += removeSong;
                (child as KinectTileButton).Click -= activeSong;
            }
        }

        public void setActiveSong(int index)
        {
            UIElement tmp = scrollChosenList.Children[index];
            KinectTileButton btn = (tmp as KinectTileButton);
            setActiveSong(btn);
            
        }

        public void setActiveSong(KinectTileButton btn)
        {
            btn.BorderBrush = Brushes.Gold;
            btn.BorderThickness = new Thickness(5.0);

            if (this.activeElement != null)
                this.activeElement.BorderThickness = new Thickness(0.0);
            
            this.activeElement = btn;

        }

        public string[] getTop10()
        {
            string[] sorted = new string[audioFiles.Count];
            audioFiles = audioFiles.OrderByDescending(o => o.clicked).ToList();
            for (int i = 0; i < audioFiles.Count; i++)
            {
                sorted[i] = audioFiles.ElementAt(i).path;
            }
            return sorted;
        }
    }
}
