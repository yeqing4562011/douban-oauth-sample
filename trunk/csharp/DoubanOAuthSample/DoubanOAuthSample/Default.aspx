<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DoubanOAuthSample._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    to test, enter the protected resouce you want to access:<br />
    <asp:TextBox ID="txtUrl" runat="server" Width="283px">http://api.douban.com/review/1533876</asp:TextBox> 
        <asp:Button ID="btnGet" runat="server" Text="Get" onclick="btnGet_Click" />
        <br />
        <br />
        <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
    </div>
    </form>
</body>
</html>
