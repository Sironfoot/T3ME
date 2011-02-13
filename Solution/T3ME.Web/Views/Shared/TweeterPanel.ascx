<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<T3ME.Business.ViewModels.TweeterPanelView>" %>


<% if (Model != null) { %>

<div id="MiniAccountPanel" class="twitterUser signedIn">

    <img src="<%: Model.ImageUrl %>" alt="" />
    <p>
        <span class="hide">You are signed in as </span><a href="<%: TwitterUrls.UserProfile(Model.Username) %>"><%: Model.Username %></a>
    </p>

    <p class="voteCount"><%= Model.TotalVotes %> vote<%= Model.TotalVotes != 1 ? "s" : "" %></p>

    <form method="post" action="/OAuth/Signout">
        <fieldset>
            <legend>Sign-out from <%: Model.ApplicationName %></legend>
            <input type="submit" value="Sign Out" />
        </fieldset>
    </form>

</div>

<% } else { %>

<div id="MiniAccountPanel" class="twitterUser signedOut">
    <a rel="nofollow" href="/OAuth/Signin">Sign-in with Twitter</a>
</div>

<%  } %>