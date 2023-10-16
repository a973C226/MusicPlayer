using MusicPlayer.UserControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using NAudio;
using NAudio.WaveFormRenderer;
using NAudio.Wave;
using System.Windows.Media;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Drawing;
using MusicPlayer.Properties;
using System.Runtime;

namespace MusicPlayer
{
    public partial class MainWindow : Window
    {

        private DispatcherTimer timer = new DispatcherTimer();
        private static bool playingNow = false;

        private string popularTracksDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"music\Popular\");
        public static List<string> popularTracksNames = new List<string>();
        public static List<string> popularTracksPaths = new List<string>();
        

        private string favoriteTracksDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"music\Favorite\");
        public static List<string> favoriteTracksNames = new List<string>();
        public static List<string> favoriteTracksPaths = new List<string>();

        public string nowPlaylistPlay = "";

        public static string nowTrackName = "";
        public static string nowTrackPath = "";


        public MainWindow()
        {
            InitializeComponent();
        }

        private FileInfo[] getTracks(string dirPath)
        {
            DirectoryInfo popularTracksDir = new DirectoryInfo(dirPath);
            return popularTracksDir.GetFiles("*.mp3");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 1);

            try
            {
                int UCIndex = 0;
                foreach (FileInfo file in getTracks(popularTracksDirPath))
                {
                    popularTracksNames.Add(file.Name);
                    popularTracksPaths.Add(file.FullName);
                }

                foreach (PopularSong popularTrackUC in popularTracksPanel.Children)
                {
                    popularTrackUC.Title = popularTracksNames[UCIndex];
                    popularTrackUC.Path = popularTracksPaths[UCIndex];
                    UCIndex++;
                }

                UCIndex = 0;

                foreach (FileInfo file in getTracks(favoriteTracksDirPath))
                {
                    favoriteTracksNames.Add(file.Name);
                    favoriteTracksPaths.Add(file.FullName);
                }

                foreach (SongItem playlistTrackUC in playlistTracksPanel.Children)
                {
                    playlistTrackUC.Title = favoriteTracksNames[UCIndex];
                    playlistTrackUC.Path = favoriteTracksPaths[UCIndex];
                    UCIndex++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("The process failed: {0}", ex.ToString());
            }
        }

        public void playTrack(string trackPath, string trackName)
        {
            try
            {
                musicMediaElement.Source = new Uri(trackPath, UriKind.Relative);
                nowTrackPath = trackPath;
                nowTrackName = trackName;
                nowFilename.Text = trackName;
                musicMediaElement.Play();
                playingNow = true;
                timer.Start();
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void musicMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            musicProgress.Value = 0;
            double trackLength = musicMediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            musicProgress.Maximum = trackLength;
            maxTrackTime.Text = new TimeSpan(0, 0, (int)trackLength).ToString().Substring(3);
            volumeSlider.Maximum = 1;
        }

        private void musicProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!playingNow)
                musicMediaElement.Position = new TimeSpan(0, 0, (int)musicProgress.Value);
        }

        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            musicMediaElement.Volume = volumeSlider.Value;
        }

        private void timerTick(object sender, EventArgs e)
        {
            playingNow = true;
            musicProgress.Value += 1;
            nowTrackTime.Text = new TimeSpan(0, 0, (int)musicProgress.Value).ToString().Substring(3);
            playingNow = false;
        }


        private void pauseBtn_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            musicMediaElement.Pause();
            pauseBtn.Visibility = Visibility.Collapsed;
            playBtn.Visibility = Visibility.Visible;
        }

        private void playBtn_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            musicMediaElement.Play();
            pauseBtn.Visibility = Visibility.Visible;
            playBtn.Visibility = Visibility.Collapsed;
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            switch (nowPlaylistPlay)
            {
                case "Popular":
                    if (popularTracksNames.IndexOf(nowTrackName) + 1 < popularTracksNames.Count)
                    {
                        nowTrackName = popularTracksNames[popularTracksNames.IndexOf(nowTrackName) + 1];
                        nowTrackPath = popularTracksPaths[popularTracksPaths.IndexOf(nowTrackPath) + 1];
                    }
                    else
                    {
                        nowTrackName = popularTracksNames[0];
                        nowTrackPath = popularTracksPaths[0];
                    }
                    break;
                case "Favorite":
                    if (favoriteTracksNames.IndexOf(nowTrackName) + 1 < favoriteTracksNames.Count) 
                    {
                        nowTrackName = favoriteTracksNames[favoriteTracksNames.IndexOf(nowTrackName) + 1];
                        nowTrackPath = favoriteTracksPaths[favoriteTracksPaths.IndexOf(nowTrackPath) + 1];
                    }
                    else
                    {
                        nowTrackName = favoriteTracksNames[0];
                        nowTrackPath = favoriteTracksPaths[0];
                    }
                    break;
            }
            playTrack(nowTrackPath, nowTrackName);
        }

        private void prevBtn_Click(object sender, RoutedEventArgs e)
        {
            switch (nowPlaylistPlay)
            {
                case "Popular":
                    if (popularTracksNames.IndexOf(nowTrackName) - 1 > -1)
                    {
                        nowTrackName = popularTracksNames[popularTracksNames.IndexOf(nowTrackName) - 1];
                        nowTrackPath = popularTracksPaths[popularTracksPaths.IndexOf(nowTrackPath) - 1];
                    }
                    else
                    {
                        nowTrackName = popularTracksNames[popularTracksNames.Count-1];
                        nowTrackPath = popularTracksPaths[popularTracksPaths.Count - 1];
                    }
                    break;
                case "Favorite":
                    if (favoriteTracksNames.IndexOf(nowTrackName) - 1 > -1)
                    {
                        nowTrackName = favoriteTracksNames[favoriteTracksNames.IndexOf(nowTrackName) - 1];
                        nowTrackPath = favoriteTracksPaths[favoriteTracksPaths.IndexOf(nowTrackPath) - 1];
                    }
                    else
                    {
                        nowTrackName = favoriteTracksNames[favoriteTracksNames.Count-1];
                        nowTrackPath = favoriteTracksPaths[favoriteTracksPaths.Count-1];
                    }
                    break;
            }
            playTrack(nowTrackPath, nowTrackName);
        }
    }
}
