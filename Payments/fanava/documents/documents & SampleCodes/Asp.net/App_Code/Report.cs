using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using System.Web.UI.WebControls;
/// <summary>
/// Summary description for Report
/// </summary>
public class Report
{
     GridView reportgrid= new GridView();
	public Report()
	{
        reportgrid.Width=622;
        reportgrid.Height=322;
		//
		// TODO: Add constructor logic here
		//
    }

        public Report(object[] response)
            
        {
              reportgrid.DataSource = response;
           

        }
    
	}
