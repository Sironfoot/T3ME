<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Top.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    404
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% T3ME.Domain.Models.Application app = (ViewData["MasterView"] as MasterFrontEndView).App; %>

    <header>
        <hgroup>
            <h1>404</h1>
            <h2>Page not found</h2>
        </hgroup>
    </header>

    <div id="Middle">
        <p><%: ViewData["404Tweet"] %></p>
    </div>

    <footer>
        <p>Go back to <a href="/"><%: app.Title %></a></p>
    </footer>

</asp:Content>