using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Net;
using System.IO;
using System.Text;
using OAuth;

namespace DoubanOAuthSample
{
    public partial class _Default : System.Web.UI.Page
    {
        string oauth_token;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {
                    oauth_token = Request.QueryString["oauth_token"];
                    if (!string.IsNullOrEmpty(oauth_token))
                    {
                        //user has grant permission
                        lblResult.Text = DoubanOAuth.GetContent();
                    }
                }
                catch { }
            }
        }

        protected void btnGet_Click(object sender, EventArgs e)
        {
            DoubanOAuth.Authorize(txtUrl.Text.Trim());
        }
    }
}
