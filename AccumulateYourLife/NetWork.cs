using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using LitJson;
using System.Threading;
using System.Windows;

namespace AccumulateYourLife
{
    /// <summary>
    /// 用于网络连接的类
    /// </summary>
    public class NetWork
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        private string URL = "http://aylife.sinaapp.com/";
        /// <summary>
        /// 当前时间
        /// </summary>
        string timesmap;
        /// <summary>
        /// 用户名
        /// </summary>
        string username;
        /// <summary>
        /// 登录验证参数
        /// </summary>
        string accesstoken = "";
        public string Accesstoken
        {
            get { return accesstoken; }
            set { accesstoken = value; }
        }
        /// <summary>
        /// 是否登录
        /// </summary>
        private bool IsSetAccessToken = false;

        /// <summary>
        /// 获取用户登录ID（随时间不同而不同）
        /// </summary>
        /// <returns></returns>
        public string GetAYLID()
        {
            string input = accesstoken + timesmap;
            return MD5.GetMd5String(input);
        }
        /// <summary>
        /// 注册新用户
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <param name="password">用户密码</param>
        /// <returns>注册结果</returns>
        public void Regist(string userid, string password, ResponseComplectedEventHandler rs)
        {
            try
            {
                string url = URL + "regist.php";
                string content = "userid=" + userid + "&password=" + password;
                WebReq http = new WebReq(url, rs);
                http.BeginCreateHtml(content);
            }
            catch
            {
                rs("注册失败！原因未知！");
            }
        }

