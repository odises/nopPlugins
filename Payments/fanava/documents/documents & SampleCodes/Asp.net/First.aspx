<%@ Page Language="C#" AutoEventWireup="true" CodeFile="First.aspx.cs" Inherits="Default2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server" action="https://fanava.shaparak.ir/cardmanager/controller" method="post">
    
    <center>
    
    <fieldset style="width:400px; height:200px; direction:rtl; background-color:Silver;">
    <legend style="font-family:Tahoma; font-size:14px">سایت پذیرنده</legend>
    <table style="font-family:Tahoma; font-size:12px; direction:rtl">
    <tr><td>
        <asp:Label ID="Label2" runat="server" Text="مبلغ خرید"></asp:Label></td><td>
            <asp:TextBox ID="Amount" name="Amount" runat="server" Width="250px"></asp:TextBox></td></tr>
     <tr><td>
         <asp:Label ID="Label3" runat="server" Text="کد خرید"></asp:Label></td><td>
             <asp:TextBox ID="ResNum" name="ResNum" runat="server" Width="250px"></asp:TextBox></td></tr>
      <tr><td>
          <asp:Label ID="Label4" runat="server" Text="کد فروشنده"></asp:Label></td><td>
              <asp:TextBox ID="MID"  name="MID" runat="server" Width="250px"></asp:TextBox></td></tr>
       <tr><td>
         <asp:Label ID="Label5" runat="server" Text="آدرس برگشت"></asp:Label></td><td>
               <asp:TextBox ID="RedirectURL"  name="RedirectURL" runat="server" 
                   value="http://localhost:33991/WebSite1/First.aspx" Width="250px"></asp:TextBox></td></tr>
        <tr><td>
            <asp:Label ID="Label6" runat="server" Text="زبان"></asp:Label></td><td>
                <asp:RadioButton ID="فارسی" runat="server" value="فارسی" name="فارسی" 
                    Width="100px" Text="فارسی"/><asp:RadioButton ID="RadioButton1" runat="server" value="فارسی" name="فارسی" 
                    Width="100px" Text="انگلیسی"/></td></tr>
                    <tr><td>
                         <button type="submit" name="Save" style="font-size: 12px; font-family: tahoma">خرید اینترنتی</button></td>
                         <td></td></tr>
    </table>
    </fieldset>
   
    
  </center>
   
    <hr />
    <center>
    <fieldset style=" font-family:Tahoma; font-size:12px; width:400px; direction:rtl; background-color:Silver">
    <legend>نمایش اطلاعات خرید</legend>
    <div style=" margin-top:20px; color: #CC0000;">
   وضعیت تراکنش:<asp:Label ID="TRState" runat="server"></asp:Label>
    <br />
       کد خرید: <asp:Label ID="ReNum" runat="server"></asp:Label>
       <br />
   شماره پیگیری: <asp:Label ID="RefNum" runat="server"></asp:Label>
        <br />
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Login.aspx">مدیریت تراکنش ها</asp:HyperLink>
        </div>
   </fieldset>
   
   </center>
     </form>
     
    </body>
</html>
