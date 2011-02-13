<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TweeterStatsView>" %>

<% int middle = (int)Math.Ceiling(Model.Tweeters.Count / 2.0); %>

<%  if (Model.Tweeters.Count > 0) { %>

    <ol class="topTweeters twoColumn leftColumn">
                
        <% foreach (var tweeter in Model.Tweeters.TakeColumn(1, 2)) { %>

        <li>
            <img alt="" src="<%: tweeter.ImageUrl %>" />
            <a href="<%: Model.UsernameLink %>?search=%40<%: tweeter.Username %>"><%: tweeter.Username %></a>
            <span>(<%: tweeter.ItemCount %> <%: Model.CountLabel %>)</span>
        </li>

        <% } %>

    </ol>
            
    <% if (Model.Tweeters.Count > 1) { %>

        <ol start="<%: middle + 1 %>" class="topTweeters twoColumn rightColumn">
                
            <% foreach (var tweeter in Model.Tweeters.TakeColumn(2, 2)) { %>

            <li>
                <img alt="" src="<%: tweeter.ImageUrl %>" />
                <a href="<%: Model.UsernameLink %>?search=%40<%: tweeter.Username %>"><%: tweeter.Username %></a>
                <span>(<%: tweeter.ItemCount %> <%: Model.CountLabel %>)</span>
            </li>

            <% } %>

        </ol>

    <% } %>

<% } else { %>

<p>No records found.</p>

<% } %>