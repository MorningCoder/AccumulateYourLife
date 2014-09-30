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

namespace AccumulateYourLife
{
    public partial class Help : PhoneApplicationPage
    {
        private int i = StaticClass.ColorIndex;
        public StyleColor c = new StyleColor();
        public List<ElementColors> col = new List<ElementColors>();
        public Help()
        {
            InitializeComponent();
            col.Add(new ElementColors("天蓝", Color.FromArgb(255, 6,146,199),Color.FromArgb(255,135,206,235)));
            col.Add(new ElementColors("草绿", Color.FromArgb(255, 32, 135, 32),Color.FromArgb(255,110,238,110)));
            col.Add(new ElementColors("粉色", Color.FromArgb(255, 241, 46, 144),Color.FromArgb(255,238,118,177)));
            col.Add(new ElementColors("橘红", Color.FromArgb(255, 255, 69, 0),Color.FromArgb(255,255,186,160)));
            col.Add(new ElementColors("棕色", Color.FromArgb(255, 139, 69, 19),Color.FromArgb(255,218,167,130)));
            //MessageBox.Show(this.listpicker.SelectedIndex.ToString());
            this.listpicker.ItemsSource = col;
            this.listpicker.SelectedIndex = i;
            //MessageBox.Show(this.listpicker.SelectedIndex.ToString());
        }

        private void listpicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show("change");
            StaticClass.AppColH = ((ElementColors)listpicker.SelectedItem).color;
            StaticClass.AppColL = ((ElementColors)listpicker.SelectedItem).colorL;
            StaticClass.ColorIndex = listpicker.SelectedIndex;

            c.colorI = StaticClass.ColorIndex;
            c.colorH = StaticClass.AppColH.Color;
            c.colorL = StaticClass.AppColL.Color;
            c.SaveColor(StaticClass.ColorFileName);
            this.LayoutRoot.Background = StaticClass.AppColL;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.LayoutRoot.Background = StaticClass.AppColL;
        }
    }
}