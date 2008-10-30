using System;
using System.Web;
using System.Text;
using OAuth;

namespace DoubanOAuthSample
{
    public class DoubanOAuth
    {
        static string consumerKey = "";
        static string consumerSecret = "";

        static string nonce;
        static string timeStamp;
        static string normalizeUrl = "";
        static string normalizedRequestParameters = "";
        static string protectedResourceUrl;


        public static void Authorize(string resourceUrl)
        {
            protectedResourceUrl = resourceUrl;
            //first, construct the request_token url
            string url = Step1ConstructURL();
            //second, redirect user to douban for authorization
            Step2Authorize(url);
        }

        public static string GetContent()
        {
            string accessToken = Step3AccessToken();
            return Step4GetContent(protectedResourceUrl, accessToken);
        }

        #region private static functions
        static string Step1ConstructURL()
        {
            Uri uri = new Uri("http://www.douban.com/service/auth/request_token");

            OAuthBase oAuth = new OAuthBase();
            nonce = oAuth.GenerateNonce();
            timeStamp = oAuth.GenerateTimeStamp();
            string sig = oAuth.GenerateSignature(
                uri,
                consumerKey,
                consumerSecret,
                string.Empty,
                string.Empty,
                "GET",
                timeStamp,
                nonce,
                OAuthBase.SignatureTypes.HMACSHA1,
                out normalizeUrl,
                out normalizedRequestParameters);

            sig = HttpUtility.UrlEncode(sig);

            //construct the oauth url
            StringBuilder sb = new StringBuilder(uri.ToString());
            sb.AppendFormat("?oauth_consumer_key={0}&", consumerKey);
            sb.AppendFormat("oauth_nonce={0}&", nonce);
            sb.AppendFormat("oauth_timestamp={0}&", timeStamp);
            sb.AppendFormat("oauth_signature_method={0}&", "HMAC-SHA1");
            sb.AppendFormat("oauth_version={0}&", "1.0");
            sb.AppendFormat("oauth_signature={0}", sig);

            string str = Helper.ExecuteUrl(sb.ToString());

            //get oauth_token and save to session for further use
            string[] token = str.Split('&');
            System.Web.HttpContext.Current.Session["token"] = token;
            System.Web.HttpContext.Current.Session["timeStamp"] = timeStamp;
            System.Web.HttpContext.Current.Session["nonce"] = nonce;

            string url = "http://www.douban.com/service/auth/authorize?" + token[1] + "&oauth_callback=http://localhost:4043/Default.aspx";

            return url;
        }

        static void Step2Authorize(string url)
        {
            System.Web.HttpContext.Current.Response.Redirect(url);
        }

        static string Step3AccessToken()
        {
            string[] token = (string[])System.Web.HttpContext.Current.Session["token"];
            nonce = System.Web.HttpContext.Current.Session["nonce"].ToString();
            timeStamp = System.Web.HttpContext.Current.Session["timeStamp"].ToString();

            Uri uri = new Uri("http://www.douban.com/service/auth/access_token");

            OAuthBase oAuth = new OAuthBase();
            string sig = oAuth.GenerateSignature(
                uri,
                consumerKey,
                consumerSecret,
                token[1].Split('=')[1],
                token[0].Split('=')[1],
                "GET",
                timeStamp,
                nonce,
                OAuthBase.SignatureTypes.HMACSHA1,
                out normalizeUrl,
                out normalizedRequestParameters);

            sig = HttpUtility.UrlEncode(sig);

            StringBuilder sb = new StringBuilder(uri.ToString());
            sb.AppendFormat("?oauth_consumer_key={0}&", consumerKey);
            sb.AppendFormat("oauth_nonce={0}&", nonce);
            sb.AppendFormat("oauth_timestamp={0}&", timeStamp);
            sb.AppendFormat("oauth_signature_method={0}&", "HMAC-SHA1");
            sb.AppendFormat("oauth_version={0}&", "1.0");
            sb.AppendFormat("oauth_signature={0}&", sig);
            sb.AppendFormat(token[1]);

            string str = Helper.ExecuteUrl(sb.ToString());

            return str;
        }
        //oauth_token_secret=4e94f823353e08de&oauth_token=06438a059352de3512ca9e10b9f89a9f&douban_user_id=2588728
        static string Step4GetContent(string protectedResourceUrl, string accessToken)
        {
            string[] token = accessToken.Split('&');
            nonce = System.Web.HttpContext.Current.Session["nonce"].ToString();
            timeStamp = System.Web.HttpContext.Current.Session["timeStamp"].ToString();

            Uri uri = new Uri(protectedResourceUrl);

            OAuthBase oAuth = new OAuthBase();
            string sig = oAuth.GenerateSignature(
                uri,
                consumerKey,
                consumerSecret,
                token[1].Split('=')[1],
                token[0].Split('=')[1],
                "GET",
                timeStamp,
                nonce,
                OAuthBase.SignatureTypes.HMACSHA1,
                out normalizeUrl,
                out normalizedRequestParameters);

            sig = HttpUtility.UrlEncode(sig);

            StringBuilder sb = new StringBuilder(uri.ToString());
            sb.AppendFormat("?oauth_consumer_key={0}&", consumerKey);
            sb.AppendFormat("oauth_nonce={0}&", nonce);
            sb.AppendFormat("oauth_timestamp={0}&", timeStamp);
            sb.AppendFormat("oauth_signature_method={0}&", "HMAC-SHA1");
            sb.AppendFormat("oauth_version={0}&", "1.0");
            sb.AppendFormat("oauth_signature={0}&", sig);
            sb.AppendFormat("oauth_token={0}", token[1].Split('=')[1]);

            string str = Helper.ExecuteUrl(sb.ToString());
            return str;
        }
        #endregion
    }
}
