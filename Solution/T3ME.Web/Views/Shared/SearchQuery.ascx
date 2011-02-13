<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TweetViewList>" %>

<% if (!String.IsNullOrWhiteSpace(Model.SearchTerm)) { %>

<div class="searchQuery">

    <p><%: Model.TotalRecords > 0 ? Model.TotalRecords.ToString() : "No" %> Results for "<strong><%: Model.SearchTerm %></strong>"</p>

    <a href="<%: Html.CurrentRoute().RemoveRouteValue("search").RemoveRouteValue("page") %>">Clear Search</a>

</div>

<% } %>