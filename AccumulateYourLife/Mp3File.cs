using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;

namespace AccumulateYourLife
{
    /// <summary>
    /// 存储音频文件相关信息
    /// </summary>
    public class Mp3File
    {
        //文件名
        public string FileName { get; set; }
        //生成时间
        public string SaveTime { get; set; }

        /// <summary>
        /// 返回音频文件列表
        /// </summary>
        /// <returns></returns>
        public static List<Mp3File> GetFileList()
        {
            List<Mp3File> mp3List = new List<Mp3File>();
            try
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!iso.DirectoryExists(StaticClass.Mp3DirectoryName))
                    {
                        iso.CreateDirectory(StaticClass.Mp3DirectoryName);
                    }
                    string[] filelist = iso.GetFileNames(StaticClass.Mp3DirectoryName + "/*.*");

                    foreach (string f in filelist)
                    {
                        Mp3File rec = new Mp3File()
                        {
                            FileName = f,
                            SaveTime = iso.GetCreationTime(f).ToString("yy-MM-dd HH:mm:ss")
                        };
                        mp3List.Add(rec);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return mp3List;
        }
    }
}
