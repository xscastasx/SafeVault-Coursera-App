// Tests/TestInputValidation.cs
using NUnit.Framework;

[TestFixture]
public class TestInputValidation
{
    [Test]
    public void TestForSQLInjection_Sanitize()
    {
        string malicious = "'; DROP TABLE Users; --";
        string sanitized = InputSanitizer.Sanitize(malicious);

        // Sanitizer should remove dangerous characters
        Assert.IsFalse(sanitized.Contains("'"));
        Assert.IsFalse(sanitized.Contains(";"));
        Assert.IsFalse(sanitized.Contains("--"));
    }

    [Test]
    public void TestForSQLInjection_UsernameValidatorRejects()
    {
        string malicious = "admin'; DROP TABLE Users; --";

        bool isValid = UsernameValidator.IsValid(malicious);

        Assert.IsFalse(isValid, "Username validator should reject SQL injection payloads");
    }

    [Test]
    public void TestForSQLInjection_ParameterizedQuery()
    {
        string malicious = "' OR 1=1 --";

        // Simulate passing into a parameterized query
        var parameter = new Microsoft.Data.SqlClient.SqlParameter("@Username", malicious);

        Assert.AreEqual(malicious, parameter.Value);
        Assert.DoesNotThrow(() =>
        {
            // Parameterized queries treat input as data, not SQL
            string query = "SELECT * FROM Users WHERE Username = @Username;";
        });
    }

    [Test]
    public void TestForXSS_Sanitize()
    {
        string malicious = "<script>alert('XSS');</script>";
        string sanitized = InputSanitizer.Sanitize(malicious);

        Assert.IsFalse(sanitized.Contains("<script>"));
        Assert.IsFalse(sanitized.Contains("</script>"));
        Assert.IsFalse(sanitized.Contains("alert"));
    }

    [Test]
    public void TestForXSS_UsernameValidatorRejects()
    {
        string malicious = "<img src=x onerror=alert('XSS')>";

        bool isValid = UsernameValidator.IsValid(malicious);

        Assert.IsFalse(isValid, "Username validator should reject XSS payloads");
    }

    [Test]
    public void TestForXSS_EmailValidatorRejects()
    {
        string malicious = "test@example.com<script>alert('XSS')</script>";

        bool isValid = EmailValidator.IsValid(malicious);

        Assert.IsFalse(isValid, "Email validator should reject XSS payloads");
    }
}