<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TransactionManagement.aspx.cs" Inherits="_Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>



<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
           <style type="text/css">
</style>

 </head>
<body>
    <form id="form1" runat="server">
    <div>

    </div>
    <hr />
      <div>
    <center>
    <fieldset style="width:400px; direction:rtl; font-family:Tahoma; font-size:12px; background-color:Silver">
    <legend>بررسی تراکنش</legend>
    <table><tr><td>شماره پیگیری:<asp:TextBox ID="TextBox3" runat="server"></asp:TextBox></td>
    <td>
        &nbsp;</td>
    </tr></table>
    <br />
        <asp:Button ID="Verify" runat="server" Text="تایید تراکنش"  
                        Font-Names="tahoma"  Font-Size="small" Width="100px" 
                        onclick="Verify_Click1"/>
      
        </fieldset></center>
    </div>
    <hr />
         <div>
    <center>
    <fieldset style="width:400px; direction:rtl; font-family:Tahoma; font-size:12px; background-color:Silver">
    <legend>برگشت تراکنش</legend>
    <table><tr><td>
        <asp:Label ID="Label2" runat="server" Text="شماره پیگیری"></asp:Label></td><td>
            <asp:TextBox ID="refNumReverese" runat="server" ></asp:TextBox></td></tr>
    <tr><td>
        <asp:Label ID="Label3" runat="server" Text="کد خرید"></asp:Label></td><td><asp:TextBox ID="resNumReverese" runat="server"></asp:TextBox></td></tr>
 
     <tr><td>
        <asp:Label ID="Label4" runat="server" Text="مبلغ خرید"></asp:Label></td><td><asp:TextBox ID="amountReverese" runat="server"></asp:TextBox></td></tr>   
    </table>
    <br />
        <asp:Button ID="Reverse" runat="server" Text=" برگشت تراکنش"  
                        Font-Names="tahoma"  Font-Size="small" Width="100px" 
                        onclick="Reverse_Click" />
      
        </fieldset></center>
    </div>
    <hr />
       <div>
    <center>
    <fieldset style="width:400px; direction:rtl; font-family:Tahoma; font-size:12px; background-color:Silver">
    <legend>تاییدامن ترتاییدامن تراکنش </legend>
    <table><tr><td>
        <asp:Label ID="Label5" runat="server" Text="شماره پیگیری"></asp:Label></td><td>
            <asp:TextBox ID="SecureRefNum" runat="server"></asp:TextBox></td></tr>
    <tr><td>
        <asp:Label ID="Label6" runat="server" Text="کد خرید"></asp:Label></td><td><asp:TextBox ID="SecureResNum" runat="server"></asp:TextBox></td></tr>
 </table>
        <asp:Button ID="SecureVerify" runat="server" Text="بررسی تراکنش امن"  
                        Font-Names="tahoma"  Font-Size="small" Width="150px" 
                        onclick="SecureVerify_Click"  />
      
        </fieldset></center>
    </div>
    <hr />
         <div>
    <center>
    <fieldset style="width:400px; direction:rtl; font-family:Tahoma; font-size:12px; background-color:Silver">
    <legend>گزارش تراکنش ها</legend>
    <table><tr><td>
        <asp:Label ID="Label7" runat="server" Text="Offset"></asp:Label></td><td>
            <asp:TextBox ID="Reportoffser" runat="server"></asp:TextBox></td></tr>
    <tr><td>
        <asp:Label ID="Label8" runat="server" Text="Lenght"></asp:Label></td><td><asp:TextBox ID="ReportLenght" runat="server"></asp:TextBox></td></tr>
 </table>
        <asp:Button ID="Report" runat="server" Text="گزارش تراکنش ها"  
                        Font-Names="tahoma"  Font-Size="small" Width="150px" OnClick="Report_Click" 
                         />
      
        </fieldset>
        <asp:GridView ID="GridView1" runat="server" BackColor="White" 
            BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
            GridLines="Horizontal" Width="400px" DataSourceID="SqlDataSource1" EnableModelValidation="True">
            <RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
            <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
            <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
            <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
            <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" />
            <AlternatingRowStyle BackColor="#F7F7F7" />
    </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server"></asp:SqlDataSource>
        </center>
        
    </div>
    <hr />
                 <div>
    <center>
    <fieldset style="width:400px; direction:rtl; font-family:Tahoma; font-size:12px; background-color:Silver">
    <legend>گزارش کامل تراکنش ها    <table><tr><td>
        <asp:Label ID="Label10" runat="server" Text="Transaction ID"></asp:Label></td><td>
            <asp:TextBox ID="TextBox4" runat="server"></asp:TextBox></td></tr>
    <tr><td>
        <asp:Label ID="Label11" runat="server" Text="ReOffset"></asp:Label></td><td><asp:TextBox ID="TextBox5" runat="server"></asp:TextBox></td></tr>
         <tr><td>
        <asp:Label ID="Label12" runat="server" Text="ReLenght"></asp:Label></td><td><asp:TextBox ID="TextBox6" runat="server"></asp:TextBox></td></tr>
 </table>
        <asp:Button ID="DetailReport" runat="server" Text="گزارش کامل تراکنش ها"  
                        Font-Names="tahoma"  Font-Size="small" Width="150px" OnClick="DetailReport_Click" 
                         />
      
        </fieldset>
        </center>
    </div>
    <hr />
        
    </form>
    </body>
</html>
