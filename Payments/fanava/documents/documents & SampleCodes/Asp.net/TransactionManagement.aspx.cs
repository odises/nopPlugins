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
using System.Windows.Forms;

public partial class _Default : System.Web.UI.Page
{
    ir.shaparak.fanava.PaymentWebServiceService PaymentWebService = new ir.shaparak.fanava.PaymentWebServiceService();
    private ir.shaparak.fanava.wsContext context = new ir.shaparak.fanava.wsContext();
    private String SessionId;
    protected void Page_Load(object sender, EventArgs e)
    {
      
    }

    private ir.shaparak.fanava.wsContext setSessionId(String sessionId)
    {

        ir.shaparak.fanava.wsContextEntry[] ContextEntry = new ir.shaparak.fanava.wsContextEntry[1];
        ir.shaparak.fanava.wsContextEntry ContextObj = new ir.shaparak.fanava.wsContextEntry();

        ContextObj.key = "SESSION_ID";
        ContextObj.value = sessionId;
        ContextEntry[0] = ContextObj;
        context.data = ContextEntry;
        return context;
    }
    protected void Reverse_Click(object sender, EventArgs e)
    {
        SessionId = Session["SessionId"].ToString();
        ir.shaparak.fanava.reverseRequest RevereseTr = new ir.shaparak.fanava.reverseRequest();
        ir.shaparak.fanava.reverseResponse Response = new ir.shaparak.fanava.reverseResponse();
     
        context = setSessionId(SessionId);
        try
        {
            
            RevereseTr.mainTransactionRefNum = refNumReverese.Text;
            RevereseTr.reverseTransactionResNum = resNumReverese.Text;
            RevereseTr.amount = Convert.ToDecimal(amountReverese.Text);
            RevereseTr.amountSpecified = true;
            Response = PaymentWebService.reverseTransaction(context, RevereseTr);
          
        }
      
        catch (FormatException ex)
        {
            MessageBox.Show(ex.Message);
           
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
          
        }
    }

    protected void SecureVerify_Click(object sender, EventArgs e)
    {
        SessionId = Session["SessionId"].ToString(); ;
        ir.shaparak.fanava.secureVerifyResponseResult[] Results = new ir.shaparak.fanava.secureVerifyResponseResult[1];
       ir.shaparak.fanava.secureVerifyInfo[] SecureVerifyRequestList = new ir.shaparak.fanava.secureVerifyInfo[1];
        SecureVerifyRequestList[0] = new ir.shaparak.fanava.secureVerifyInfo();
        context = setSessionId(SessionId);
        
        try
        {
            SecureVerifyRequestList[0].refNum = SecureRefNum.Text;
            SecureVerifyRequestList[0].resNum = SecureResNum.Text;

            Results = PaymentWebService.secureVerifyTransaction(context,SecureVerifyRequestList);
         
            if (Results != null && Results.Length != 0)
            {
                if (Results[0].verificationErrorSpecified)
                    MessageBox.Show(Results[0].verificationError.ToString());
              
                else
                    MessageBox.Show(Results[0].amount.ToString());
                
            }
        }
      
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);

        }
    }
    protected void Report_Click(object sender, EventArgs e)
    {
        SessionId = Session["SessionId"].ToString();
        ir.shaparak.fanava.reportResponse Response = new ir.shaparak.fanava.reportResponse();
        ir.shaparak.fanava.reportResponseResult[] ResponseList = new ir.shaparak.fanava.reportResponseResult[1];
        ir.shaparak.fanava.reportRequest ReportRequestList = new ir.shaparak.fanava.reportRequest();
        ResponseList[0] = new ir.shaparak.fanava.reportResponseResult();
        
        try
        {
            context = setSessionId(SessionId);

            ReportRequestList.offset = Int64.Parse(Reportoffser.Text);
            ReportRequestList.length = Int16.Parse(ReportLenght.Text);
            ReportRequestList.orderField = ir.shaparak.fanava.orderField.TRANSACTION_TIME;
            ReportRequestList.orderFieldSpecified = true;
            ReportRequestList.orderType = ir.shaparak.fanava.orderType.DESC;
            ReportRequestList.orderTypeSpecified = true;
            ReportRequestList.onlyReversed = false;
         

            Response = PaymentWebService.reportTransaction(context, ReportRequestList);
            
            if (Response.totalRecord > 0)
            {
                Session["DataSource"] = Response.reportResponseResults;
                string url = "ReportView.aspx";
                string s = "window.open('" + url + "', 'popup_window', 'width=300,height=100,left=100,top=100,resizable=yes');";
                ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
            }
            else
            {
                MessageBox.Show("Nothing find for this request.");
            
            }
        }
       
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
           
        }
    }
    protected void DetailReport_Click(object sender, EventArgs e)
    {
        SessionId = Session["SessionId"].ToString();
        ir.shaparak.fanava.detailReportRequest DetailReport = new ir.shaparak.fanava.detailReportRequest();
        ir.shaparak.fanava.detailReportResponse DetailReportResponse = new ir.shaparak.fanava.detailReportResponse();

        context = setSessionId(SessionId);

        try
        {
            if (TextBox4.Text != "")
            {
                DetailReport.mainTransactionId = Convert.ToInt64(TextBox4.Text);
                DetailReport.mainTransactionIdSpecified = true;
            }
            if (TextBox6.Text != "")
                DetailReport.length = Convert.ToInt16(TextBox6.Text);
            if (TextBox5.Text != "")
                DetailReport.offset = Convert.ToInt64(TextBox5.Text);

            DetailReportResponse = PaymentWebService.detailReportTransaction(context, DetailReport);

            if (DetailReportResponse.totalRecord > 0)
            {
             
               Session["DataSource"] = DetailReportResponse.detailReportResponseResults;
               string url = "ReportView.aspx";
               string s = "window.open('" + url + "', 'popup_window', 'width=600,height=300,left=300,top=100,resizable=yes');";
               ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
            }
            else
            {
                MessageBox.Show("Nothing find for this request.");
            
            }

        }
        
        catch (FormatException ex)
        {
            MessageBox.Show("1");
            
        }
        catch (Exception ex)
        {
            
          
        }
    }
    protected void Verify_Click1(object sender, EventArgs e)
    {
       
        ir.shaparak.fanava.verifyResponseResult[] results = new ir.shaparak.fanava.verifyResponseResult[1];
        string[] verifyRequestList = new string[1];

       try
        {
            SessionId = Session["SessionId"].ToString();
            context = setSessionId(SessionId);
          
            verifyRequestList[0] = TextBox3.Text;
            results = PaymentWebService.verifyTransaction(context, verifyRequestList);
           
            if (results != null && results.Length != 0)
            {
                if (results[0].verificationErrorSpecified)

                   
                    MessageBox.Show(results[0].verificationError.ToString());
               
                else
                    MessageBox.Show(results[0].amount.ToString());
                    
            }
        }
        
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
           
        }
    }
    protected void Button3_Click(object sender, EventArgs e)
    {
        
    }
}
