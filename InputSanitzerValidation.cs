using System.Text.RegularExpressions;

public static class InputSanitizer
{
    public static string Sanitize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Normalize whitespace
        input = input.Trim();

        // Remove <script>...</script> blocks (multi-line safe)
        input = Regex.Replace(
            input,
            "<script[^>]*?>.*?</script>",
            "",
            RegexOptions.IgnoreCase | RegexOptions.Singleline
        );

        // Remove inline event handlers: onclick=, onerror=, onload=, etc.
        input = Regex.Replace(
            input,
            @"on\w+\s*=\s*(['""]).*?\1",
            "",
            RegexOptions.IgnoreCase
        );

        // Remove javascript: URLs
        input = Regex.Replace(
            input,
            @"javascript\s*:",
            "",
            RegexOptions.IgnoreCase
        );

        // Remove HTML tags but avoid eating text between < and >
        input = Regex.Replace(
            input,
            @"<[^>]+>",
            ""
        );

        // Remove common injection characters
        input = Regex.Replace(
            input,
            @"[<>""'/]",
            ""
        );

        return input;
    }
}

public static class UsernameValidator
{
    private static readonly Regex UsernameRegex =
        new Regex(@"^[a-zA-Z0-9_-]{3,20}$", RegexOptions.Compiled);

    public static bool IsValid(string username)
    {
        username = InputSanitizer.Sanitize(username);
        return UsernameRegex.IsMatch(username);
    }
}

public static class EmailValidator
{
    private static readonly Regex EmailRegex =
        new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public static bool IsValid(string email)
    {
        email = InputSanitizer.Sanitize(email);
        return EmailRegex.IsMatch(email);
    }
}
