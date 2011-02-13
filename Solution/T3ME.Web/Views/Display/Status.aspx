<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Main.Master" Inherits="T3ME.Business.ViewPages.T3meViewPage<TweetFullRecordView>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%= Model.Message.ParseTweetForPageTitle() %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% string nounSingular = App.Noun.Singular.ToLower(); %>

    <section id="TweetDetail" class="<%= Model.HasBeenVotedByUser ? "votedByUser" : "" %><%= Model.TotalVotes > 0 ? " voted" : "" %>">
        <h1><%: nounSingular.CapitaliseFirstLetter() %> by <%: Model.Username %></h1>

        <article>
            <p><%= Html.ParseTweetText(Model.Message) %></p>

            <time datetime="<%: Model.DatePosted.ToHtml5TimeString() %>">
                <span class="hide">Posted </span><%: Model.DatePosted.ToTwitterString() %> via <a href="<%: Model.DeviceUrl %>"><%: Model.DeviceName%></a>
            </time>

            <ul class="actions">
                <li class="retweet"><a rel="nofollow" href="<%: TwitterUrls.Retweet(Model.Username, Model.Message) %>">Retweet</a></li>
                <li class="reply"><a rel="nofollow" href="<%: TwitterUrls.Reply(Model.Username, Model.TwitterId) %>">Reply</a></li>
            </ul>

            <% using (Html.BeginForm("RegisterVote", "Vote", FormMethod.Post, new { @class = "voteButton" })) { %>
                <fieldset>
                    <legend>Vote for <%: nounSingular%> by <%: Model.Username %></legend>
                    <%= Html.Hidden("tweetId", Model.TwitterId) %>
                    <input type="submit" value="Vote" <%= Model.HasBeenVotedByUser ? "disabled=\"disabled\"" : "" %>
                        title="<%= Model.HasBeenVotedByUser ? "You have already voted for this " + Html.Encode(nounSingular) : "Vote for this " + Html.Encode(nounSingular) %>" />
                
                    <%: Html.AntiForgeryToken() %>
                </fieldset>
            <% } %>
        </article>

        <footer>
            <div class="avatarVotes">
                <img src="<%: Model.ProfileImageUrl %>" alt="" />
                <p><%= Model.TotalVotes > 0 ? ("+" + Model.TotalVotes) : "0" %><span class="hide"> votes</span></p>
            </div>

            <div class="nameDetails">
                <p class="username"><a href="<%: TwitterUrls.UserProfile(Model.Username) %>"><%: Model.Username %></a></p>
                <p class="fullName"><%: Model.FullName %></p>
            </div>

            <% if (Model.Voters.Count > 0) { %>

                <div class="votedDetails">
                    <p>Voted By:</p>

                    <ul>
                        <% foreach (var voter in Model.Voters) { %>

                            <li>
                                <a href="<%: TwitterUrls.UserProfile(voter.Username) %>">
                                    <strong><%: voter.Username %></strong>
                                    <img src="<%: voter.ProfileImageUrl %>" alt="Profile image for <%: voter.Username %>" title="<%: voter.Username %>" />
                                </a>
                            </li>

                        <% } %>
                    </ul>

                    <% int excess = Model.TotalVotes - Model.Voters.Count;
                       if (excess > 0) { %>

                        <span>+ <%: excess %> more</span>

                    <% } %>
                </div>

            <% } %>

            <div class="options">
                <% if (Model.TotalTweetsByTweeter > 1) { %>
                    <p>
                        <a href="/?search=%40<%: Model.Username %>">
                            View All <%: App.Noun.Plural.CapitaliseFirstLetter() %> by <strong><%: Model.Username %></strong> (<%= Model.TotalTweetsByTweeter %>)
                        </a>
                    </p>
                <% } %>
            </div>
        </footer>

    </section>

</asp:Content>