<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>

<html lang="en">
<head>
    <meta charset="utf-8">
    <title>Twitter Sign-in Complete</title>

    <script>

        window.opener.ajaxLogin();
        window.close();

    </script>
</head>
<body>
    <div>
        <h1>Signed in!</h1>
    </div>
</body>
</html>