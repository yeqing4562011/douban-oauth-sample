using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Net;
using System.IO;
using System.Text;

namespace DoubanOAuthSample
{
    public class Helper
    {
        public static string ExecuteUrl(string url)
        {
            //encode in utf8
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            //instantiate a request
            WebRequest req = WebRequest.Create(url);
            //wait for response
            WebResponse res = req.GetResponse();
            //get stream object from response
            Stream stream = res.GetResponseStream();
            StreamReader reader = new StreamReader(stream, encode);
            //read the stream into string
            string str = reader.ReadToEnd();
            //clean up
            reader.Close();
            res.Close();
            //return result
            return str;
        }
    }
}
