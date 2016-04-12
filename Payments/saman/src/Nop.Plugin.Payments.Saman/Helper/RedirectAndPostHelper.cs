using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.Saman.Helper
{
    public class RedirectAndPostHelper
    {
        public static string BuildPostForm(string Url, Dictionary<string, object> PostData)
        {
            string formId = "__PostForm";

            StringBuilder strForm = new StringBuilder();
            strForm.Append(string.Format("<form id=\"{0}\" name=\"{0}\" action=\"{1}\" method=\"POST\">", formId, Url));
            foreach (var item in PostData)
            {
                strForm.Append(string.Format("<input type=\"hidden\" name=\"{0}\" value=\"{1}\"/>", item.Key, item.Value));
            }
            strForm.Append("</form>");

            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script language=\"javascript\">");
            strScript.Append(string.Format("var v{0}=document.{0};", formId));
            strScript.Append(string.Format("v{0}.submit();", formId));
            strScript.Append("</script>");

            return strForm.ToString() + strScript.ToString();
        }
    }
}
