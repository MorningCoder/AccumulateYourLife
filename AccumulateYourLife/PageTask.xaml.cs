using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace AccumulateYourLife
{
    public partial class PageTask : PhoneApplicationPage
    {
        RemTask task = null;
        public PageTask()
        {
            InitializeComponent();

            this.Background = StaticClass.AppColL;
            this.canvas.Background = StaticClass.AppColL;
            this.WordList.Background = StaticClass.AppColL;
            this.TaskList.Background = StaticClass.AppColL;
            this.ApplicationBar.BackgroundColor = StaticClass.AppColH.Color;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string file = this.NavigationContext.QueryString["filename"];
            if(string.IsNullOrEmpty(file))
            {
                MessageBox.Show("该任务不存在或已被删除");
                this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else
            {
                task = RemTask.GetInstance(file);
                this.WordList.ItemsSource = task.WordList;
                this.TaskList.ItemsSource = task.RemindTimeList;
            }
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }
    }
}