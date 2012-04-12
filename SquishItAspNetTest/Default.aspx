<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SquishItAspNetTest._Default" %>
<%@ Import Namespace="SquishIt.Framework"%>
<%@ Import Namespace="SquishItAspNetTest" %>

<%@ Register src="Menu.ascx" tagname="Menu" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <%= 
        //this should be loaded by the browser last
        Bundle.JavaScript()
            .AddDirectory("~/js/deferred")
            .WithDeferredLoad()
            .Render("~/js/combined_deferred_#.js")%>
    <%= Bundle.JavaScript()
                //CDN order will NOT be maintained in release mode.
                //Any files added with AddCdn will be placed above the
                //generated compressed file.
                .AddRemote("~/js/jquery_1.4.2.js", "http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js")
                .Add("~/js/jquery-ui-1.8.5.js")
                //.WithMinifier(JavaScriptMinifiers.Closure)
                .ForceRelease()
                //.ForceDebug()
                .Render("~/js/combined_#.js") %>
                <%
                    Bundle.JavaScript()
                        .Add("~/js/Menu.js")
                        .AsNamed(Constants.JavaScript.MenuItems, "~/js/menu_#.js"); %>
                        
    <%= Bundle.JavaScript().RenderNamed("test") %>
                                
    <%= Bundle.Css()
                .AddRemote("~/css/jquery-ui-1.8.5.css", "http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.16/themes/base/jquery-ui.css")
                .Add("~/css/jquery-ui-1.8.5.css")
                .Add("~/css/CodeThinked.css")
                .Add("~/css/extra/extra.css")
                .Add("~/css/testdotless.css.less")
                .AppendHashForAssets()
                .WithAttribute("media", "screen")
                .Render("~/css/combined_#.css") %>
                
    <%= Bundle.Css()
                .Add("~/css/extra/extra.css")
                        .WithAttribute("media", "screen")
                .ForceRelease()
                .Render("~/combined_#.css") %>
     <%= Bundle.Css()
                .Add("~/css/extra/extra.css")
                .Add("~/css/import.css")
                .Add("~/css/CodeThinked.css")
                .WithAttribute("media", "screen")
                .ForceRelease()
                .Render("~/css/combinedimport_#.css") %>
    <%= Bundle.Css()
                .Add("~/css/import.css")
                .WithAttribute("media", "screen")
                .ForceRelease()
                .Render("~/combinedimport_#.css") %>   
    <!-- NonRecursive Test -->  
    <%= Bundle.Css()
                .AddDirectory("~/css/", true)
                .WithAttribute("media", "screen")
                .ForceDebug()
                .Render("~/nonrecursivedirectory_#.css") %>     
    <form id="form1" runat="server">
    <div>
    
        <uc1:Menu ID="Menu1" runat="server" />
    
    </div>
    </form>
</body>
</html>
