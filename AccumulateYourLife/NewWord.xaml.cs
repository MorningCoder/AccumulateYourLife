using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.IO.IsolatedStorage;
using LitJson;


namespace AccumulateYourLife
{
    public partial class NewWord : PhoneApplicationPage
    {


        public NewWord()
        {
            InitializeComponent();
        }

        private void ApplicationBarMenuItemHelp_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButtonQuery_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.textBoxQueryWord.Text))
            {
                MessageBox.Show("请输入要查询的单词");
                return;
            }
            this.textBoxQueryWord.IsEnabled = false;
            IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
            string WordContent = this.textBoxQueryWord.Text;
            //如果本地词库存在此单词
            if (setting.Contains(WordContent))
            {
                this.NavigationService.Navigate(new Uri("/PageWord.xaml?word=" + WordContent, UriKind.Relative));
            }
            //不存在则网络查找并保存至本地，这两个操作最后都会导航到单词页
            else if (!setting.Contains(WordContent))
            {
                WebClient wc = new WebClient();
                //字符串下载完成后执行事件
                wc.DownloadStringCompleted += wc_DownloadStringCompleted;
                //异步从uri中下载字符串
                wc.DownloadStringAsync(new Uri("http://fanyi.youdao.com/openapi.do?keyfrom=sxt102400&key=1695079984&type=data&doctype=json&version=1.1&q=" + textBoxQueryWord.Text));

                this.progressBar.IsIndeterminate = true;

            }
        }

        //从网络上下载字符串（单词信息），下载完成后保存
        private void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {

            //解析json
            Word newWord = new Word(textBoxQueryWord.Text);
            string dlStr = e.Result;
            JsonData jd = JsonMapper.ToObject(dlStr);
            var basicVar = jd["basic"];
            if (basicVar == null)
            {
                MessageBox.Show("该单词不存在");
                this.textBoxQueryWord.IsEnabled = true;
                this.progressBar.IsIndeterminate = false;

                return;
            }
            if (basicVar.Count <= 0)
            {
                MessageBox.Show("找不到这个单词，请检查拼写！");
                this.textBoxQueryWord.IsEnabled = true;
                this.progressBar.IsIndeterminate = false;
                return;
            }
            else
            {
                if (basicVar.Count >= 2)//单词
                {
                    newWord.Symbol = '[' + basicVar["phonetic"].ToString().Trim(new char[] { '{', '}' }) + ']';
                }
                var explainsVar = basicVar["explains"];
                foreach (var explain in explainsVar)
                {
                    newWord.Meaning += explain.ToString().Trim(new char[] { '{', '}' }) + '\n';
                }
            }



            //添加到本地词库
            try
            {
                IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
                setting.Add(newWord.Content, newWord);
                setting.Save();
            }
            catch
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    long orgSize = iso.Quota;
                    long sizeToAdd = 1024 * 1024;
                    iso.IncreaseQuotaTo(orgSize + sizeToAdd);
                }
            }

            this.progressBar.IsIndeterminate = false;
            //转至单词页面
            this.NavigationService.Navigate(new Uri("/PageWord.xaml?word=" + newWord.Content, UriKind.Relative));
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.LayoutRoot.Background = StaticClass.AppColL;
            this.ApplicationBar.BackgroundColor = StaticClass.AppColH.Color;
            this.textBoxQueryWord.Text = "";
            this.WordLib.ItemsSource = await Word.GetLibraryListAsync();
        }

        private void WordLib_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Word w = (Word)WordLib.SelectedItem;
            this.NavigationService.Navigate(new Uri("/PageWord.xaml?word=" + w.Content, UriKind.Relative));
        }
    }
}