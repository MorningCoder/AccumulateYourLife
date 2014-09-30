using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows;

namespace AccumulateYourLife
{
    [DataContract]
    /// <summary>
    /// 表示一个记忆任务
    /// </summary>
    public class RemTask
    {
        [DataMember]
        //public string Mp3Path;
        /// <summary>
        /// 本任务的创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        [DataMember]
        /// <summary>
        /// 本任务包含的单词列表
        /// </summary>
        public List<Word> WordList { get; set; }
        [DataMember]
        /// <summary>
        /// 用于提醒的日期数组
        /// </summary>
        public List<DateTime> RemindTimeList { get; set; }
        [DataMember]
        //保存本地的文件名
        public string filename;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="list"></param>
        public RemTask(List<Word> list)
        {
            CreateTime = DateTime.Now;
            WordList = list;
            RemindTimeList = new List<DateTime>();
            filename = CreateTime.ToString("yyyymmddhhMMss") + ".dat";
            RemindTimeList.Add(CreateTime.AddDays(1));
            RemindTimeList.Add(CreateTime.AddDays(2));
            RemindTimeList.Add(CreateTime.AddDays(5));
            RemindTimeList.Add(CreateTime.AddDays(8));
            RemindTimeList.Add(CreateTime.AddDays(14));
        }

        /// <summary>
        /// 获得list
        /// </summary>
        /// <returns></returns>
        public static List<RemTask> GetList()
        {
            List<RemTask> list = new List<RemTask>();
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!iso.DirectoryExists("RememberTasks"))
                    return list;
                string[] namearr = iso.GetFileNames("RememberTasks/*.*");
                if (namearr.Length == 0)
                    return list;
                foreach (string s in namearr)
                {
                    list.Add(RemTask.GetInstance(s));
                }
                return list;
            }
        }

        /// <summary>
        /// 文件反序列化为对象
        /// </summary>
        /// <param name="filename"></param>
        public static RemTask GetInstance(string filename)
        {
            try
            {
                using(IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    //如果文件存在
                    if (iso.FileExists("RememberTasks/" + filename))
                    {
                        using (IsolatedStorageFileStream fs = iso.OpenFile("RememberTasks/" + filename, FileMode.Open, FileAccess.Read))
                        {
                            var serializer = new DataContractJsonSerializer(typeof(RemTask));

                            return (RemTask)serializer.ReadObject(fs);
                        }
                    }
                    else
                        return null;
                }
            }
            catch(Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }

        /// <summary>
        /// 对象序列化为文件
        /// </summary>
        public void SaveAsFile()
        {
            //文件写入操作
            try
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if(!iso.DirectoryExists("RememberTasks"))
                    {
                        iso.CreateDirectory("RememberTasks");
                    }
                    using (IsolatedStorageFileStream fs = iso.OpenFile("RememberTasks/" + filename, FileMode.Create))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(RemTask));
                        serializer.WriteObject(fs, this);
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
        }

        /// <summary>
        /// 保存至云端
        /// </summary>
        public void SaveToCloud()
        {
            string str = string.Format("<CreateTime>{0}</CreateTime>" + "<RemindTime>{1}</RemindTime>" +
                "<RemindTime>{2}</RemindTime>" + "<RemindTime>{3}</RemindTime>" + "<RemindTime>{4}</RemindTime>"
                + "<RemindTime>{5}</RemindTime>",
                this.CreateTime.ToString("yyyymmdd"), this.RemindTimeList[0].ToString("yyyymmdd"),
                this.RemindTimeList[1].ToString("yyyymmdd"), this.RemindTimeList[2].ToString("yyyymmdd"),
                this.RemindTimeList[3].ToString("yyyymmdd"), this.RemindTimeList[4].ToString("yyyymmdd"));
           
            StaticClass.network.UpLoadText(str, "create", UplodeTextCallBack);
        }

        private void UplodeTextCallBack(string result)
        {
            MessageBox.Show(result);
        }
    }
}
