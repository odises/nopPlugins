using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;
using System.Windows.Forms;



public partial class Default2 : System.Web.UI.Page
{

    private string refrenceNumber = string.Empty;
    private string reservationNumber = string.Empty;
    private string transactionState = string.Empty;
    bool isError = false;
    string errorMsg = "";
    string succeedMsg = "";
    ir.shaparak.fanava.PaymentWebServiceService PaymentWebService = new ir.shaparak.fanava.PaymentWebServiceService();
    private ir.shaparak.fanava.wsContext context = new ir.shaparak.fanava.wsContext();
    private String SessionId;

    protected void Page_Load(object sender, EventArgs e)
    {

        try
        {
            if (RequestUnpack())
            {


                if (transactionState.Equals("OK"))
                {
                    TRState.Text = transactionState;
                    RefNum.Text = Request.Form["RefNum"];
                    ReNum.Text = Request.Form["ResNum"];
                    ir.shaparak.fanava.loginRequest req = new ir.shaparak.fanava.loginRequest();
                     req.username = "a-1002";
                     req.password = "71550306";
                     SessionId = PaymentWebService.login(req);
                     ir.shaparak.fanava.verifyResponseResult[] results = new ir.shaparak.fanava.verifyResponseResult[1];
                     string[] verifyRequestList = new string[1];

                     try
                     {
                         context = setSessionId(SessionId);
                         verifyRequestList[0] = Request.Form["RefNum"].ToString();
                         results = PaymentWebService.verifyTransaction(context, verifyRequestList);
                         RefNum.Text = results[0].refNum.ToString();
                         ResNum.Text=Request.Form["ResNum"];
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
                        errorMsg=ex.Message;
                     }
                     }

                }
                else
                {
                    TRState.Text = "test";
                    isError = true;
                    errorMsg = "متاسفانه بانک خريد شما را تاييد نکرده است";

                    if (transactionState.Equals("Canceled By User") || transactionState.Equals(string.Empty))
                    {
                        // Transaction was canceled by user
                        isError = true;
                        errorMsg = "تراكنش توسط خريدار كنسل شد";
                    }
                    else if (transactionState.Equals("Invalid Amount"))
                    {
                        // Amount of revers teransaction is more than teransaction
                    }
                    else if (transactionState.Equals("Invalid Transaction"))
                    {
                        // Can not find teransaction
                    }
                    else if (transactionState.Equals("Invalid Card Number"))
                    {
                        // Card number is wrong
                    }
                    else if (transactionState.Equals("No Such Issuer"))
                    {
                        // Issuer can not find
                    }
                    else if (transactionState.Equals("Expired Card Pick Up"))
                    {
                        // The card is expired
                    }
                    else if (transactionState.Equals("Allowable PIN Tries Exceeded Pick Up"))
                    {
                        // For third time user enter a wrong PIN so card become invalid
                    }
                    else if (transactionState.Equals("Incorrect PIN"))
                    {
                        // Pin card is wrong
                    }
                    else if (transactionState.Equals("Exceeds Withdrawal Amount Limit"))
                    {
                        // Exceeds withdrawal from amount limit
                    }
                    else if (transactionState.Equals("Transaction Cannot Be Completed"))
                    {
                        // PIN and PAD are currect but Transaction Cannot Be Completed
                    }
                    else if (transactionState.Equals("Response Received Too Late"))
                    {
                        // Timeout occur
                    }
                    else if (transactionState.Equals("Suspected Fraud Pick Up"))
                    {
                        // User did not insert cvv2 & expiredate or they are wrong.
                    }
                    else if (transactionState.Equals("No Sufficient Funds"))
                    {
                        // there are not suficient funds in the account
                    }
                    else if (transactionState.Equals("Issuer Down Slm"))
                    {
                        // The bank server is down
                    }
                    else if (transactionState.Equals("TME Error"))
                    {
                        // unknown error occur
                    }

                }

            }
        

        catch (Exception ex)
        {

        }


    }

    private bool RequestUnpack()
    {
        if (RequestFeildIsEmpty()) return false;

        refrenceNumber = Request.Form["RefNum"].ToString();
        reservationNumber = Request.Form["ResNum"].ToString();
        transactionState = Request.Form["State"].ToString();

        return true;
    }
    private bool RequestFeildIsEmpty()
    {
        if (Request.Form["State"].ToString().Equals(string.Empty))
        {
            isError = true;
            errorMsg = "خريد شما توسط بانک تاييد شده است اما رسيد ديجيتالي شما تاييد نگشت! مشکلي در فرايند رزرو خريد شما پيش آمده است";
            return true;
        }

        if (Request.Form["RefNum"].ToString().Equals(string.Empty) && Request.Form["State"].ToString().Equals(string.Empty))
        {
            isError = true;
            errorMsg = "فرايند انتقال وجه با موفقيت انجام شده است اما فرايند تاييد رسيد ديجيتالي با خطا مواجه گشت";
            return true;
        }

        if (Request.Form["ResNum"].ToString().Equals(string.Empty) && Request.Form["State"].ToString().Equals(string.Empty))
        {
            isError = true;
            errorMsg = "خطا در برقرار ارتباط با بانک";
            return true;
        }
        return false;
    }
    private string TransactionChecking(int i)
    {
        bool isError = false;
        string errorMsg = "";
        switch (i)
        {

            case -1:		//TP_ERROR
                isError = true;
                errorMsg = "بروز خطا درهنگام بررسي صحت رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-1";
                break;
            case -2:		//ACCOUNTS_DONT_MATCH
                isError = true;
                errorMsg = "بروز خطا در هنگام تاييد رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-2";
                break;
            case -3:		//BAD_INPUT
                isError = true;
                errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-3";
                break;
            case -4:		//INVALID_PASSWORD_OR_ACCOUNT
                isError = true;
                errorMsg = "خطاي دروني سيستم درهنگام بررسي صحت رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-4";
                break;
            case -5:		//DATABASE_EXCEPTION
                isError = true;
                errorMsg = "خطاي دروني سيستم درهنگام بررسي صحت رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-5";
                break;
            case -7:		//ERROR_STR_NULL
                isError = true;
                errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-7";
                break;
            case -8:		//ERROR_STR_TOO_LONG
                isError = true;
                errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-8";
                break;
            case -9:		//ERROR_STR_NOT_AL_NUM
                isError = true;
                errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-9";
                break;
            case -10:	//ERROR_STR_NOT_BASE64
                isError = true;
                errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-10";
                break;
            case -11:	//ERROR_STR_TOO_SHORT
                isError = true;
                errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-11";
                break;
            case -12:		//ERROR_STR_NULL
                isError = true;
                errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-12";
                break;
            case -13:		//ERROR IN AMOUNT OF REVERS TRANSACTION AMOUNT
                isError = true;
                errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-13";
                break;
            case -14:	//INVALID TRANSACTION
                isError = true;
                errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-14";
                break;
            case -15:	//RETERNED AMOUNT IS WRONG
                isError = true;
                errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-15";
                break;
            case -16:	//INTERNAL ERROR
                isError = true;
                errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-16";
                break;
            case -17:	// REVERS TRANSACTIN FROM OTHER BANK
                isError = true;
                errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-17";
                break;
            case -18:	//INVALID IP
                isError = true;
                errorMsg = "خطا در پردازش رسيد ديجيتالي در نتيجه خريد شما تاييد نگرييد" + "-18";
                break;

        }
        return errorMsg;
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


}