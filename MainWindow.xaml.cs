using Microsoft.Kinect.Toolkit;
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
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Controls;
using Fizbin.Kinect.Gestures;
using System.Threading;

namespace Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ISwitchable currentView;
        String currentViewName;

        private readonly KinectSensorChooser sensorChooser;
        private GestureGenerator gestureGenerator;

        public Menu mainMenu;
        //public Playlist playlist;
        public static string[] chosenSongs;
        //public static string mainDir = @"D:\Studia\player";
        public static string mainDir = @"C:\Users\Adam\Desktop";

        public MainWindow()
        {
            InitializeComponent();
            ViewSwitcher.SetMainWindow(this);
            ViewSwitcher.Switch(mainMenu = new Menu(this));

            gestureGenerator = new GestureGenerator();
            // Listen to recognized gestures
            gestureGenerator.GestureRecognized += gestureGenerator_GestureRecognized;

            this.sensorChooser = new KinectSensorChooser();
            //this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.Start();

            var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);

            buttonMinus.MouseEnter += this.volumneChanged;
            buttonPlus.MouseEnter += this.volumneChanged;
            KinectRegion.AddHandPointerEnterHandler(buttonMinus, this.volumneChanged);
            //KinectRegion.AddHandPointerLeaveHandler(buttonMinus, this.volumneChanged);

            KinectRegion.AddHandPointerEnterHandler(buttonPlus, this.volumneChanged);
            //KinectRegion.AddHandPointerLeaveHandler(buttonPlus, this.volumneChanged);
          

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

            if (view.Name == "MainScreen")
                buttonBack.Visibility = Visibility.Collapsed;
            else
                buttonBack.Visibility = Visibility.Visible;

            helpContent.Visibility = Visibility.Hidden;
            currentView = view as ISwitchable;
            currentViewName = view.Name;
            mainContent.Children.Clear();
            mainContent.Children.Add(view);
            //if ("PlayerScreen".Equals(currentViewName))
            //    ((player)view).nextToPlay(0);
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
                case "buttonMute":
                    sliderVolumn.Value = 0;
                    player.getInstance(null).mediaElement.Volume = 0;
                    break;
                case "buttonVolumne":
                    sliderVolumn.Value = 10.0;
                    player.getInstance(null).mediaElement.Volume = 1;;
                    break;
                case "buttonBack":
                    ViewSwitcher.Switch(mainMenu);
                    break;
                case "buttonExit":
                    helpContent.Visibility = Visibility.Hidden;
                    break;
                case "buttonHelp":
                    helpContent.Visibility = Visibility.Visible;
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    //TODO fix below if condition to looking if file exists in relative path
                    if( System.IO.File.Exists(MainWindow.mainDir + @"/Resources/Help_pages/" + currentViewName + ".png"))
                        bi.UriSource = new Uri(@"Resources/Help_pages/" + currentViewName + ".png", UriKind.Relative);   
                    else
                        bi.UriSource = new Uri(@"Resources/Help_pages/under-construction.gif", UriKind.Relative);
                    
                        bi.EndInit();
                        imageHelp.Source = bi;
                    break;
                case "buttonLanguage0":
                    if(languageContent.Visibility == Visibility.Visible)
                        languageContent.Visibility = Visibility.Collapsed;
                    else
                        languageContent.Visibility = Visibility.Visible;
                break;
            }
        }

        private void volumneChanged(object sender, RoutedEventArgs args)
        {
            string senderName = (sender as KinectTileButton).Name.ToString();
            Boolean isEnter = args.RoutedEvent.Name == "HandPointerEnter";

            switch (senderName)
            {
                case "buttonMinus":
                        volumneChanged(-1);
                       break;
                case "buttonPlus":
                        //if (isEnter)
                        //    volumneChanged(1);
                        //else
                            volumneChanged(1);
                    break;
            }
        }

        private void volumneChanged(int direction)
        {
            //if (direction * 0.5 + sliderVolumn.Value > 0 && direction * 0.5 + sliderVolumn.Value < 10)
            sliderVolumn.Value += direction * 0.5;
            player.getInstance(null).mediaElement.Volume = sliderVolumn.Value / 10.0;
        }

        /// <summary>
        /// Called when the KinectSensorChooser gets a new sensor
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="args">event arguments</param>
        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            // Initialize Gesture Generator
            gestureGenerator.Initialize(args.OldSensor, args.NewSensor);

            // Handle old and new sensor normally..
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }

            if (args.NewSensor != null)
            {
                try
                {
                    gestureGenerator.Initialize(args.OldSensor, args.NewSensor);

                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();

                    try
                    {
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                    }
                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    }
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }
        }

        void gestureGenerator_GestureRecognized(GestureType gestureType, int trackingId)
        {
            if (helpContent.Visibility == Visibility.Visible)
            {
                if (gestureType == GestureType.SwipeLeft)
                {
                    helpContent.Visibility = Visibility.Hidden;
                    
                }
             return;
            }
            player p = player.getInstance(null);
            switch (gestureType)
            {
                case GestureType.JoinedHands:
                    if ("PlaylistScreen".Equals(currentViewName) && Playlist.Instance.scrollChosenList.Children.Count > 0)
                    {
                        if (Playlist.Instance.backToPlaylist.Visibility == System.Windows.Visibility.Visible)
                        {
                            Playlist.Instance.setPlaylistVisible();
                            Playlist.Instance.updateAudioList();
                            //ViewSwitcher.Switch(playlist);

                        }
                        else
                        {
                            Playlist.Instance.setPlayerVisible();
                            Playlist.Instance.updateAudioList();
                        }
                    }
                    break;
                case GestureType.SwipeDownLeft:
                    if(helpContent.Visibility == Visibility.Visible)
                        helpContent.Visibility = Visibility.Hidden;
                    //else if(buttonBack.Visibility == Visibility.Visible)
                      //  ViewSwitcher.Switch(mainMenu);
                    break;
                case GestureType.WaveLeft:
                    if (("PlayerScreen".Equals(currentViewName) || "PlaylistScreen".Equals(currentViewName)) && p.mediaElement.IsLoaded && p.playerControlGrid.Visibility == Visibility.Visible)
                    {
                        p.mediaElement.Position += new TimeSpan(0, 0, 0, 5, 0); ;
                    }
                    break;
                case GestureType.WaveRight:
                    if (("PlayerScreen".Equals(currentViewName) || "PlaylistScreen".Equals(currentViewName)) && p.mediaElement.IsLoaded && p.playerControlGrid.Visibility == Visibility.Visible)
                    {
                        p.mediaElement.Position += new TimeSpan(0, 0, 0, 5, 0); ;
                    }
                    break;
                case GestureType.ZoomIn:
                    if ("PlayerScreen".Equals(currentViewName) || "PlaylistScreen".Equals(currentViewName))
                    {
                        if (p.currentPhoto)
                        {
                            p.photoElement.Height = p.photoElement.Height * 2;
                            p.photoElement.Width = p.photoElement.Width * 2;
                        }
                        else if (p.mediaElement != null && p.isPlaying)
                            p.mediaElement.Pause();
                    }
                    break;
                case GestureType.ZoomOut:
                    if ("PlayerScreen".Equals(currentViewName) || "PlaylistScreen".Equals(currentViewName))
                    {
                        if (p.currentPhoto)
                        {
                            p.photoElement.Height = p.photoElement.Height * 0.5;
                            p.photoElement.Width = p.photoElement.Width * 0.5;
                        }
                        else if (p.mediaElement != null && !p.isPlaying)
                            p.playVideo();
                    }
                    break;
                case GestureType.SwipeLeft:
                    if (("PlayerScreen".Equals(currentViewName) || "PlaylistScreen".Equals(currentViewName)))
                    {
                        if (p.mediaElement != null && p.mediaElement.Position.TotalSeconds > 2)
                        {
                            p.mediaElement.Stop();
                            p.mediaElement.Play();
                        }
                        else
                            p.nextToPlay(-1);
                    }
                    break;
                case GestureType.SwipeRight:
                    if (("PlayerScreen".Equals(currentViewName) || "PlaylistScreen".Equals(currentViewName)) && p.mediaElement.IsLoaded && p.playerControlGrid.Visibility == Visibility.Visible)
                    {
                       p.nextToPlay(1);
                    }
                    break;

                default:
                    break;
            }
        }

     
    }
}
