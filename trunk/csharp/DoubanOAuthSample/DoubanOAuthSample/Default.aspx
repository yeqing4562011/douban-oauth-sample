<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DoubanOAuthSample._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <b>说明</b><br />
        <br />
        DoubanOAuth.cs里面两个参数，consumerKey和consumerSecret必需填写上你豆瓣的apikey和私钥。<br /><br />
        要测试这个示范，找出你账号里所曾写过的一篇书评，将它的ID替换到下面框框里，如：http://api.douban.com/review/<b>1533876</b>，加重的数字换成你的书评ID。<br /><br />

    <asp:TextBox ID="txtUrl" runat="server" Width="283px">http://api.douban.com/review/1533876</asp:TextBox> 
        <asp:Button ID="btnGet" runat="server" Text="Get" onclick="btnGet_Click" />
        <br />
        <br />
        <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
    </div>
    </form>
</body>
</html>
