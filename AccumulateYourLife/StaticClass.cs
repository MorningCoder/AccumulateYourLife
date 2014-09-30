using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows;

namespace AccumulateYourLife
{
    [DataContract]
    /// <summary>
    /// 用于存储静态变量
    /// </summary>
    public static class StaticClass
    {
        [DataMember]
        //图片文件名数组
        public static List<string> ImageArr { get; set; }
        //保存音频文件的文件夹名称
        public static string Mp3DirectoryName = "SavedMp3s";
        //全局网络链接对象
        public static NetWork network = new NetWork();
        //保存主题颜色设置的文件
        public const string ColorFileName = "ColorSetting.dat";
        //保存至本地的单词本文件名
        public const string WordListFileName = "WordList.dat";
        //保存至本地的记事薄文件名
        public const string RecordListFileName = "RecordList.dat";
        //保存本机URI
        public static string MyUri = null;
        //保存网络是否可用布尔值
        public static bool isNet = false;
        //应用主题深色颜色
        public static SolidColorBrush AppColH = new SolidColorBrush(Color.FromArgb(255, 6,146,199));
        //应用主题浅色颜色
        public static SolidColorBrush AppColL = new SolidColorBrush(Color.FromArgb(255,135,206,235));
        //颜色记录索引
        public static int ColorIndex = 0;
        //是否登陆
        public static bool isLogin = false;
        public static void SetTwoBrushes(Color h,Color l,int index)
        {
            AppColH = new SolidColorBrush(h);
            AppColL = new SolidColorBrush(l);
            ColorIndex = index;
            ImageArr = new List<string>();
        }

        public static void SavePictureList()
        {
            //文件写入操作
            try
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream fs = iso.OpenFile("picturelist.dat", FileMode.Create, FileAccess.ReadWrite))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(List<string>));
                        serializer.WriteObject(fs, StaticClass.ImageArr);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public static void GetPictureList()
        {
            //文件读取操作
            try
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    //如果文件存在
                    if (iso.FileExists("picturelist.dat"))
                    {
                        using (IsolatedStorageFileStream fs = iso.OpenFile("picturelist.dat", FileMode.Open, FileAccess.Read))
                        {
                            var serializer = new DataContractJsonSerializer(typeof(List<string>));

                            StaticClass.ImageArr = (List<string>)serializer.ReadObject(fs);
                        }
                    }
                    //文件不存在就新建
                    else if (!iso.FileExists("picturelist.dat"))
                    {
                        using (IsolatedStorageFileStream fs = iso.OpenFile("picturelist.dat", FileMode.CreateNew, FileAccess.ReadWrite))
                        {
                            var serializer = new DataContractJsonSerializer(typeof(List<string>));
                            serializer.WriteObject(fs, StaticClass.ImageArr);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "static");
            }
        }
    }
}