using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public class UserService
{
    private readonly ConcurrentDictionary<string, string> _users = new();

    // Password validation function with security rules
    private bool ValidatePassword(string password)
    {
        if (password.Length < 8)
        {
            return false;
        }

        if (!Regex.IsMatch(password, @"[A-Z]"))
        {
            return false;
        }

        if (!Regex.IsMatch(password, @"[a-z]"))
        {
            return false;
        }

        if (!Regex.IsMatch(password, @"[0-9]"))
        {
            return false;
        }

        if (!Regex.IsMatch(password, @"[\W_]"))
        {
            return false;
        }

        return true;
    }

    public string RegisterUser(string email, string password, string confirmPassword)
    {
        Console.WriteLine($"Email: {email}");
        Console.WriteLine($"Password: {password}");
        Console.WriteLine($"ConfirmPassword: {confirmPassword}");
        
        if (_users.ContainsKey(email))
        {
            return "User already exists!";
        }

        if (password != confirmPassword) {
            return "Password does not match!";
        }

        if (!ValidatePassword(password))
        {
            return "Password does not meet security requirements. It must have at least 8 characters, contain uppercase and lowercase letters, numbers, and special characters.";
        }

        var hashedPassword = HashPassword(password);
        _users[email] = hashedPassword;
        return "User successfully registered!";
    }

    public bool ValidateUser(string email, string password)
    {
        if (_users.TryGetValue(email, out var storedHashedPassword))
        {
            return VerifyPassword(password, storedHashedPassword);
        }
        return false;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private bool VerifyPassword(string enteredPassword, string storedHash)
    {
        var enteredHash = HashPassword(enteredPassword);
        return enteredHash == storedHash;
    }
}
