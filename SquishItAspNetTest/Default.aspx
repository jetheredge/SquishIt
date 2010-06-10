<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SquishItAspNetTest._Default" %>
<%@ Import Namespace="SquishIt.Framework.JavaScript.Minifiers"%>
<%@ Import Namespace="SquishIt.Framework"%>
<%@ Import Namespace="SquishItAspNetTest" %>

<%@ Register src="Menu.ascx" tagname="Menu" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <%= Bundle.JavaScript()
                //CDN order will NOT be maintained in release mode.
                //Any files added with AddCdn will be placed above the
                //generated compressed file.
                .AddCdn("~/js/jquery_1.4.2.js", "http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js")
                .Add("~/js/jquery-ui-1.8rc3.js")
                //.WithMinifier(JavaScriptMinifiers.Closure)
                .RenderOnlyIfOutputFileMissing()
                //.ForceRelease()
                //.ForceDebug()
                .Render("~/js/combined_#.js") %>
                <%
                    Bundle.JavaScript()
                        .Add("~/js/Menu.js")
                        .RenderOnlyIfOutputFileMissing()
                        .AsNamed(Constants.JavaScript.MenuItems, "~/js/menu_#.js"); %> 
                
    <%= Bundle.Css()
                .Add("~/css/jquery-ui-1.8rc3.css")
                .Add("~/css/CodeThinked.css")
                .Add("~/css/extra/extra.css")
                .Add("~/css/testdotless.css.less")
                .WithMedia("screen")
                .ForceRelease()
                //.RenderOnlyIfOutputFileMissing()
                .Render("~/css/combined_#.css") %>
                
    <%= Bundle.Css()
                .Add("~/css/extra/extra.css")
                .WithMedia("screen")
                .ForceRelease()
                //.RenderOnlyIfOutputFileMissing()
                .Render("~/combined_#.css") %>
     <%= Bundle.Css()
                .Add("~/css/extra/extra.css")
                .Add("~/css/import.css")
                .Add("~/css/CodeThinked.css")
                .WithMedia("screen")
                .ForceRelease()
                .Render("~/css/combinedimport_#.css") %>
    <%= Bundle.Css()
                .Add("~/css/import.css")
                .WithMedia("screen")
                .ForceRelease()
                .Render("~/combinedimport_#.css") %>     
    <form id="form1" runat="server">
    <div>
    
        <uc1:Menu ID="Menu1" runat="server" />
    
    </div>
    </form>
</body>
</html>
