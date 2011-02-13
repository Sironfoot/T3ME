<%@ Page Language="C#" MasterPageFile="~/Views/Shared/TwoColumn.Master"
    Inherits="T3ME.Business.ViewPages.T3meViewPage<TweetViewList>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Latest <%: App.Noun.Plural.CapitaliseFirstLetter() %> from Twitter
    <%  RouteBuilder route = Html.CurrentRoute();

        if (route.ContainsKey("search"))
        { %>
         containing the phrase '<%: route["search"] %>'
     <% }
    %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <section id="TwitterStream">
        <h1>
            Latest <%: App.Noun.Plural.CapitaliseFirstLetter() %> from Twitter
        </h1>

        <% Html.RenderPartial("LanguageLabel"); %>

        <% Html.RenderPartial("SearchQuery", Model); %>

        <% if (Model.Tweets.Count > 0)
           {
               Html.RenderPartial("Tweet", Model);
           }
           else
           { %>
               <p class="noRecordsMessage"><%: Html.NoRecordsMessage(App, DefaultLanguage != null) %></p>
        <% } %>

        <nav id="MainPaging" class="paging">
            <% Html.PagingLinks(Model.RecordsPerPage, Model.TotalRecords)
                   .SetCurrentPage(Model.PageNumber)
                   .SetCutOffRange(16)
                   .SetPageRouteKey("page")
                   .Render(); %>
        </nav>
    </section>

</asp:Content>