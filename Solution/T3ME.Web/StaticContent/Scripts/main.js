/* --- Handles Ajax Voting when clicking on Vote buttons --- */
var ajaxVoting = function (buttonControl)
{
    var form = buttonControl.parents('form.voteButton');

    var signinUrl = $('#MiniAccountPanel > a').attr('href');

    if ($('#MiniAccountPanel').hasClass('signedOut'))
    {
        // TODO: Put this in a better place
        window.tempTweetId = form.find('input[name=tweetId]').val();
        twitterSigninPopup(signinUrl);

        return;
    }

    var tweetContainer = form.parent();

    $.post(form.attr('action'), form.serialize(), function (data)
    {
        if (data === 'ok')
        {
            tweetContainer
                    .addClass('votedByUser')
                    .addClass('voted');

            form.remove();

            tweetContainer.append('<p class="userVotedMessage">You have voted for this tweet.</p>');

            var voteContainer = tweetContainer.find('.avatarVotes > p > span:first');

            var totalVotes = parseInt(voteContainer.text());

            voteContainer.text(totalVotes + 1);
        }
        else if (data === 'signin')
        {
            window.location = signinUrl;
        }
    });
}

var newAjaxTweets;
var numNewTweets = 0;

var checkForNewTweets = function ()
{
    var latestTweet = $('#TwitterStream > article:first');

    if (latestTweet.length > 0)
    {
        var lastTweetId = latestTweet.attr('data-tweetId');

        var newTweetUrl = '/Ajax/NewTweetsSince?lastTweetId=' + lastTweetId +
            (searchQuery != '' ? '&search=' + searchQuery : '') + (searchOrder != '' ? '&order=' + searchOrder : '');

        $.get(newTweetUrl, function (htmlSnippet)
        {
            if ($.trim(htmlSnippet).length > 0)
            {
                newAjaxTweets = htmlSnippet;

                numNewTweets = jQuery('<div>' + newAjaxTweets + '</div>').find('article').length;

                var ajaxAlertPanel = $('#TwitterStream div.ajaxTweetAlertPanel');

                if (ajaxAlertPanel.length == 0)
                {
                    var markup = '<div class="ajaxTweetAlertPanel" style="display: none;">' +
                                    '<p>' +
                                        '<strong>' + numNewTweets + '</strong> more tweet<span class="plural">' + (numNewTweets != 1 ? 's' : '') + '</span>. ' +
                                        '<a href="/">Refresh</a> to see them.</p>' +
                                 '</div>';

                    $('#TwitterStream > h1').after(markup);

                    ajaxAlertPanel = $('#TwitterStream div.ajaxTweetAlertPanel');

                    ajaxAlertPanel.find('p > a').click(function ()
                    {
                        var indexToRemoveFrom = recordsPerPage - numNewTweets;
                        $('#TwitterStream > article:gt(' + (indexToRemoveFrom - 1) + ')').remove();

                        $('#TwitterStream > article:first').before(newAjaxTweets);

                        $('#TwitterStream > article:lt(' + numNewTweets + ')').effect("highlight", {}, 1500);

                        ajaxAlertPanel.remove();

                        return false;
                    });

                    ajaxAlertPanel.fadeIn(1000);
                }

                ajaxAlertPanel.find('p > strong').text(numNewTweets);
                if (numNewTweets > 1)
                {
                    ajaxAlertPanel.find('p > span.plural').text('s');
                }
            }

            setTimeout(function () { checkForNewTweets(); }, 1000 * 60);
        });
    }
}

