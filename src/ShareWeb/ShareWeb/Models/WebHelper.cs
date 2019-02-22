using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.WebPages;

namespace ShareWeb.Models
{
    //生成验证码
    public class VerifyCode
    {
        public byte[] GetVerifyCode()
        {
            int codeW = 80;
            int codeH = 30;
            int fontSize = 16;
            string chkCode = string.Empty;
            Color[] color = { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.Brown, Color.DarkBlue };
            string[] font = { "Times New Roman" };
            char[] character = { '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'd', 'e', 'f', 'h', 'k', 'm', 'n', 'r', 'x', 'y', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y' };
            Random rnd = new Random();
            for (int i = 0; i < 4; i++)
            {
                chkCode += character[rnd.Next(character.Length)];
            }
            //保存到session并用MD5散列算法进行加密
            WebHelper.WriteSession("share_session_verifycode", Md5.GetMD5(chkCode.ToLower(), "verifycode"));
            Bitmap bmp = new Bitmap(codeW, codeH);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            for (int i = 0; i < 3; i++)
            {
                int x1 = rnd.Next(codeW);
                int y1 = rnd.Next(codeH);
                int x2 = rnd.Next(codeW);
                int y2 = rnd.Next(codeH);
                Color clr = color[rnd.Next(color.Length)];
                g.DrawLine(new Pen(clr), x1, y1, x2, y2);
            }
            for (int i = 0; i < chkCode.Length; i++)
            {
                string fnt = font[rnd.Next(font.Length)];
                Font ft = new Font(fnt, fontSize);
                Color clr = color[rnd.Next(color.Length)];
                g.DrawString(chkCode[i].ToString(), ft, new SolidBrush(clr), (float)i * 18, (float)0);
            }
            MemoryStream ms = new MemoryStream();
            try
            {
                bmp.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                g.Dispose();
                bmp.Dispose();
            }
        }
    }

    public class WebHelper
    {
        public static void WriteSession<T>(string key, T value)
        {
            if (key.IsEmpty())
                return;
            HttpContext.Current.Session[key] = value;
        }

        public static void WriteSession(string key, string value)
        {
            WriteSession<string>(key, value);
        }
    }

