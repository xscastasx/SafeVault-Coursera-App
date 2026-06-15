using Isopoh.Cryptography.Argon2;
using System.Text;
using System.Security.Cryptography;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        // Generate a random salt
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        var config = new Argon2Config
        {
            Type = Argon2Type.DataDependentAddressing,
            TimeCost = 4,
            MemoryCost = 1024 * 64, // 64 MB
            Lanes = 4,
            Threads = 4,
            Password = Encoding.UTF8.GetBytes(password),
            Salt = salt
        };

        // Correct: returns encoded Argon2 hash string
        return Argon2.Hash(config);
    }

    public static bool VerifyPassword(string password, string hash)
    {
        return Argon2.Verify(hash, password);
    }
}
