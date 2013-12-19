﻿using Microsoft.Kinect.Toolkit;
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
        public player playerScreen;
        public Playlist playlist;
        public static string[] chosenSongs;
        //public static string mainDir = @"D:\Studia\player\";
        public static string mainDir = @"C:\Users\Adam\Documents\GitHub\Player";

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
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    //TODO fix below if condition to looking if file exists in relative path
                    if( System.IO.File.Exists(MainWindow.mainDir + @"Resources/Help_pages/" + currentViewName + ".png"))
                        bi.UriSource = new Uri(@"Resources/Help_pages/" + currentViewName + ".png", UriKind.Relative);   
                    else
                        bi.UriSource = new Uri(@"Resources/Help_pages/under-construction.gif", UriKind.Relative);
                    
                        bi.EndInit();
                        imageHelp.Source = bi;
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
            switch (gestureType)
            {
                case GestureType.SwipeLeft:
                    if(helpContent.Visibility == Visibility.Visible)
                        helpContent.Visibility = Visibility.Hidden;
                    else if(buttonBack.Visibility == Visibility.Visible)
                        ViewSwitcher.Switch(mainMenu);
                    break;
                case GestureType.WaveLeft:
                    if(playerScreen.mediaElement.IsLoaded && playerScreen.playerControlGrid.Visibility == Visibility.Visible)
                    {
                        playerScreen.mediaElement.Position += new TimeSpan(0, 0, 0, 2, 0); ;
                    }
                    break;
                case GestureType.WaveRight:
                    if (playerScreen.mediaElement.IsLoaded && playerScreen.playerControlGrid.Visibility == Visibility.Visible)
                    {
                        playerScreen.mediaElement.Position += new TimeSpan(0, 0, 0, 2, 0); ;
                    }
                    break;
                case GestureType.ZoomIn:
                    if(playerScreen.photoElement.Visibility == Visibility.Visible)
                    {
                        playerScreen.photoElement.Height = playerScreen.photoElement.Height * 2;
                        playerScreen.photoElement.Width = playerScreen.photoElement.Width * 2;
                    }
                    break;
                case GestureType.ZoomOut:
                    if (playerScreen.photoElement.Visibility == Visibility.Visible)
                    {
                        playerScreen.photoElement.Height = playerScreen.photoElement.Height * 0.5;
                        playerScreen.photoElement.Width = playerScreen.photoElement.Width * 0.5;
                    }
                    break;
                case GestureType.SwipeDownLeft:
                    if (playerScreen.mediaElement.IsLoaded && playerScreen.playerControlGrid.Visibility == Visibility.Visible)
                    {
                        if (playerScreen.mediaElement != null && playerScreen.mediaElement.Position.TotalSeconds > 2)
                        {
                            playerScreen.mediaElement.Stop();
                            playerScreen.mediaElement.Play();
                        }
                        else
                            playerScreen.nextToPlay(-1);
                    }
                    break;
                case GestureType.SwipeRight:
                    if (playerScreen.mediaElement.IsLoaded && playerScreen.playerControlGrid.Visibility == Visibility.Visible)
                    {
                       playerScreen.nextToPlay(1);
                    }
                    break;

                default:
                    break;
            }
        }
     
    }
}
