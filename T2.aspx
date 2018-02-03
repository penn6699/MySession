<%@ Page Language="C#" AutoEventWireup="true" CodeFile="T2.aspx.cs" Inherits="T2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    User:<%=MySessionContext.Current.Get("User") %>
        <br />
    SessionID:<%=MySessionContext.Current.SessionID %>
    </div>
    </form>
</body>
</html>