        /// <summary>
        /// 判断是否已登录
        /// </summary>
        /// <returns></returns>
        public bool Exist()
        {
            try
            {
                accesstoken = "";
                IsSetAccessToken = false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="userid">用户名</param>
        /// <param name="password">登录密码</param>
        /// <returns>返回结果 0表示登录成功 非0表示登录失败</returns>
        public void Login(string userid, string password, ResponseComplectedEventHandler rs)
        {
            if (IsSetAccessToken)
                rs("已经登录成功！");
            username = userid;
            timesmap = DateTime.Now.ToString("yyyyMMddHHmmss");
            string url = URL + "login.php";
            string content = "userid=" + userid + "&password=" + password + "&timesmap=" + timesmap;
            try
            {
                WebReq http = new WebReq(url, rs);
                http.BeginCreateHtml(content);
            }
            catch
            {
                rs("连接网络发生错误！");
            }
        }
        /// <summary>
        /// 上传文本
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <returns>上传结果</returns>
        public void UpLoadText(string text, string cmd, ResponseComplectedEventHandler rs)
        {
            timesmap = DateTime.Now.ToString("yyyyMMddHHmmss");
            string url = URL + "uploadtext.php";
            string content = "userid=" + username + "&timesmap=" + timesmap + "&text=" + text + "&AYLID=" + GetAYLID() + "&accesstoken=" + accesstoken + "&inserttime=" + cmd;
            try
            {
                WebReq http = new WebReq(url, rs);
            }
            catch
            {
                rs("连接网络发生错误！");
            }
        }
        byte[] form_data, foot_data;
        FileStream fileStream;
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="FileName">文件名称</param>
        /// <returns>上传结果</returns>
        public void UpLoadFile(string FileName, FileStream stream, ResponseComplectedEventHandler rs)
        {
            OnUploadFileResponseComplected += new ResponseComplectedEventHandler(rs);
            timesmap = DateTime.Now.ToString("yyyyMMddHHmmss");
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            //请求
            WebRequest req = WebRequest.Create(URL + "uploadpicture.php");
            req.Method = "POST";
            req.ContentType = "multipart/form-data; boundary=" + boundary;
            //组织表单数据
            StringBuilder sb = new StringBuilder();
            sb.Append("--" + boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"userid\"");
            sb.Append("\r\n\r\n");
            sb.Append(username);
            sb.Append("\r\n");
            sb.Append("--" + boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"timesmap\"");
            sb.Append("\r\n\r\n");
            sb.Append(timesmap);
            sb.Append("\r\n");
            sb.Append("--" + boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"AYLID\"");
            sb.Append("\r\n\r\n");
            sb.Append(GetAYLID());
            sb.Append("\r\n");
            sb.Append("--" + boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"accesstoken\"");
            sb.Append("\r\n\r\n");
            sb.Append(accesstoken);
            sb.Append("\r\n");
            sb.Append("--" + boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"picture\"; filename=\"" + FileName + "\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: image/pjpeg");
            sb.Append("\r\n\r\n");
            string head = sb.ToString();
            form_data = Encoding.UTF8.GetBytes(head);
            //结尾
            foot_data = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            //文件
            fileStream = stream;
            //post总长度
            long length = form_data.Length + fileStream.Length + foot_data.Length;
            req.ContentLength = length;
            req.BeginGetRequestStream(new AsyncCallback(UploadFileGetRequsetStream), req);
        }
        /// <summary>
        /// 获取用户单条文本
        /// </summary>
        /// <param name="InsertTime">文本保存时间</param>
        /// <returns>文本内容</returns>
        public void DownLoadText(string InsertTime, ResponseComplectedEventHandler rs)
        {
            timesmap = DateTime.Now.ToString("yyyyMMddHHmmss");
            string url = URL + "downloadtext.php";
            string content = "userid=" + username + "&timesmap=" + timesmap + "&AYLID=" + GetAYLID() + "&accesstoken=" + accesstoken + "&num=1";
            try
            {
                WebReq http = new WebReq(url, rs);
            }
            catch
            {
                rs("连接网络发生错误！");
            }
        }
        /// <summary>
        /// 下载文本
        /// </summary>
        /// <returns>用户所有文本内容（json格式）</returns>
        public void DownLoadTexts(ResponseComplectedEventHandler rs)
        {
            timesmap = DateTime.Now.ToString("yyyyMMddHHmmss");
            string url = URL + "downloadtext.php";
            string content = "userid=" + username + "&timesmap=" + timesmap + "&AYLID=" + GetAYLID() + "&accesstoken=" + accesstoken + "&num=all";
            try
            {
                WebReq http = new WebReq(url, rs);
            }
            catch
            {
                rs("连接网络发生错误！");
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="FileName">文件保存名</param>
        /// <param name="rs">回调函数</param>
        public void DownLoadFile(string FileName, DownLoadPictureComplected rs)
        {
            timesmap = DateTime.Now.ToString("yyyyMMddHHmmss");
            string url = URL + "downloadpicture.php";
            string content = "userid=" + username + "&timesmap=" + timesmap + "&AYLID=" + GetAYLID() + "&accesstoken=" + accesstoken + "&filename=" + FileName;
            try
            {
                WebClient wc = new WebClient();
                wc.OpenReadCompleted += (s, arg) =>
                {
                    if (arg.Result != null)
                    {
                        rs(arg.Result);
                    }
                    else
                    {
                        rs(null);
                    }
                };
            }
            catch
            {

            }
        }
        protected void UploadFileGetRequsetStream(IAsyncResult ar)
        {
            HttpWebRequest http = ar.AsyncState as HttpWebRequest;
            Stream requestStream = http.EndGetRequestStream(ar) as Stream;
            //发送表单参数
            requestStream.Write(form_data, 0, form_data.Length);
            //文件内容
            byte[] buffer = new Byte[checked((uint)Math.Min(4096, (int)fileStream.Length))];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                requestStream.Write(buffer, 0, bytesRead);
            //结尾
            requestStream.Write(foot_data, 0, foot_data.Length);
            requestStream.Close();
            http.BeginGetResponse(new AsyncCallback(UploadFileGetResponse), http);
        }
        protected void UploadFileGetResponse(IAsyncResult ar)
        {
            HttpWebRequest req = ar.AsyncState as HttpWebRequest;
            if (req == null)
                return;
            HttpWebResponse response = req.EndGetResponse(ar) as HttpWebResponse;
            if (response.StatusCode != HttpStatusCode.OK)
            {
                response.Close();
                return;
            }
            Stream st = response.GetResponseStream();
            StreamReader sr = new StreamReader(st);
            string html = sr.ReadToEnd();
            OnUploadFileResponseComplected(html);
        }
        public event ResponseComplectedEventHandler OnUploadFileResponseComplected;
        public void ReqCompleted(string html)
        {

        }
        //////////////////////////////////////////////////////以下是异步请求操作类//////////////////////////////////////////////////////////////////
        public delegate void ResponseComplectedEventHandler(string html);
        public delegate void DownLoadPictureComplected(Stream stream);
        public class WebReq
        {
            ManualResetEvent alldone = new ManualResetEvent(false);
            private HttpWebRequest req = null;
            private HttpWebResponse response = null;
            //URL
            private Uri url;
            //事件
            public event ResponseComplectedEventHandler OnResponseComplected;
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="Url"></param>
            public WebReq(string Url, ResponseComplectedEventHandler rc)
            {
                url = new Uri(Url);
                OnResponseComplected += new ResponseComplectedEventHandler(rc);
            }
            /// <summary>
            /// 获取响应回调函数
            /// </summary>
            /// <param name="ar"></param>
            protected void responseCallback(IAsyncResult ar)
            {
                response = (HttpWebResponse)req.EndGetResponse(ar);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    response.Close();
                    return;
                }
                Stream st = response.GetResponseStream();
                StreamReader sr = new StreamReader(st, UTF8Encoding.UTF8);
                string html = sr.ReadToEnd();
                OnResponseComplected(html);
            }

            protected void GetRequestStreamCallback(IAsyncResult ar)
            {
                Stream requeststream = req.EndGetRequestStream(ar) as Stream;
                requeststream.Write(bytes, 0, bytes.Length);
                requeststream.Close();
                alldone.Set();
                //req.BeginGetResponse(new AsyncCallback(responseCallback), null);
            }
            byte[] bytes;
            /// <summary>
            /// 进行页面请求
            /// </summary>
            public void BeginCreateHtml(string content)
            {
                req = HttpWebRequest.Create(url.AbsoluteUri) as HttpWebRequest;
                bytes = Encoding.UTF8.GetBytes(content);
                req.ContentLength = bytes.Length;
                req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0)";
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "post";
                // req.BeginGetResponse(new AsyncCallback(responseCallback), null);
                req.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), null);
                alldone.WaitOne();
                req.BeginGetResponse(new AsyncCallback(responseCallback), null);
            }
        }
    }
}