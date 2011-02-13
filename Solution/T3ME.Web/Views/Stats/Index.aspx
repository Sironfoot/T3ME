<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TwoColumn.master" Inherits="System.Web.Mvc.ViewPage<T3ME.Business.ViewModels.StatsView>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Stats
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <section id="StandardContent">
        <h1>Interesting Stats</h1>

        <% Html.RenderPartial("LanguageLabel"); %>

        <h2>Most Frequent Tweeters</h2>
        <% Html.RenderPartial("TweeterStatList", Model.MostFrequentTweeters); %>

        <h2>Tweeters With Most Number of Votes</h2>
        <% Html.RenderPartial("TweeterStatList", Model.TweetersWithMostVotes); %>

        <h2>Most Popular Hashtags</h2>

        <% var tags = Model.MostPopualarHashtags; %>
        <% int middle = (int)Math.Ceiling(tags.Count / 2.0); %>

        <% if (tags.Count > 0) { %>

            <ol class="tagList twoColumn leftColumn">
                <% foreach (var tag in tags.TakeColumn(1, 2)) { %>

                    <li><a href="/?search=%23<%: tag.Name %>"><%: tag.Name %></a> <span>(<%: tag.UsageCount %>)</span></li>

                <% } %>
            </ol>

            <% if (tags.Count > 1) { %>

                <ol start="<%: middle + 1 %>" class="tagList twoColumn rightColumn">
                
                    <% foreach (var tag in tags.TakeColumn(2, 2)) { %>

                    <li><a href="/?search=%23<%: tag.Name %>"><%: tag.Name %></a> <span>(<%: tag.UsageCount %>)</span></li>

                    <% } %>

                </ol>

            <% } %>

        <% } else { %>
        <p>No hashtags found.</p>
        <% } %>

        <h2>Misc. Stats</h2>

        <dl class="miscStats">
            <dt>Total Proverbs in Database</dt>
            <dd><%: Model.TotalTweets %></dd>

            <dt>Total Votes Cast</dt>
            <dd><%: Model.TotalVotes %></dd>

            <% if (Model.HasSelectedLanguage) { %>

            <dt>Total Proverbs in <%: Model.LanguageName %></dt>
            <dd><%: Model.TotalTweetsForLanguage %></dd>

            <dt>Total Votes for proverbs in <%: Model.LanguageName %></dt>
            <dd><%: Model.TotalVotesForLanguage %></dd>

            <% } %>
        </dl>

    <//section>

</asp:Content>