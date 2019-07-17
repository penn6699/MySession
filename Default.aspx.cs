using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        MySessionContext.Current["User"] = "U"+Guid.NewGuid().ToString("N");
        MySessionContext.Current.Timeout = 1;
        Response.Write("User: " + MySessionContext.Current.Get("User"));
        Response.Write("<br/>User2: " + MySessionContext.Current["User"]);
        Response.Write("<br/>SessionID:" + MySessionContext.Current.SessionID);

    }
}