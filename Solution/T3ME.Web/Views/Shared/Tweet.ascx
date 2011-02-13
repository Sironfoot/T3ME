<%@ Control Language="C#" Inherits="T3ME.Business.ViewPages.ViewUserControlBase<TweetViewList>" %>

<% string nounSingular = App.Noun.Singular.ToLower(); %>

<% foreach (var tweet in Model.Tweets)
   { %>

    <% string viewTweetUrl = Url.Action("Status", "Display", new { twitterId = tweet.TwitterId, message = tweet.Message.ToFriendlyUrl() }); %>

    <article data-tweetId="<%: tweet.TwitterId %>" class="<%= tweet.HasBeenVotedByUser ? "votedByUser" : "" %><%= tweet.TotalVotes > 0 ? " voted" : "" %>">
        <strong><a href="<%: TwitterUrls.UserProfile(tweet.Username) %>"><%: tweet.Username%></a></strong>
        <p><%= Html.ParseTweetText(tweet.Message)%></p>
        <time datetime="<%: tweet.DatePosted.ToHtml5TimeString() %>">
            <span class="hide">Posted </span><a href="<%: viewTweetUrl %>"><%: tweet.DatePosted.ToTwitterString()%></a> via
            <a href="<%: tweet.DeviceUrl %>"><%: tweet.DeviceName%></a>
        </time>

        <div class="avatarVotes">
            <img src="<%: tweet.ProfileImageUrl %>" alt="" />
            <p><span><%: tweet.TotalVotes%></span><span class="hide"> vote<%: tweet.TotalVotes != 1 ? "s" : ""%></span></p>
        </div>

        <div class="actions">
            <a class="primaryAction" href="<%: viewTweetUrl %>">View Tweet</a>

            <ul class="secondaryActions">
                <li class="retweet"><a rel="nofollow" href="<%: TwitterUrls.Retweet(tweet.Username, tweet.Message) %>">Retweet</a></li>
                <li class="reply"><a rel="nofollow" href="<%: TwitterUrls.Reply(tweet.Username, tweet.TwitterId) %>">Reply</a></li>
            </ul>
        </div>

        <%  if(tweet.HasBeenVotedByUser)
            { %>
                <p class="userVotedMessage">You have voted for this tweet.</p>
        <%  }
            else
            {
                using (Html.BeginForm("RegisterVote", "Vote", FormMethod.Post, new { @class = "voteButton" }))
                { %>
                <fieldset>
                    <legend>Vote for <%: nounSingular%> by <%: tweet.Username%></legend>
                    <%= Html.Hidden("tweetId", tweet.TwitterId)%>
                    <input type="submit" value="Vote" <%= tweet.HasBeenVotedByUser ? "disabled=\"disabled\"" : "" %>
                        title="<%= tweet.HasBeenVotedByUser ? "You have already voted for this " + Html.Encode(nounSingular) : "Vote for this " + Html.Encode(nounSingular) %>" />

                    <%: Html.AntiForgeryToken()%>
                </fieldset>
            <%  } 
           } %>

    </article>

<% } %>