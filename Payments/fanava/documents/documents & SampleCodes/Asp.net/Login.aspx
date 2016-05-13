<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Default4" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    
    <fieldset style="width:400px; direction:rtl; font-family:Tahoma; font-size:12px ; background-color:Silver; position:absolute; left:30%; top:30%">
    <legend>ورود</legend>
    <table><tr><td>نام کاربری:<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox></td>
    <td>کلمه عبور:<asp:TextBox ID="TextBox2" runat="server"></asp:TextBox></td></tr></table>
    <br />
        <asp:Button ID="Login" runat="server" Text="ورود" onclick="Login_Click" 
                        Font-Names="tahoma"  Font-Size="small" Width="100px"/>
      
        </fieldset>
    </div>
    </form>
</body>
</html>
