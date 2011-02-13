<%@ Control Language="C#" Inherits="T3ME.Business.ViewPages.ViewUserControlBase<dynamic>" %>

<% if (DefaultLanguage != null) { %>

    <div class="selectedLanguage">
        <p><span class="hide">Showing tweets </span>for <%: DefaultLanguage.Name %></p>
        <p><a href="<%: Url.Action("ClearLanguage", "Language", new { returnUrl = Html.RealUrl() } )  %>">Clear</a><span class="hide"> language filter</span></p>
    </div>

<% } %>