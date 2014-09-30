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
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Phone.BackgroundTransfer;
using Microsoft.Phone.Tasks;
using System.Threading;

namespace AccumulateYourLife
{
    public partial class RemWord : PhoneApplicationPage
    {
        private BackgroundTransferRequest _request;
        private string downloadPath;
        List<Word> list = Word.GetLocalWordList(StaticClass.WordListFileName);
        List<Word> checkWordList = new List<Word>();
        private bool isHaveTask = false;
        private string path;
        public RemWord()
        {
            InitializeComponent();
            this.Background = StaticClass.AppColL;
            this.ApplicationBar.BackgroundColor = StaticClass.AppColH.Color;
            this.canvas.Background = StaticClass.AppColL;
            this.WordList.Background = StaticClass.AppColL;
            this.WordList.ItemsSource = list;
        }

        void AddStream(string downloadPath, bool isTheLast)
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (FileStream AddStream = iso.OpenFile(path, FileMode.Append, FileAccess.Write)) //合成的音频文件
                {
                    using (BinaryWriter AddWriter = new BinaryWriter(AddStream))
                    {
                        using (FileStream TempStream = iso.OpenFile(downloadPath, FileMode.Open))
                        {
                            using (BinaryReader TempReader = new BinaryReader(TempStream))
                            {
                                AddWriter.Write(TempReader.ReadBytes((int)TempStream.Length));
                            }
                        }
                        if (!isTheLast)
                        {
                            using (FileStream TempStreamNull = new FileStream("mp3/空.mp3", FileMode.Open))
                            {
                                using (BinaryReader TempReader = new BinaryReader(TempStreamNull))
                                {
                                    AddWriter.Write(TempReader.ReadBytes((int)TempStreamNull.Length));
                                }
                            }
                        }
                    }
                }
            }
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

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            this.progressBar.IsIndeterminate = true;
            path = StaticClass.Mp3DirectoryName + "/" + DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss") + ".mp3";
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!iso.DirectoryExists(StaticClass.Mp3DirectoryName))
                {
                    iso.CreateDirectory(StaticClass.Mp3DirectoryName);
                }
                foreach (Word word in checkWordList.ToArray())
                {
                    downloadPath = string.Format("shared/transfers/{0}.mp3", word.Content);
                    //http://translate.google.com/translate_tts?tl=en_cn&q=morning
                    //创建一个后台文件传输请求
                    _request = new BackgroundTransferRequest(
                    new Uri("http://translate.google.com/translate_tts?tl=en_cn&q=" + word.Content))
                    {
                        Method = "GET",// 设置传输的方法为GET请求
                        DownloadLocation = new Uri(downloadPath, UriKind.Relative),
                        Tag = "shared/transfers/" + word.Content + ".mp3",   //添加请求的Tag属性，Tag属性不能超过4000个字符
                        TransferPreferences = TransferPreferences.AllowCellularAndBattery
                    };
                    // 使用BackgroundTransferService添加文件传输请求
                    try
                    {
                        if (!iso.FileExists(downloadPath))
                        {
                            isHaveTask = true;
                            BackgroundTransferService.Add(_request);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("无法添加请求:" + ex.Message);
                    }
                    if (checkWordList.IndexOf(word) == checkWordList.Count - 1)
                    {
                        if (isHaveTask)
                        {
                            for (int i = 0; i < 100; i++)
                            {
                                Thread.Sleep(1000);
                                if (BackgroundTransferService.Requests.Last().TransferStatus == TransferStatus.Completed)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            foreach (Word word in checkWordList.ToArray())
            {
                bool isTheLast = false;
                if (checkWordList.IndexOf(word) == checkWordList.Count - 1)
                {
                    isTheLast = true;
                }
                AddStream(string.Format("shared/transfers/{0}.mp3", word.Content), isTheLast);
            }
            this.progressBar.IsIndeterminate = false;
            MessageBox.Show("生成成功");
            this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

        }



    }



}





