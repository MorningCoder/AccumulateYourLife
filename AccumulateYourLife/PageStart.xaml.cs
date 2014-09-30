using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Notification;
using System.Threading;
using System.Diagnostics;

namespace AccumulateYourLife
{
    public partial class PageStart : PhoneApplicationPage
    {
        HttpNotificationChannel httpchanel;
        string channelname = "TileNotificationService";

        public PageStart()
        {
            InitializeComponent();

            try
            {
                httpchanel = HttpNotificationChannel.Find(channelname);
                if (httpchanel == null)
                {
                    //新建对象
                    httpchanel = new HttpNotificationChannel(channelname);
                    //注册事件
                    httpchanel.ChannelUriUpdated += httpchanel_ChannelUriUpdated;
                    httpchanel.ErrorOccurred += httpchanel_ErrorOccurred;
                    //打开通道
                    httpchanel.Open();
                    //绑定到磁贴通知
                    httpchanel.BindToShellTile();

                    StaticClass.MyUri = httpchanel.ChannelUri.ToString();
                }
                else
                {
                    httpchanel.ChannelUriUpdated += httpchanel_ChannelUriUpdated;
                    httpchanel.ErrorOccurred += httpchanel_ErrorOccurred;

                    StaticClass.MyUri = httpchanel.ChannelUri.ToString();
                }
            }
            catch(Exception ee)
            {
                MessageBox.Show("URI获取失败！错误信息：" + ee.Message);
            }
        }

        void httpchanel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {   
            Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(e.Message);
                });
        }

        void httpchanel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            StaticClass.MyUri = e.ChannelUri.ToString();
            StaticClass.network.UpLoadText(string.Format("<Uri>{0}</Uri>", StaticClass.MyUri),"uri", Callback);
        }

        public void Callback(string result)
        {
            Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show(result);
            });
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            //判断网络连接情况
            if (!DeviceNetworkInformation.IsNetworkAvailable)
                StaticClass.isNet = false;
            if (DeviceNetworkInformation.IsCellularDataEnabled || DeviceNetworkInformation.IsWiFiEnabled)
                StaticClass.isNet = true;
            if(!StaticClass.isNet)
            {
                MessageBox.Show("网络未连接！请检查网络设置");
                StaticClass.isNet = false;
            }
            StaticClass.GetPictureList();
            StyleColor c = StyleColor.GetColor(StaticClass.ColorFileName);
            StaticClass.SetTwoBrushes(c.colorH, c.colorL, c.colorI);
            Thread.Sleep(200);
            this.NavigationService.Navigate(new Uri("/PageLogin.xaml", UriKind.Relative));
        }
    }
}