<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BundlerAspNetTest._Default" %>
<%@ Import Namespace="Bundler.Framework"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <%= Bundle.JavaScript()
                .AddJs("~/js/jquery-1.4.2.js")
                .AddJs("~/js/jquery-ui-1.8rc3.js")
                .RenderJs("~/js/combined.js") %>
    <%= Bundle.Css()
                .AddCss("~/css/jquery-ui-1.8rc3.css")
                .AddCss("~/css/CodeThinked.css")
                .AddCss("~/css/testdotless.css.less")
                .RenderCss("~/css/combined.css") %>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
