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
using System.Windows.Input;

namespace AccumulateYourLife
{
    /// <summary>
    /// 单词页类
    /// 显示一个单词的具体内容
    /// </summary>
    public partial class PageWord : PhoneApplicationPage
    {
        List<Word> list = null;
        private Word word = null;

        //手势开始时X坐标
        double startX;
        //double startY;

        public PageWord()
        {
            InitializeComponent();
            this.Background = StaticClass.AppColH;
        }

        private void ApplicationBarIconButtonAdd_Click(object sender, EventArgs e)
        {
            list = Word.GetLocalWordList(StaticClass.WordListFileName);
            if (word == null)
            {
                MessageBox.Show("单词为空！");
                return;
            }
            if (list == null)
            {
                MessageBox.Show("单词列表为空");
                return;
            }
            //添加到本地单词列表
            try
            {
                //这个检查单词是否存在的算法不太好
                if (list.Exists(w => w.Content.Equals(word.Content)))
                {
                    MessageBox.Show("单词已存在单词本中");
                    return;
                }
                word.AddToListFile(StaticClass.WordListFileName, list);
            }
            catch (IsolatedStorageException ee)
            {
                MessageBox.Show("文件读写异常！添加失败！" + ee.Message);
                return;
            }
            catch (InvalidDataException ee)
            {
                MessageBox.Show("对象序列化异常！添加失败！" + ee.Message);
                return;
            }
            MessageBox.Show("单词添加成功!");
            this.ApplicationBar = (ApplicationBar)Resources["CutBar"];
            this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButtonSpeak_Click(object sender, EventArgs e)
        {
            if (word == null)
            {
                MessageBox.Show("单词为空！");
                return;
            }

            word.Speak();
        }

        private void ApplicationBarMenuItemHelp_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
        }

        //导航到此页面时已确定单词已存在于本地单词词库
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.LayoutRoot.Background = StaticClass.AppColL;
            list = Word.GetLocalWordList(StaticClass.WordListFileName);
            
            IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
            string content = this.NavigationContext.QueryString["word"];

            //查找本地单词本是否添加，相应改变应用程序栏图标
            word = Word.GetLocalWordInstance(content);
            //如果本地单词词库内不存在
            if (word == null)
            {
                //词库内不存在，属异常情况
                MessageBox.Show("单词为空！");
                return;
            }
            //如果单词本内不存在或者不存在单词本文件
            else if (list == null||!list.Exists(w => w.Content.Equals(word.Content)))
            {
                this.textBlockWord.Text = word.Content;
                this.textBlockSymbol.Text = word.Symbol;
                this.textBlockMeaning.Text = word.Meaning;
                this.ApplicationBar = (ApplicationBar)Resources["AddBar"];
                this.ApplicationBar.BackgroundColor = StaticClass.AppColH.Color;
            }
            //如果存在
            else
            {
                this.textBlockWord.Text = word.Content;
                this.textBlockSymbol.Text = word.Symbol;
                this.textBlockMeaning.Text = word.Meaning;
                this.ApplicationBar = (ApplicationBar)Resources["CutBar"];
                this.ApplicationBar.BackgroundColor = StaticClass.AppColH.Color;
            }

        }

        //删除单词
        private void ApplicationBarIconButtonDelete_Click(object sender, EventArgs e)
        {
            list = Word.GetLocalWordList(StaticClass.WordListFileName);
            //单词本文件不存在 或者单词不存在
            if (list == null || !list.Exists(w => w.Content.Equals(word.Content)))
            {
                MessageBox.Show("单词未添加到单词本或单词本不存在");
                this.ApplicationBar = (ApplicationBar)Resources["AddBar"];
                return;
            }

            list.RemoveAll(w => w.Content.Equals(word.Content));
            Word.AddToFile(list, StaticClass.WordListFileName);
            
            MessageBox.Show("单词已从单词本中移除");
            this.ApplicationBar = (ApplicationBar)Resources["AddBar"];
            this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        //手势开始
        private void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            startX = e.ManipulationOrigin.X;
            //startY = e.ManipulationOrigin.Y;
        }

        //手势结束
        private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            //如果是连续运动
            if (e.IsInertial)
            {
                this.OnFlick(sender, e);
            }
        }

        //滑动
        private void OnFlick(object sender, ManipulationCompletedEventArgs e)
        {
            double x = e.FinalVelocities.LinearVelocity.X;
            list = Word.GetLocalWordList(StaticClass.WordListFileName);
            int index = Word.IndexOfWord(word.Content);
            
            //上一个单词
            if (x > startX)
            {
                if (index == 0)
                {
                    return;
                }
                Word w = list[index - 1];
                this.NavigationService.Navigate(new Uri("/PageWord.xaml?word=" + w.Content, UriKind.Relative));
                
            }
            //下一个单词
            else if (x < startX)
            {
                if (index == list.Count - 1)
                {
                    return;
                }
                Word w = list[index +1];
                this.NavigationService.Navigate(new Uri("/PageWord.xaml?word=" + w.Content, UriKind.Relative));
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

            base.OnBackKeyPress(e);
        }
    }
}