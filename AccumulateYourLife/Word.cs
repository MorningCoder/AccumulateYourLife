using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Phone.Speech.Synthesis;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Media;

namespace AccumulateYourLife
{
    /// <summary>
    /// 单词类
    /// </summary>
    [DataContract]
    public class Word
    {
        //单词音标
        [DataMember]
        public string Symbol { get; set; }
        //单词本身内容
        [DataMember]
        public string Content { get; set; }
        //单词释义
        [DataMember]
        public string Meaning { get; set; }

        //构造函数
        public Word(string content)
        {
            this.Content = content;
            Symbol = null;
            //Property = WordProperty.n;
            Meaning = null;
        }

        /// <summary>
        /// 发音
        /// </summary>
        public async void Speak()
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();
            await synth.SpeakTextAsync(this.Content);
        }

        /// <summary>
        /// 添加单词列表到到单词本文件
        /// </summary>
        /// <param name="list">单词列表</param>
        /// <param name="filename">单词本文件名</param>
        public static void AddToFile(List<Word> list, string filename)
        {
            
            //文件写入操作
            try
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream fs = iso.OpenFile(filename, FileMode.Create))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(List<Word>));
                        serializer.WriteObject(fs, list);
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

        }

        /// <summary>
        /// 添加到本地单词本文件
        /// </summary>
        /// <param name="filename">单词本文件名</param>
        /// <param name="list">单词列表对象</param>
        public void AddToListFile(string filename,List<Word> list)
        {
            //单词先添加到list
            list.Add(this);

            //文件写入操作
            try
            {
                using(IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using(IsolatedStorageFileStream fs = iso.OpenFile(filename,FileMode.Create))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(List<Word>));
                        serializer.WriteObject(fs, list);
                    }
                }
            }
            catch(IsolatedStorageException e)
            {
                throw new IsolatedStorageException(e.Message);
            }
            catch(InvalidDataException e)
            {
                throw new InvalidDataException(e.Message);
            }
        }

        /// <summary>
        /// 添加到本地单词库
        /// </summary>
        public void AddToLibrary()
        {
            IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
            if (!setting.Contains(this.Content))
            {
                setting.Add(this.Content, this);
                try
                {
                    //保存
                    setting.Save();
                }
                catch
                {

                    //申请更大空间
                    using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        long orgSize = iso.Quota;
                        long sizeToAdd = 1 * 1024 * 1024;
                        iso.IncreaseQuotaTo(orgSize + sizeToAdd);
                    }
                }
            }
        }

        /// <summary>
        /// 根据单词内容查找本地单词库获取一个Word实例
        /// </summary>
        public static Word GetLocalWordInstance(string content)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
            if (!setting.Contains(content))
                return null;
            else
                return (Word)setting[content];
        }

        /// <summary>
        /// 异步获取本地词库单词列表
        /// </summary>
        /// <returns></returns>
        public static Task<List<Word>> GetLibraryListAsync()
        {
            return Task.Run<List<Word>>(() =>
                {
                    IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
                    List<Word> list = new List<Word>();
                    foreach (string content in setting.Keys)
                    {
                        list.Add((Word)setting[content]);
                    }
                    return list;
                });
        }

        /// <summary>
        /// 读取本地单词本文件并返回单词本单词列表
        /// </summary>
        public static List<Word> GetLocalWordList(string filename)
        {
            List<Word> list = new List<Word>();
            //文件读取操作
            try
            {
                using(IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    //如果文件存在
                    if (iso.FileExists(filename))
                    {
                        using (IsolatedStorageFileStream fs = iso.OpenFile(filename, FileMode.Open, FileAccess.Read))
                        {
                            var serializer = new DataContractJsonSerializer(typeof(List<Word>));

                            list = (List<Word>)serializer.ReadObject(fs);
                        }
                    }
                    //文件不存在就新建
                    else if(!iso.FileExists(filename))
                    {
                        using(IsolatedStorageFileStream fs = iso.OpenFile(filename,FileMode.CreateNew,FileAccess.ReadWrite))
                        {
                            var serializer = new DataContractJsonSerializer(typeof(List<Word>));
                            serializer.WriteObject(fs, list);
                        }
                    }
                }
            }
            catch(IsolatedStorageException e)
            {
                MessageBox.Show("读写文件异常！" + e.Message);
                return null;
            }
            catch(InvalidDataException e)
            {
                MessageBox.Show("对象序列化异常！" + e.Message);
                return null;
            }
            return list;
        }
 
        //重写ToString方法
        public override string ToString()
        {
            return string.Format("内容：{0}\n 音标：{1}\n 释义：{2}", this.Content, this.Symbol, this.Meaning);
        }

        /// <summary>
        /// 获取单词索引
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static int IndexOfWord(string content)
        {
            List<Word> list = Word.GetLocalWordList(StaticClass.WordListFileName);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Content == content)
                {
                    return i;
                }
            }

            return -1;
        }
    }

}
