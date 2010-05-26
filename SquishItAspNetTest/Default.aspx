<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SquishItAspNetTest._Default" %>
<%@ Import Namespace="SquishIt.Framework.JavaScript.Minifiers"%>
<%@ Import Namespace="SquishIt.Framework"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <%= Bundle.JavaScript()
                .Add("~/js/jquery-1.4.2.js")
                .Add("~/js/jquery-ui-1.8rc3.js")
                //.WithMinifier(JavaScriptMinifiers.Closure)
                .RenderOnlyIfOutputFileMissing()
                .ForceDebug()
                .Render("~/js/combined_#.js") %>
    <%= Bundle.Css()
                .Add("~/css/jquery-ui-1.8rc3.css")
                .Add("~/css/CodeThinked.css")
                .Add("~/css/extra/extra.css")
                .Add("~/css/testdotless.css.less")
                .WithMedia("screen")
                .ForceDebug()
                //.RenderOnlyIfOutputFileMissing()
                .Render("~/css/combined_#.css") %>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