$(document).ready(function ()
{
    var pagingControl = $('#MainPaging');

    pagingControl.addClass('jsPaging');
    pagingControl.empty();

    var forwardPage = currentPage;
    var backwardPage = currentPage;

    var pagingClickEvent = function (event)
    {
        var page = event.data.page;
        var direction = event.data.direction;

        var control = $(this);
        control.unbind('click');
        control.addClass('jsPagingWait');

        var url = pagingUrl.replace('xxx', page);

        $.get(url, function (htmlSnippet)
        {
            control.removeClass('jsPagingWait');

            var totalTweets = jQuery('<div>' + htmlSnippet + '</div>').find('article').length;

            if (totalTweets > 0)
            {
                if (direction === 'forward')
                {
                    $('#TwitterStream > article:last').after(htmlSnippet);
                }
                else
                {
                    $('#TwitterStream > article:first').before(htmlSnippet);
                }
            }

            if (totalTweets != recordsPerPage || page === 1)
            {
                control.addClass('jsPagingEnd');
                control.find('strong').text('No More Tweets');

                if (page === 1)
                {
                    setTimeout(function ()
                    {
                        control.fadeOut(500);
                    }, 1500);
                }
            }
            else
            {
                control.bind('click', { page: direction === 'forward' ? ++page : --page, direction: direction }, pagingClickEvent);
            }
        });
    }

    if (currentPage > 1)
    {
        var prevPagingControl = pagingControl
            .clone()
            .append('<strong>More</strong>')
            .insertBefore('#TwitterStream > article:first');

        prevPagingControl.bind('click', { page: --currentPage, direction: 'backward' }, pagingClickEvent);
    }

    if ($('#TwitterStream > article').length === recordsPerPage)
    {
        pagingControl.append('<strong>More</strong>');
        pagingControl.bind('click', { page: ++currentPage, direction: 'forward' }, pagingClickEvent);
    }
    else
    {
        pagingControl.addClass('jsPagingEnd');
        pagingControl.append('<strong>No More Tweets</strong>');

        if ($('#TwitterStream > article').length === 0)
        {
            pagingControl.remove();
        }
    }

    $('.voteButton input[type=submit]:enabled').live('click', function (event)
    {
        event.preventDefault();

        var button = $(this);
        ajaxVoting(button);
    });

    $('.twitterUser.signedOut > a').click(function (event)
    {
        event.preventDefault();

        twitterSigninPopup($(this).attr('href'));
    });

    if (runAjaxNewTweetsCheck)
    {
        setTimeout(function () { checkForNewTweets(); }, 1000 * 10);
    }
});


function twitterSigninPopup(url)
{
    window.open(url + '?popup=true', 'TwitterLogin', 'status=0, toolbar=0, menubar=0, directories=0, resizable=0, scrollbars=0, width=800, height=370');
}

function ajaxLogin()
{
    $.get('/OAuth/CurrentUserDetail', function (tweeterPanelHtml)
    {
        $('#MiniAccountPanel').replaceWith(tweeterPanelHtml);

        checkTweetsForVotes();

        if (window.tempTweetId)
        {
            var voteButton = $('#TwitterStream form.voteButton input[value=' + window.tempTweetId + '] ~ input[type=submit]:enabled');
            if (voteButton.length > 0)
            {
                ajaxVoting(voteButton);
            }

            window.tempTweetId = null;
        }
    });
}

function checkTweetsForVotes()
{
    var tweetIds = [];

    $('#TwitterStream form.voteButton input[name=tweetId]').each(function ()
    {
        tweetIds.push($(this).val());
    });

    $.ajax({
        type: 'POST',
        url: '/Ajax/CheckTweetsForVotes',
        dataType: 'json',
        data: { tweetIds: tweetIds.join(',') },
        success: function (votes, status)
        {
            

            for (var i = 0; i < votes.length; i++)
            {
                var hasVoted = votes[i].HasVoted;

                if (hasVoted)
                {
                    var tweetId = votes[i].TwitterId;

                    var hiddenVoteField = $('#TwitterStream form.voteButton input[value=' + tweetId + ']');
                    if (hiddenVoteField.length > 0)
                    {
                        var articleElement = hiddenVoteField.parents('article');

                        articleElement.addClass('votedByUser').addClass('voted');

                        articleElement.find('.voteButton').remove();

                        articleElement.append('<p class="userVotedMessage">You have voted for this tweet.</p>');
                    }
                }
            }
        }
    });
}