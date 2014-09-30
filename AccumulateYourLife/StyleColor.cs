using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.IO;
using System.IO.IsolatedStorage;
using AccumulateYourLife;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows;

namespace AccumulateYourLife
{
    /// <summary>
    /// 用于存储主题风格颜色的类
    /// </summary>
    [DataContract]
    public class StyleColor
    {
        [DataMember]
        public Color colorH { get; set; }

        [DataMember]
        public Color colorL { get; set; }

        [DataMember]
        public int colorI { get; set; }

        public StyleColor()
        {
            colorH = Color.FromArgb(255, 6, 146, 199);
            colorL = Color.FromArgb(255, 135, 206, 235);
        }

        public StyleColor(Color h,Color l)
        {
            colorH = h;
            colorL = l;
        }

        public static StyleColor GetColor(string filename)
        {
            StyleColor color = new StyleColor();
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
                            var serializer = new DataContractJsonSerializer(typeof(StyleColor));

                            color = (StyleColor)serializer.ReadObject(fs);
                        }
                    }
                    //文件不存在就新建
                    else if (!iso.FileExists(filename))
                    {
                        using (IsolatedStorageFileStream fs = iso.OpenFile(filename, FileMode.CreateNew, FileAccess.ReadWrite))
                        {
                            var serializer = new DataContractJsonSerializer(typeof(StyleColor));
                            serializer.WriteObject(fs, color);
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
            return color;
        }

        public void SaveColor(string filename)
        {
            //文件写入操作
            try
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream fs = iso.OpenFile(filename, FileMode.Create))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(StyleColor));
                        serializer.WriteObject(fs,this);
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
    }
}
