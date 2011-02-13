<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<T3ME.Business.ViewModels.Forms.LanguageSelectForm>" %>

<% using (Html.BeginForm("SetLanguage", "Language")) { %>
    <fieldset>
        <legend>Language</legend>

        <div class="formRow">
            <%: Html.DropDownListFor(m => m.SelectedLanguage, new SelectList(Model.Languages, "Id", "Name"), "[ Any Language ]") %>
        </div>

        <%: Html.Hidden("returnUrl", Html.RealUrl())%>

        <div class="formButtons">
            <input type="submit" value="Filter" />
        </div>
    </fieldset>
<% } %>