/* 
 * 本程序提供最基本的Douban OAuth认证的C#示例代码
 * 
 * OAuthBase.cs代码来源于 http://code.google.com/p/oauth/
 * 
 * 更多其他语言版本的Douban OAuth认证示例代码在 http://code.google.com/p/douban-oauth-sample/ 上提供
 * 
 * 有任何疑问，可以到 http://www.douban.com/group/dbapi/ 上提问
 * 
 * 感谢leyang豆友 (http://www.douban.com/people/leyang/) 的大力协助
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using OAuth;

namespace DoubanOAuthBasicSample
{
    /*
     * Douban OAuth认证包括以下四步内容
     * 
     * 1. 获取Request Token，该步骤使用API Key和API Key Secret签名
     * 2. 用户确认授权
     * 3. 换取Access Token，该步骤使用API Key、API Key Secret、Request Token和Request Token Secret签名
     * 4. 访问受限资源，该步骤使用API Key、API Key Secret、Access Token和Access Token Secret签名
     * 
     */
    class Program
    {
        string apiKey = "";
        string apiKeySecret = "";
        string requestToken = "";
        string requestTokenSecret = "";
        string accessToken = "e9cd6627dec6a7e7bd6832779c53f7b5";
        string accessTokenSecret = "c6495005ed0e7fa2";

        Uri requestTokenUri = new Uri("http://www.douban.com/service/auth/request_token");
        Uri accessTokenUri = new Uri("http://www.douban.com/service/auth/access_token");
        string authorizationUri = "http://www.douban.com/service/auth/authorize?oauth_token=";
        Uri miniblogUri = new Uri("http://api.douban.com/miniblog/saying");

       OAuthBase oAuth = new OAuthBase();

        private Dictionary<string, string> parseResponse(string parameters)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(parameters)) {
                string[] p = parameters.Split('&');
                foreach (string s in p)
                    if (!string.IsNullOrEmpty(s))
                        if (s.IndexOf('=') > -1) {
                            string[] temp = s.Split('=');
                            result.Add(temp[0], temp[1]);
                        }
                        else result.Add(s, string.Empty);
            }

            return result;
        }

        //1. 获取Request Token，该步骤使用API Key和API Key Secret签名
        public void getRequestToken()
        {
            Uri uri = requestTokenUri;
            string nonce = oAuth.GenerateNonce();
            string timeStamp = oAuth.GenerateTimeStamp();
            string normalizeUrl, normalizedRequestParameters;

            // 签名
            string sig = oAuth.GenerateSignature(
                uri,
                apiKey,
                apiKeySecret,
                string.Empty,
                string.Empty,
                "GET",
                timeStamp,
                nonce,
                OAuthBase.SignatureTypes.HMACSHA1,
                out normalizeUrl,
                out normalizedRequestParameters);
            sig = HttpUtility.UrlEncode(sig);

            //构造请求Request Token的url
            StringBuilder sb = new StringBuilder(uri.ToString());
            sb.AppendFormat("?oauth_consumer_key={0}&", apiKey);
            sb.AppendFormat("oauth_nonce={0}&", nonce);
            sb.AppendFormat("oauth_timestamp={0}&", timeStamp);
            sb.AppendFormat("oauth_signature_method={0}&", "HMAC-SHA1");
            sb.AppendFormat("oauth_version={0}&", "1.0");
            sb.AppendFormat("oauth_signature={0}", sig);

            Console.WriteLine("请求Request Token的url: \n" + sb.ToString());

            //请求Request Token
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sb.ToString());
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader stream = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
            string responseBody = stream.ReadToEnd();
            stream.Close();
            response.Close();

            Console.WriteLine("请求Request Token的返回值: \n" + responseBody);

            //解析返回的Request Token和Request Token Secret
            Dictionary<string, string> responseValues = parseResponse(responseBody);
            requestToken = responseValues["oauth_token"];
            requestTokenSecret = responseValues["oauth_token_secret"];
        }

        // 2. 用户确认授权
        public void authorization()
        {
            //生成引导用户授权的url
            string url =  authorizationUri + requestToken;

            Console.WriteLine("请将下面url粘贴到浏览器中，并同意授权，同意后按任意键继续:");
            Console.WriteLine(url);
        }

        // 3. 换取Access Token，该步骤使用API Key、API Key Secret、Request Token和Request Token Secret签名
        public void getAccessToken()
        {
            Uri uri = accessTokenUri;
            string nonce = oAuth.GenerateNonce();
            string timeStamp = oAuth.GenerateTimeStamp();
            string normalizeUrl, normalizedRequestParameters;

            // 签名
            string sig = oAuth.GenerateSignature(
                uri,
                apiKey,
                apiKeySecret,
                requestToken,
                requestTokenSecret,
                "GET",
                timeStamp,
                nonce,
                OAuthBase.SignatureTypes.HMACSHA1,
                out normalizeUrl,
                out normalizedRequestParameters);
            sig = HttpUtility.UrlEncode(sig);

            //构造请求Access Token的url
            StringBuilder sb = new StringBuilder(uri.ToString());
            sb.AppendFormat("?oauth_consumer_key={0}&", apiKey);
            sb.AppendFormat("oauth_nonce={0}&", nonce);
            sb.AppendFormat("oauth_timestamp={0}&", timeStamp);
            sb.AppendFormat("oauth_signature_method={0}&", "HMAC-SHA1");
            sb.AppendFormat("oauth_version={0}&", "1.0");
            sb.AppendFormat("oauth_signature={0}&", sig);
            sb.AppendFormat("oauth_token={0}&", requestToken);

            Console.WriteLine("请求Access Token的url: \n" + sb.ToString());

            //请求Access Token
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sb.ToString());
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader stream = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
            string responseBody = stream.ReadToEnd();
            stream.Close();
            response.Close();

            Console.WriteLine("请求Access Token的返回值: \n" + responseBody);

            //解析返回的Request Token和Request Token Secret
            Dictionary<string, string> responseValues = parseResponse(responseBody);
            accessToken = responseValues["oauth_token"];
            accessTokenSecret = responseValues["oauth_token_secret"];
        }

        // 4. 访问受限资源，该步骤使用API Key、API Key Secret、Access Token和Access Token Secret签名
        public void sendMiniBlog()
        {
            Uri uri = miniblogUri;
            string nonce = oAuth.GenerateNonce();
            string timeStamp = oAuth.GenerateTimeStamp();
            string normalizeUrl, normalizedRequestParameters;

            // 签名
            string sig = oAuth.GenerateSignature(
                uri,
                apiKey,
                apiKeySecret,
                accessToken,
                accessTokenSecret,
                "POST",
                timeStamp,
                nonce,
                OAuthBase.SignatureTypes.HMACSHA1,
                out normalizeUrl,
                out normalizedRequestParameters);
            sig = HttpUtility.UrlEncode(sig);

            //构造OAuth头部
            StringBuilder oauthHeader = new StringBuilder();
            oauthHeader.AppendFormat("OAuth realm=\"\", oauth_consumer_key={0}, ", apiKey);
            oauthHeader.AppendFormat("oauth_nonce={0}, ", nonce);
            oauthHeader.AppendFormat("oauth_timestamp={0}, ", timeStamp);
            oauthHeader.AppendFormat("oauth_signature_method={0}, ", "HMAC-SHA1");
            oauthHeader.AppendFormat("oauth_version={0}, ", "1.0");
            oauthHeader.AppendFormat("oauth_signature={0}, ", sig);
            oauthHeader.AppendFormat("oauth_token={0}", accessToken);

            //构造请求
            StringBuilder requestBody =  new StringBuilder("<?xml version='1.0' encoding='UTF-8'?>");
            requestBody.Append("<entry xmlns:ns0=\"http://www.w3.org/2005/Atom\" xmlns:db=\"http://www.douban.com/xmlns/\">");
            requestBody.Append("<content>C# OAuth认证成功</content>");
            requestBody.Append("</entry>");
            Encoding encoding = Encoding.GetEncoding("utf-8");
            byte[] data = encoding.GetBytes(requestBody.ToString());

            // Http Request的设置
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Headers.Set("Authorization", oauthHeader.ToString());
            request.ContentType = "application/atom+xml";
            request.Method = "POST";
            request.ContentLength = data.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader stream = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
                string responseBody = stream.ReadToEnd();
                stream.Close();
                response.Close();

                Console.WriteLine("发送广播成功");
            }
            catch (WebException e)
            {
                StreamReader stream = new StreamReader(e.Response.GetResponseStream(), System.Text.Encoding.UTF8);
                string responseBody = stream.ReadToEnd();
                stream.Close();

                Console.WriteLine("发送广播失败，原因: " + responseBody);
            }
        }

        static void Main(string[] args)
        {

            /* Fix HttpWebRequest vs. lighttpd bug
             * More details in http://www.gnegg.ch/2006/09/lighttpd-net-httpwebrequest/
             */
            System.Net.ServicePointManager.Expect100Continue = false;

            Program p = new Program();
            Console.WriteLine(" 1. 获取Request Token，该步骤使用API Key和API Key Secret签名");
            p.getRequestToken();
            Console.WriteLine();
            Console.WriteLine("2. 用户确认授权");
            p.authorization();
            Console.ReadKey();
            Console.WriteLine();
            Console.WriteLine("3. 换取Access Token，该步骤使用API Key、API Key Secret、Request Token和Request Token Secret签名");
            p.getAccessToken();
            Console.WriteLine();
            Console.WriteLine("4. 访问受限资源，该步骤使用API Key、API Key Secret、Access Token和Access Token Secret签名");
            p.sendMiniBlog();
        }
    }
}
