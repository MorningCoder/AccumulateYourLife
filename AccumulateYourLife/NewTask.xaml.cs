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
    public partial class NewTask : PhoneApplicationPage
    {
        RemTask task = null;
        List<Word> checkWordList = new List<Word>();
        List<Word> list = Word.GetLocalWordList(StaticClass.WordListFileName);
        /// <summary>
        /// 新建记忆任务
        /// </summary>
        public NewTask()
        {
            InitializeComponent();
            this.Background = StaticClass.AppColL;
            this.ApplicationBar.BackgroundColor = StaticClass.AppColH.Color;
            this.canvas.Background = StaticClass.AppColL;
            this.WordList.Background = StaticClass.AppColL;
            this.WordList.ItemsSource = list;
        }

        //新建任务
        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            //获取选中的单词并新建列表
            task = new RemTask(checkWordList);
            //保存至云端
            task.SaveToCloud();
            //保存至本地文件
            task.SaveAsFile();

            //保存为Record
            Record rec = new Record();
            rec.Title = task.CreateTime.ToString();
            rec.isRemTask = true;
            rec.Content = "您创建了一个记忆任务，点击查看此记忆任务，不要偷懒哦！";
            List<Record> list = Record.GetLocalRecordList(StaticClass.RecordListFileName);
            list.Add(rec);
            Record.AddToFile(list, StaticClass.RecordListFileName);

            MessageBox.Show("新建成功");
            this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            checkWordList.Add(new Word(cb.Content.ToString()));
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            foreach (Word word in checkWordList.ToArray())
            {
                if (word.Content.Equals(cb.Content))
                {
                    checkWordList.Remove(word);
                }
            }
        }
    }
}