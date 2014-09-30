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
using System.Runtime.Serialization;
using LitJson;

namespace AccumulateYourLife
{
    public partial class PageSync : PhoneApplicationPage
    {
        private int i = 0;
        //是否全部上传
        private bool isAll = true;
        //是否只传单词本文件
        private bool isWordOnly = false;
        //是否只传记事薄
        private bool isRecordOnly = false;
        //保存图片文件名数组
        [DataMember]
        public List<string> ImageArr = new List<string>();

        public PageSync()
        {
            InitializeComponent();
            this.canvas.Background = StaticClass.AppColL;
            this.All.SwitchForeground = StaticClass.AppColH;
            //this.All.BorderBrush = StaticClass.AppColH;
            this.upload.Background = StaticClass.AppColH;
            this.download.Background = StaticClass.AppColH;
            StaticClass.GetPictureList();

        }


        private void All_Checked(object sender, RoutedEventArgs e)
        {
            isAll = true;
            WordOnly.IsEnabled = false;
            RecordOnly.IsEnabled = false;
            isWordOnly = false;
            isRecordOnly = false;
            

        }
        //上传至云端
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.progressBar.IsIndeterminate = true;
            if (isAll)
            {
                MessageBox.Show("all");
                try
                {
                    using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        //如果文件存在
                        if (iso.FileExists(StaticClass.WordListFileName))
                        {
                            FileStream stream = (FileStream)iso.OpenFile(StaticClass.WordListFileName, FileMode.Open, FileAccess.Read);
                            StaticClass.network.UpLoadFile(StaticClass.WordListFileName, stream, UploadAllCallBack);
                        }
                        else
                        {
                            MessageBox.Show("单词本文件不存在，上传失败");
                            return;
                        }
                    }
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
                try
                {
                    using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        //如果文件存在
                        if (iso.FileExists(StaticClass.RecordListFileName))
                        {

                            FileStream stream = (FileStream)iso.OpenFile(StaticClass.RecordListFileName, FileMode.Open, FileAccess.Read);
                            StaticClass.network.UpLoadFile(StaticClass.RecordListFileName, stream, UploadAllCallBack);
                            
                            if (ImageArr.Count == 0)
                                return;
                            
                            StaticClass.network.UpLoadFile(ImageArr[i], (FileStream)Record.GetPicture(ImageArr[i++]), UploadAllCallBack);
                        }
                        else
                        {
                            MessageBox.Show("记事薄文件不存在，上传失败");
                            return;
                        }
                    }
                }
                catch (IsolatedStorageException e1)
                {
                    MessageBox.Show(e1.Message);
                    return;
                }
                catch (Exception e2)
                {
                    MessageBox.Show(e2.Message);
                    return;
                }


            }
            else if (isWordOnly)
            {
                this.progressBar.IsIndeterminate = true;
                try
                {
                    using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        //如果文件存在
                        if (iso.FileExists(StaticClass.WordListFileName))
                        {
                            FileStream stream = (FileStream)iso.OpenFile(StaticClass.WordListFileName, FileMode.Open, FileAccess.Read);
                            StaticClass.network.UpLoadFile(StaticClass.WordListFileName, stream, UploadFileCallBack);
                        }
                        else
                        {
                            MessageBox.Show("单词本文件不存在，上传失败");
                            return;
                        }
                    }
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
            }
            else if (isRecordOnly)
            {
                //文件读取操作
                try
                {
                    using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        //如果文件存在
                        if (iso.FileExists(StaticClass.RecordListFileName))
                        {
                            FileStream stream = (FileStream)iso.OpenFile(StaticClass.RecordListFileName, FileMode.Open, FileAccess.Read);
                            StaticClass.network.UpLoadFile(StaticClass.RecordListFileName, stream, UploadFileCallBack);
                            if (ImageArr.Count == 0)
                                return;

                            StaticClass.network.UpLoadFile(ImageArr[i], (FileStream)Record.GetPicture(ImageArr[i++]), UploadPictureCallBack);
                        }
                        else
                        {
                            MessageBox.Show("记事薄文件不存在，上传失败");
                            return;
                        }
                    }
                }
                catch (IsolatedStorageException e1)
                {
                    MessageBox.Show(e1.Message);
                    return;
                }
                catch (Exception e2)
                {
                    MessageBox.Show(e2.Message);
                    return;
                }

            }
            else
            {
                MessageBox.Show("出现异常，请重新上传");
                return;
            }
        }

        //上传全部回调
        private void UploadAllCallBack(string result)
        {
            try
            {
                Dispatcher.BeginInvoke(() =>
                {
                    this.progressBar.IsIndeterminate = false;
                    JsonData jd = JsonMapper.ToObject(result);
                    int returnResult = Convert.ToInt32(jd["result"].ToString().Trim(new char[] { '[', ']' }));
                    string returnMsg = (string)jd["message"].ToString().Trim(new char[] { '[', ']' });

                    if (returnResult == 0)
                        MessageBox.Show("上传成功");
                    else
                        MessageBox.Show(returnMsg + "请重新上传");
                });
            }
            catch(Exception ee)
            { }
        }

        //上传图片回调函数
        private void UploadPictureCallBack(string result)
        {
            this.progressBar.IsIndeterminate = false;
            StaticClass.network.UpLoadFile(ImageArr[i], (FileStream)Record.GetPicture(ImageArr[i++]), UploadPictureCallBack);
            int x = ImageArr.Count - 1;
            if (ImageArr[i].Equals(ImageArr[x]))
                return;
        }

        //上传文件回调函数
        private void UploadFileCallBack(string result)
        {
            try
            {
                Dispatcher.BeginInvoke(() =>
                {
                    this.progressBar.IsIndeterminate = false;
                    JsonData jd = JsonMapper.ToObject(result);
                    int returnResult = Convert.ToInt32(jd["result"].ToString().Trim(new char[] { '[', ']' }));
                    string returnMsg = (string)jd["message"].ToString().Trim(new char[] { '[', ']' });

                    if (returnResult == 0)
                        MessageBox.Show("上传成功");
                    else
                        MessageBox.Show(returnMsg + "请重新上传");
                });
            }
            catch(Exception ee)
            { }
        }

        private void WordOnly_Checked(object sender, RoutedEventArgs e)
        {
            isWordOnly = true;
            isRecordOnly = false;
            isAll = false;
        }

        private void RecordOnly_Checked(object sender, RoutedEventArgs e)
        {
            isRecordOnly = true;
            isWordOnly = false;
            isAll = false;
        }

        private void All_Unchecked(object sender, RoutedEventArgs e)
        {
            isAll = false;
            if (this.WordOnly.IsEnabled == false || this.RecordOnly.IsEnabled == false)
            {
                WordOnly.IsEnabled = true;
                RecordOnly.IsEnabled = true;
            }
            this.WordOnly.IsChecked = true;
            isWordOnly = true;
        }

        //从云端下载
        private void download_Click(object sender, RoutedEventArgs e)
        {
            this.progressBar.IsIndeterminate = true;
            if (isAll)
            {
                StaticClass.network.DownLoadFile(StaticClass.WordListFileName, DownLoadWordCallBack);
                StaticClass.network.DownLoadFile(StaticClass.RecordListFileName, DownLoadRecordCallBack);
            }
            else if (isWordOnly)
            {
                StaticClass.network.DownLoadFile(StaticClass.WordListFileName, DownLoadWordCallBack);
            }
            else if (isRecordOnly)
            {
                StaticClass.network.DownLoadFile(StaticClass.RecordListFileName, DownLoadRecordCallBack);
            }
            else
            {
                MessageBox.Show("出现异常，请重新上传");
                return;
            }
        }

        //下载单词本文件回调函数
        private void DownLoadWordCallBack(Stream result)
        {
            byte[] buf = new byte[result.Length];
            result.Write(buf, 0, buf.Length);
            //文件写入操作
            try
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream fs = iso.OpenFile(StaticClass.WordListFileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        fs.Write(buf, 0, buf.Length);
                    }
                    
                }
            }
            catch (IsolatedStorageException e)
            {
                throw new IsolatedStorageException(e.Message);
            }
            catch (InvalidDataException e)
            {
                throw new InvalidDataException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            Dispatcher.BeginInvoke(() =>
            {
                this.progressBar.IsIndeterminate = false;
            });
        }

        //下载记事回调函数
        private void DownLoadRecordCallBack(Stream result)
        {
            byte[] buf = new byte[result.Length];
            result.Write(buf, 0, buf.Length);
            //文件写入操作
            try
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream fs = iso.OpenFile(StaticClass.RecordListFileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        fs.Write(buf, 0, buf.Length);
                    }

                }
            }
            catch (IsolatedStorageException e)
            {
                throw new IsolatedStorageException(e.Message);
            }
            catch (InvalidDataException e)
            {
                throw new InvalidDataException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            Dispatcher.BeginInvoke(() =>
            {
                this.progressBar.IsIndeterminate = false;
            });
        }

    }
}