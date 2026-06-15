# SecureAppCopilot – Secure Authentication API

A minimal ASP.NET Core API implementing secure authentication using:

- Argon2 password hashing  
- JWT authentication with role-based authorization  
- Input sanitization and validation  
- Parameterized SQL queries (SQL Injection safe)  
- Secrets stored using `dotnet user-secrets` (no secrets in GitHub)

This project was built with a focus on secure coding practices.

---

## 🚀 Features

### 🔐 Authentication
- Register users with Argon2id password hashing
- Login with secure password verification
- JWT token generation
- Role-based authorization (`Admin`, `User`)

### 🛡️ Security
- No SQL injection (all queries parameterized)
- Input sanitization for usernames and emails
- XSS protection in sanitization layer
- Secrets stored outside the repository
- Strong JWT signing key

### 🗄️ Database
SQL Server table schema:

```sql
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Role NVARCHAR(50) NOT NULL DEFAULT 'User'
);
