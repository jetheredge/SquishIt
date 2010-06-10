<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="SquishItAspNetTest.Menu" %>
<%@ Import Namespace="SquishIt.Framework" %>
<%@ Import Namespace="SquishItAspNetTest" %>
<h3>This set of links was set using the bundler in Defaut.aspx and called in Menu.ascx</h3>
<%=Bundle.JavaScript().RenderNamed(Constants.JavaScript.MenuItems)%>