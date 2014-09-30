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
using LitJson;

namespace AccumulateYourLife
{
    public partial class PageLogin : PhoneApplicationPage
    {
        public PageLogin()
        {
            InitializeComponent();
            this.Background = StaticClass.AppColH;
        }

        private void ApplicationBarIconButtonCheck_Click(object sender, EventArgs e)
        {
            if (!StaticClass.isNet)
            {
                MessageBox.Show("网络无连接，请检查网络设置~");
                return;
            }
            //登陆
            if (this.pivot.SelectedIndex == 0)
            {
                if (uid.Text == "" || psw.Password == "")
                {
                    MessageBox.Show("用户名或密码不能为空");
                    return;
                }

                this.uid.IsEnabled = false;
                this.psw.IsEnabled = false;
                
                StaticClass.network.Login(this.uid.Text, this.psw.Password, LoginCallBack);
            }
            //注册
            else if (this.pivot.SelectedIndex == 1)
            {
                if (textBoxRegistName.Text == "" || passwordBoxFirst.Password == "")
                {
                    MessageBox.Show("用户名或密码不能为空!");
                    return;
                }
                if (!passwordBoxFirst.Password.Equals(passwordBoxSecond.Password))
                {
                    MessageBox.Show("两次输入密码不一致，请重新输入！");
                    return;
                }
                StaticClass.network.Regist(textBoxRegistName.Text, passwordBoxFirst.Password, RegistCallBack);
                this.textBoxRegistName.IsEnabled = false;
                this.passwordBoxFirst.IsEnabled = false;
                this.passwordBoxSecond.IsEnabled = false;
            }
            //异常情况
            else
                return;

            this.progressBar.IsIndeterminate = true;
        }

        public void Callback(string result)
        {
            Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show(result);
            });
        }

        private void LoginCallBack(string result)
        {
            JsonData jd = JsonMapper.ToObject(result);
            int returnResult = Convert.ToInt32(jd["result"].ToString().Trim(new char[] { '[',']'}));
            string returnMsg = (string)jd["message"].ToString().Trim(new char[] { '[', ']' });
            

            Dispatcher.BeginInvoke(() =>
            {
                this.progressBar.IsIndeterminate = false;
                if (returnResult == 1)
                {
                    MessageBox.Show(returnMsg + "请重新登录");
                    StaticClass.isLogin = false;
                    uid.Text = "";
                    psw.Password = "";
                    uid.IsEnabled = true;
                    psw.IsEnabled = true;
                }
                else if (returnResult == 0)
                {
                    StaticClass.isLogin = true;
                    StaticClass.network.Accesstoken = (string)jd["accesstoken"].ToString().Trim(new char[] { '[', ']' });
                    //发送本机uri
                    if (!string.IsNullOrEmpty(StaticClass.MyUri))
                        StaticClass.network.UpLoadText(string.Format("<Uri>{0}</Uri>", StaticClass.MyUri), "uri", Callback);
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
            });
        }

        private void RegistCallBack(string result)
        {
            JsonData jd = JsonMapper.ToObject(result);
            int returnResult = Convert.ToInt32(jd["result"].ToString().Trim(new char[] { '[', ']' }));
            string returnMsg = (string)jd["message"].ToString().Trim(new char[] { '[', ']' });
            
            Dispatcher.BeginInvoke(() =>
            {
                this.progressBar.IsIndeterminate = false;
                if(returnResult == 0)
                    MessageBox.Show("注册成功");
                else if (returnResult == 1)
                    MessageBox.Show(returnMsg + "注册失败");
                textBoxRegistName.Text = "";
                passwordBoxFirst.Password = "";
                passwordBoxSecond.Password = "";
                uid.IsEnabled = true;
                psw.IsEnabled = true;
                
            });
        }

        private void ApplicationBarMenuItemHelp_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.LayoutRoot.Background = StaticClass.AppColL;
            this.ApplicationBar.BackgroundColor = StaticClass.AppColH.Color;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            MessageBoxResult LeaveResult = MessageBox.Show("确定退出应用？", "退出", MessageBoxButton.OKCancel);
            if (LeaveResult == MessageBoxResult.OK)
            {
                StaticClass.network.Accesstoken = null;
                Application.Current.Terminate();
                //退出的处理

            }
            else
                e.Cancel = true;
        }
    }
}