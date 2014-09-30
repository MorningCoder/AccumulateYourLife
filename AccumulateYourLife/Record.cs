using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO.IsolatedStorage;
using System.Windows;

namespace AccumulateYourLife
{
    /// <summary>
    /// 表示记事薄一个记事项
    /// </summary>
    [DataContract]
    public class Record
    {
        /// <summary>
        /// 记事标题
        /// </summary>
        [DataMember]
        public string Title { get; set; }
        /// <summary>
        /// 记事内容
        /// </summary>
        [DataMember]
        public string Content { get; set; }
        /// <summary>
        /// 图片的数据文件路径
        /// </summary>
        [DataMember]
        public string ImageName { get; set; }
        [DataMember]
        /// <summary>
        /// 显示颜色字符串
        /// </summary>
        public string ShowColor { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime SetTime { get; set; }
        /// <summary>
        /// 是否为记忆任务
        /// </summary>
        [DataMember]
        public bool isRemTask { get; set; }

        //构造函数
        public Record()
        {
            Title = string.Empty;
            Content = string.Empty;
            ImageName = string.Empty;
            ShowColor = StaticClass.AppColH.Color.ToString();
            SetTime = DateTime.Now;
            isRemTask = false;
        }

        /// <summary>
        /// 根据备忘创建时间匹配本地文件并返回匹配的Record对象
        /// </summary>
        /// <param name="time">创建时间</param>
        /// <returns>匹配的Record对象</returns>
        public static Record DateMatch(string time,string filename)
        {
            List<Record> list = new List<Record>();
            //文件读取操作
            try
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    //如果文件存在
                    if (iso.FileExists(filename))
                    {
                        using (IsolatedStorageFileStream fs = iso.OpenFile(filename, FileMode.Open, FileAccess.Read))
                        {
                            var serializer = new DataContractJsonSerializer(typeof(List<Record>));
                            list = (List<Record>)serializer.ReadObject(fs);
                        }
                    }
                    //文件不存在属于异常情况
                    else
                    {
                        MessageBox.Show("发生异常！文件不存在！");
                        return null;
                    }

                    return list.Find(w => (w.SetTime.ToFileTime().ToString()).Equals(time));
                }
            }
            catch(IsolatedStorageException e1)
            {
                MessageBox.Show("文件读取异常！" + e1.Message);
                return null;
            }
            catch(ArgumentNullException e2)
            {
                MessageBox.Show("参数为空异常！" + e2.Message);
                return null;
            }
        }
        /// <summary>
        /// 添加记事列表到文件
        /// </summary>
        /// <param name="list"></param>
        /// <param name="filename"></param>
        public static void AddToFile(List<Record> list, string filename)
        {
            //文件写入操作
            try
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream fs = iso.OpenFile(filename, FileMode.Create))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(List<Record>));
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
        /// 读取本地记事薄文件并返回列表
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static List<Record> GetLocalRecordList(string filename)
        {
            List<Record> list = new List<Record>();
            //文件读取操作
            try
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    //如果文件存在
                    if (iso.FileExists(filename))
                    {
                        using (IsolatedStorageFileStream fs = iso.OpenFile(filename, FileMode.Open, FileAccess.Read))
                        {
                            var serializer = new DataContractJsonSerializer(typeof(List<Record>));

                            list = (List<Record>)serializer.ReadObject(fs);
                        }
                    }
                    //文件不存在就新建
                    else if (!iso.FileExists(filename))
                    {
                        using (IsolatedStorageFileStream fs = iso.OpenFile(filename, FileMode.CreateNew, FileAccess.ReadWrite))
                        {
                            var serializer = new DataContractJsonSerializer(typeof(List<Record>));
                            serializer.WriteObject(fs, list);
                        }
                    }
                }
            }
            catch (IsolatedStorageException e)
            {
                MessageBox.Show("读写文件异常！" + e.Message);
                return null;
            }
            catch (InvalidDataException e)
            {
                MessageBox.Show("对象序列化异常！" + e.Message);
                return null;
            }
            
                return list;
        }

        /// <summary>
        /// 读取图片文件
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        public static Stream GetPicture(string filename)
        {
            //文件读取操作
            try
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    //如果文件存在
                    if (iso.FileExists(filename))
                        return iso.OpenFile(filename, FileMode.Open, FileAccess.Read);
                    else
                        return null;
                }
            }
            catch (IsolatedStorageException e)
            {
                MessageBox.Show("读写文件异常！" + e.Message);
                return null;
            }
            catch (InvalidDataException e)
            {
                MessageBox.Show("对象序列化异常！" + e.Message);
                return null;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        /// <summary>
        /// 保存图片到独立存储
        /// </summary>
        /// <param name="stream">图片流</param>
        /// <param name="filename">文件名</param>
        public void SavePicture(Stream stream, string filename)
        {
            try
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (iso.FileExists(filename))
                    {
                        MessageBox.Show("文件存在");
                        iso.DeleteFile(filename);
                    }

                    //创建新文件
                    IsolatedStorageFileStream fileStream = iso.CreateFile(filename);
                    //读取图片流到bitmap
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.SetSource(stream);
                    //新建wb对象
                    WriteableBitmap wb = new WriteableBitmap(bitmap);
                    //保存图片
                    Extensions.SaveJpeg(wb, fileStream, wb.PixelWidth, wb.PixelHeight, 0, 100);

                    fileStream.Close();
                }
            }
            catch(Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }
    }
}