    //MD5 散列算法
    public class Md5
    {
        public static string GetMD5(string sDataIn, string code)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytValue, bytHash;
            bytValue = System.Text.Encoding.UTF8.GetBytes(sDataIn + code);
            bytHash = md5.ComputeHash(bytValue);
            md5.Clear();
            string sTemp = "";
            for (int i = 0; i < bytHash.Length; i++)
            {
                sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
            }
            return sTemp.ToLower();
        }
    }

    public static class LogHelper
    {
        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <param name="ip">Ip</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="detail">详情</param>
        public static void Log(string email, string type, string title, string content, string ip)
        {
            var entity = new Log
            {
                User = email,
                Type = type,
                Title = title,
                Content = content,
                Ip = ip + "(" + IpHelper.GetLocation(ip) + ")",
                CreateTime = DateTime.Now,
                IsRead = false
            };
            var db = new ShareWebContext();
            db.Logs.Add(entity);
            db.SaveChanges();
        }
    }

    /// <summary>
    /// 模拟登陆课程平台
    /// </summary>
    public class LogingHelper
    {
        public static CookieContainer theCC = new CookieContainer();
        

        public static string Login(string sno, string password)
        {
            string url = "http://eol.sicau.edu.cn/Login";
            string referer = url;
            string paramList = "name=" + sno + "&password=" + password;

            HttpWebResponse res = null;
            HttpWebRequest req = null;
            string strResult = "";
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                //配置请求header     
                req.Headers.Add(HttpRequestHeader.AcceptCharset, "GBK,utf-8;q=0.7,*;q=0.3");
                req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate,sdch");
                req.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-CN,zh;q=0.8");
                req.Accept = "application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
                req.KeepAlive = true;
                req.Referer = referer;
                req.Headers.Add(HttpRequestHeader.CacheControl, "max-age=0");
                req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US) AppleWebKit/534.7 (KHTML, like Gecko) Chrome/7.0.517.5 Safari/534.7";
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.AllowAutoRedirect = true;
                //设置cookieContainer用来接收cookie     
                req.CookieContainer = theCC;
                StringBuilder UrlEncoded = new StringBuilder();
                //对参数进行encode     
                Char[] reserved = { '?', '=', '&' };
                byte[] SomeBytes = null;
                if (paramList != null)
                {
                    int i = 0, j;
                    while (i < paramList.Length)
                    {
                        j = paramList.IndexOfAny(reserved, i);
                        if (j == -1)
                        {
                            UrlEncoded.Append(HttpUtility.UrlEncode(paramList.Substring(i, paramList.Length - i)));
                            break;
                        }
                        UrlEncoded.Append(HttpUtility.UrlEncode(paramList.Substring(i, j - i)));
                        UrlEncoded.Append(paramList.Substring(j, 1));
                        i = j + 1;
                    }
                    SomeBytes = Encoding.UTF8.GetBytes(UrlEncoded.ToString());
                    req.ContentLength = SomeBytes.Length;
                    Stream newStream = req.GetRequestStream();
                    newStream.Write(SomeBytes, 0, SomeBytes.Length);
                    newStream.Close();
                }
                else
                {
                    req.ContentLength = 0;
                }
                //返回请求     
                res = (HttpWebResponse)req.GetResponse();
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                Stream responseStream = null;
                if (res.ContentEncoding.ToLower() == "gzip")
                {
                    responseStream = new System.IO.Compression.GZipStream(res.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                }
                else if (res.ContentEncoding.ToLower() == "deflate")
                {
                    responseStream = new System.IO.Compression.DeflateStream(res.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                }
                else
                {
                    responseStream = res.GetResponseStream();
                }
                StreamReader sr = new StreamReader(responseStream, encode);
                strResult = sr.ReadToEnd();
            }
            catch (Exception)
            {
                //未处理异常
            }
            finally
            {
                res.Close();
            }
            return strResult;
        }
    }
    
    public static class IpHelper
    {

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }

        #region Get

        /// <summary>
        /// HTTP GET方式请求数据.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="headht"></param>
        /// <returns></returns>
        private static string HttpGet(string url, Hashtable headht = null)
        {
            HttpWebRequest request;

            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;
            WebResponse response = null;
            string responseStr = null;
            if (headht != null)
            {
                foreach (DictionaryEntry item in headht)
                {
                    request.Headers.Add(item.Key.ToString(), item.Value.ToString());
                }
            }

            try
            {
                response = request.GetResponse();

                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return responseStr;
        }

        private static string HttpGet(string url, Encoding encodeing, Hashtable headht = null)
        {
            HttpWebRequest request;

            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;
            WebResponse response = null;
            string responseStr = null;
            if (headht != null)
            {
                foreach (DictionaryEntry item in headht)
                {
                    request.Headers.Add(item.Key.ToString(), item.Value.ToString());
                }
            }

            try
            {
                response = request.GetResponse();

                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), encodeing);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return responseStr;
        }
        #endregion


        public static string GetLocation(string ip)
        {
            string res = "";
            try
            {
                string url = "http://apis.juhe.cn/ip/ip2addr?ip=" + ip + "&dtype=json&key=b39857e36bee7a305d55cdb113a9d725";
                res = HttpGet(url);
                var resjson = res.ToObject<objex>();
                res = resjson.result.area + " " + resjson.result.location;
            }
            catch
            {
                res = "";
            }
            if (!string.IsNullOrEmpty(res))
            {
                return res;
            }
            try
            {
                string url = "https://sp0.baidu.com/8aQDcjqpAAV3otqbppnN2DJv/api.php?query=" + ip + "&resource_id=6006&ie=utf8&oe=gbk&format=json";
                res = HttpGet(url, Encoding.GetEncoding("GBK"));
                var resjson = res.ToObject<obj>();
                res = resjson.data[0].location;
            }
            catch
            {
                res = "";
            }
            return res;
        }


        #region Ip(获取Ip)
        /// <summary>
        /// 获取Ip
        /// </summary>
        public static string GetIp()
        {
            var result = string.Empty;
            if (HttpContext.Current != null)
                result = GetWebClientIp();
            if (result.IsEmpty())
                result = GetLanIp();
            return result;
        }

        /// <summary>
        /// 获取Web客户端的Ip
        /// </summary>
        private static string GetWebClientIp()
        {
            var ip = GetWebRemoteIp();
            foreach (var hostAddress in Dns.GetHostAddresses(ip))
            {
                if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                    return hostAddress.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取Web远程Ip
        /// </summary>
        private static string GetWebRemoteIp()
        {
            return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }

        /// <summary>
        /// 获取局域网IP
        /// </summary>
        private static string GetLanIp()
        {
            foreach (var hostAddress in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                    return hostAddress.ToString();
            }
            return string.Empty;
        }

        #endregion

    }
    public static class Json
    {
        public static T ToObject<T>(this string Json)
        {
            return Json == null ? default(T) : JsonConvert.DeserializeObject<T>(Json);
        }
    }
    /// <summary>
    /// 百度接口
    /// </summary>
    public class obj
    {
        public List<dataone> data { get; set; }
    }
    public class dataone
    {
        public string location { get; set; }
    }
    /// <summary>
    /// 聚合数据
    /// </summary>
    public class objex
    {
        public string resultcode { get; set; }
        public dataoneex result { get; set; }
        public string reason { get; set; }
        public string error_code { get; set; }
    }
    public class dataoneex
    {
        public string area { get; set; }
        public string location { get; set; }
    }
}