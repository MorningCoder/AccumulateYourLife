using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Phone.BackgroundTransfer;
using Microsoft.Phone.Tasks;

namespace AccumulateYourLife
{
    public partial class PageMp3s : PhoneApplicationPage
    {
        public PageMp3s()
        {
            InitializeComponent();
            this.Background = StaticClass.AppColL;
            this.root.Background = StaticClass.AppColL;
            this.ApplicationBar.BackgroundColor = StaticClass.AppColH.Color;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            List<Mp3File> list = Mp3File.GetFileList();
            this.fileList.ItemsSource = list;
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            Button playButton = e.OriginalSource as Button;
            if (playButton != null)
            {
                string fileName = StaticClass.Mp3DirectoryName + "/" + playButton.Tag.ToString();
                // 从独立存储区中读取文件
                try
                {
                    using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        // 如果文件不存在，跳出
                        if (!iso.FileExists(fileName))
                        {
                            MessageBox.Show("该文件不存在 " + fileName);
                            return;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("未能正确读取文件，播放失败");
                    return;
                }
                // 播放声音
                PlaySound(fileName);
            }
        }

        private void PlaySound(string path)
        {
            MediaPlayerLauncher mp = new MediaPlayerLauncher();
            mp.Location = MediaLocationType.Data;
            mp.Controls = MediaPlaybackControls.Pause | MediaPlaybackControls.Stop;
            mp.Orientation = MediaPlayerOrientation.Portrait;
            mp.Media = new Uri(path, UriKind.Relative);
            try
            {
                mp.Show();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }
    }


}