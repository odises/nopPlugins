using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class Default4 : System.Web.UI.Page
{
    ir.shaparak.fanava.PaymentWebServiceService PaymentWebService = new ir.shaparak.fanava.PaymentWebServiceService();
    private String SessionId;
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Login_Click(object sender, EventArgs e)
    {
        ir.shaparak.fanava.loginRequest req = new ir.shaparak.fanava.loginRequest();
        req.username = "a-1002";
        req.password = "71550306";
        SessionId = PaymentWebService.login(req);
        Session["SessionId"] = SessionId;
        Response.Redirect("~/TransactionManagement.aspx");
    }
}
