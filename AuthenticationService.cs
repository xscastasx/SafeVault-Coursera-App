using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace SecureAppCopilot.AuthenticationService
{
    public class AuthenticationService
    {
        private readonly string _connectionString;
        private readonly string _jwtKey;

        public AuthenticationService(string connectionString, string? jwtKey = null)
        {
            _connectionString = connectionString;
            _jwtKey = jwtKey ?? "replace-with-secure-key";
        }

        public Task RegisterAsync(string username, string password, string role = "User")
            => RegisterUserAsync(username, password, role);

        public Task<User?> LoginAsync(string username, string password)
            => LoginUserAsync(username, password);

        public async Task RegisterUserAsync(string username, string password, string role = "User")
        {
            string passwordHash = PasswordHasher.HashPassword(password);

            const string query = @"
                INSERT INTO Users (Username, PasswordHash, Role)
                VALUES (@Username, @PasswordHash, @Role);
            ";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);
            command.Parameters.AddWithValue("@Role", role);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<User?> LoginUserAsync(string username, string password)
        {
            const string query = @"
                SELECT Id, Username, PasswordHash, Role
                FROM Users
                WHERE Username = @Username;
            ";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Username", username);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (!reader.Read())
                return null;

            var user = new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                Role = reader.GetString(3)
            };

            if (!PasswordHasher.VerifyPassword(password, user.PasswordHash))
                return null;

            return user;
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
