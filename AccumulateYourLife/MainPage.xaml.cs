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
using AccumulateYourLife.Resources;
using System.Windows.Media;

namespace AccumulateYourLife
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
            
        }

        /// <summary>
        /// 当用户导航到主页时重新从本地存储加载单词本列表以及记事薄
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //设置页面主体颜色
            if (this.MainPivot.SelectedIndex == 0)
                this.ApplicationBar = (ApplicationBar)Resources["WordBar"];
            else if (this.MainPivot.SelectedIndex == 1)
                this.ApplicationBar = (ApplicationBar)Resources["RecordBar"];
            this.ApplicationBar.BackgroundColor = StaticClass.AppColH.Color;
            this.MainPivot.Background = StaticClass.AppColL;
            
            //try
            //{
                List<Record> recordList = Record.GetLocalRecordList(StaticClass.RecordListFileName);
                List<Word> wordList = Word.GetLocalWordList(StaticClass.WordListFileName);
                List<RemTask> taskList = RemTask.GetList();

                wordList.Sort((w1, w2) => w1.Content[0].CompareTo(w2.Content[0]));
                if (wordList == null)
                {
                    MessageBox.Show("word null");
                    return;
                }
                if (recordList == null) 
                {
                    MessageBox.Show("record null");
                    return;
                }                
                this.WordList.ItemsSource = wordList;
                this.RecordList.ItemsSource = recordList;
            //}
            //catch (Exception ee)
            //{
            //    MessageBox.Show("异常！" + ee.Message);
            //}
            
        }

        //添加
        private void ApplicationBarIconButtonAdd_Click(object sender, EventArgs e)
        {
            switch (this.MainPivot.SelectedIndex)
            {
                case 0:
                    this.NavigationService.Navigate(new Uri("/NewWord.xaml", UriKind.Relative));
                    break;
                case 1:
                    {
                        var s=(ApplicationBarIconButton)sender;
                        if (s.Text == "添加记事项")
                        {
                            this.NavigationService.Navigate(new Uri("/NewRecord.xaml", UriKind.Relative));   
                        }
                        else
                        {
                            this.NavigationService.Navigate(new Uri("/NewTask.xaml", UriKind.Relative));
                        }                                         
                    } break;
                    
            }
        }
        
        //同步按钮
        private void ApplicationBarIconButtonSync_Click(object sender, EventArgs e)
        {
            if(!StaticClass.isLogin)
            {
                MessageBox.Show("请先登录");
                this.NavigationService.Navigate(new Uri("/PageLogin.xaml", UriKind.Relative));
            }
            this.NavigationService.Navigate(new Uri("/PageSync.xaml", UriKind.Relative));
        }

        //记忆按钮
        private void ApplicationBarIconButtonRemember_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/RemWord.xaml", UriKind.Relative));
        }

        //关于
        private void ApplicationBarMenuItemAbout_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        //帮助
        private void ApplicationBarMenuItemHelp_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
        }


        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult LeaveResult = MessageBox.Show("确定退出应用？", "退出", MessageBoxButton.OKCancel);
            if (LeaveResult == MessageBoxResult.OK)
            {
                StaticClass.network.Accesstoken = null;
                IsolatedStorageSettings.ApplicationSettings.Save();
                Application.Current.Terminate();
                //退出的处理

            }
            else
                e.Cancel = true;
        }

        private void MainPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((Pivot)sender).SelectedIndex)
            {
                case 0:
                    this.ApplicationBar = (ApplicationBar)Resources["WordBar"];
                    this.ApplicationBar.BackgroundColor = StaticClass.AppColH.Color;
                    break;
                case 1:
                    this.ApplicationBar = (ApplicationBar)Resources["RecordBar"];
                    this.ApplicationBar.BackgroundColor = StaticClass.AppColH.Color;
                    break;
            }
        }

        //用户点击一个单词本LongListSelector项时发生
        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Word SelectWord = (Word)this.WordList.SelectedItem;
            this.NavigationService.Navigate(new Uri("/PageWord.xaml?word=" + SelectWord.Content,UriKind.Relative));
        }

        //倒序
        private void ApplicationBarMenuItemReverse_Click(object sender, EventArgs e)
        {
            List<Word> list = Word.GetLocalWordList(StaticClass.WordListFileName);
            list.Reverse();
            Word.AddToFile(list, StaticClass.WordListFileName);
            this.WordList.ItemsSource = list;

        }

        //乱序
        private void ApplicationBarMenuItemMess_Click(object sender, EventArgs e)
        {
            try
            {
                this.ProgressBar.IsIndeterminate = true;
                List<Word> list = Word.GetLocalWordList(StaticClass.WordListFileName);

                //实际元素数
                int count = list.Count;
                //产生随机数数组
                int[] arr = new int[count];
                //开始构造随机数
                Random ra = new Random();
                //判断随机数是否重复
                bool isExist = false;

                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = -1;
                }

                int n = 0;
                while (n < count)
                {
                    isExist = false;
                    int number = ra.Next(count);
                    for (int i = 0; i <= n; i++)
                    {
                        if (arr[i] == number)
                        {
                            isExist = true;
                            break;
                        }
                    }
                    if (!isExist)
                    {
                        arr[n] = number;
                        n++;
                    }
                }

                //新建list
                List<Word> newlist = new List<Word>();
                //重新赋值
                for (int i = 0; i < count; i++)
                {
                    int tmp = arr[i];
                    Word temp = list[tmp];
                    newlist.Add(temp);
                }
                //保存
                Word.AddToFile(newlist, StaticClass.WordListFileName);
                this.WordList.ItemsSource = newlist;
                this.ProgressBar.IsIndeterminate = false;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        //选择Record一项时发生
        private void RecordList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Record SelectRecord = (Record)this.RecordList.SelectedItem;
            if (SelectRecord.isRemTask)
            {
                this.NavigationService.Navigate(new Uri("/PageTask.xaml?filename=" + SelectRecord.SetTime.ToString("yyyymmddhhMMss") + ".dat", UriKind.Relative));
            }
            else
            {
                this.NavigationService.Navigate(new Uri("/PageRecord.xaml?record=" + SelectRecord.SetTime.ToFileTime().ToString(), UriKind.Relative));
            }
        }


        private void ApplicationBarMenuItemMp3_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/PageMp3s.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            
            
        }
    }
}