<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Medals.aspx.cs" Inherits="SquishItAspNetTest._Medals" %>
<%@ Import Namespace="SquishIt.Framework"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <%= Bundle.JavaScript()
                .Add("~/js/jquery_1.4.2.js")
                .Add("~/js/Medals.coffee")
                .Render("~/js/combined_#.js") %>                
    <form id="form1" runat="server">
    <div>
        <h2>Gold and Silver</h2>
        <ul id="goldandsilver">
        </ul>
        <h2>The Field</h2>
        <ul id="others">
        </ul>    
    </div>
    </form>
</body>
</html>
