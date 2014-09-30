using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AccumulateYourLife
{
    /// <summary>
    /// 表示元素颜色的类
    /// </summary>
    public class ElementColors
    {
        //表示颜色字符串
        public string text { get; set; }
        //表示颜色
        public SolidColorBrush color { get; set; }
        //颜色的浅色
        public SolidColorBrush colorL { get; set; }
        public ElementColors(string t, Color H)
        {
            text = t;
            color = new SolidColorBrush(H);
            colorL = null;
        }

        public ElementColors(string t,Color H,Color L)
        {
            text = t;
            color = new SolidColorBrush(H);
            colorL = new SolidColorBrush(L);
        }
    }
}
