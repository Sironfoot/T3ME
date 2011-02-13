<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/TwoColumn.Master"
    Inherits="T3ME.Business.ViewPages.T3meViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	About <%: App.Title %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <section id="StandardContent">
        <h1>About <%: App.Title %></h1>

        <p>
            <%: App.Title %> trawls <a href="<%: TwitterUrls.Domain %>">Twitter</a> for <%: App.Noun.Plural.CapitaliseFirstLetter() %>.
        </p>
            
        <p>
            If you have a Twitter account you can sign-in and vote for your favourite <%: App.Noun.Singular %>.
            Or make your own by tweeting with one of the following hashtags:
        </p>

        <ul>
            <% foreach (string hashtag in App.HashTags) { %>

                <li><a rel="nofollow" href="<%: TwitterUrls.PostMessage("#" + hashtag) %>">#<%: hashtag %></a></li>

            <% } %>
        </ul>

        <p>...and your <%: App.Noun.Singular.ToLower() %> will appear here within a few minutes.</p>

        <p>
            Don't forget to <a href="<%: TwitterUrls.UserProfile(App.Account.ScreenName) %>">follow us on Twitter</a>
            where we'll retweet popular <%: App.Noun.Plural.ToLower() %> that get voted for here.
        </p>

        <hr />

    </section>

</asp:Content>