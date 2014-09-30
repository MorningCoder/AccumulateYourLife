using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;

namespace AccumulateYourLife
{
    public partial class NewRecord : PhoneApplicationPage
    {
        //当前页面的保存的record
        public Record record = new Record();
        
        public NewRecord()
        {
            InitializeComponent();
            this.Background = StaticClass.AppColH;
        }

        private void ApplicationBarIconButtonPhoto_Click(object sender, EventArgs e)
        {
            PhotoChooserTask task = new PhotoChooserTask();
            task.Completed += task_Completed;
            task.ShowCamera = true;
            task.Show();
        }

        private void task_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto);

                record.SavePicture(e.ChosenPhoto, this.record.SetTime.ToFileTime().ToString()+".jpg");
                record.ImageName = this.record.SetTime.ToFileTime().ToString()+".jpg";
                this.image.Source = bmp;
                
                StaticClass.ImageArr.Add(record.ImageName);
                StaticClass.SavePictureList();
            }
            //如果没有选择照片
            else
            {
                MessageBox.Show("未选择照片！");
                return;
            }
        }

        private void ApplicationBarIconButtonAlarm_Click(object sender, EventArgs e)
        {

        }

        private void ApplicationBarMenuItemHelp_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButtonSave_Click(object sender, EventArgs e)
        {
            //如果记事标题为空则自动换成日期显示
            if (title.Text == "")
            {
                title.Text = this.record.SetTime.ToLongDateString();
                record.Title = title.Text;
            }
            else
            {
                record.Title = title.Text;
                record.Content = content.Text;
            }
                

            try
            {
                List<Record> list = Record.GetLocalRecordList(StaticClass.RecordListFileName);
                list.Add(record);
                Record.AddToFile(list, StaticClass.RecordListFileName);
                MessageBox.Show("保存成功");
                this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            catch (Exception ee)
            {
                MessageBox.Show("异常" + ee.Message);
            }
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.RootCanvas.Background = StaticClass.AppColL;
            this.ApplicationBar.BackgroundColor = StaticClass.AppColH.Color;
        }
    }
}