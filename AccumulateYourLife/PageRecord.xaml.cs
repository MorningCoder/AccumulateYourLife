using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;
using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace AccumulateYourLife
{
    public partial class PageRecord : PhoneApplicationPage
    {
        private List<Record> list = null;
        private Record record = null;

        public PageRecord()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.LayoutRoot.Background = StaticClass.AppColL;
            list = Record.GetLocalRecordList(StaticClass.RecordListFileName);
            string datetime = this.NavigationContext.QueryString["record"];

            //查找本地备忘列表
            record = Record.DateMatch(datetime, StaticClass.RecordListFileName);

            //如果不存在则属于异常情况
            if (record == null)
            {
                MessageBox.Show("该记事不存在或已被删除！");
                return;
            }

            //读取图片
            try
            {
                if (record.ImageName != string.Empty)
                {
                    BitmapImage bmp = new BitmapImage();
                    Stream s = Record.GetPicture(record.ImageName);
                    bmp.SetSource(s);
                    this.image.Source = bmp;
                }
                else 
                {
                    image.Height = 0;
                }
            }
            catch(Exception ee)
            {
              MessageBox.Show(ee.Message);
            }

            this.textBlockTitle.Text = record.Title;
            this.textBlockContent.Text = record.Content;
            this.textBlockSetTime.Text = record.SetTime.ToShortDateString() + " " + record.SetTime.ToShortTimeString();
        }
    }
}