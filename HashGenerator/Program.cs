// Generate BCrypt hash for password
using BCrypt.Net;

Console.WriteLine("Generating BCrypt hash for 'Admin123!'");
string password = "Admin123!";
string hash = BCrypt.Net.BCrypt.HashPassword(password);
Console.WriteLine($"Password: {password}");
Console.WriteLine($"Hash: {hash}");

// Verify the hash
bool isValid = BCrypt.Net.BCrypt.Verify(password, hash);
Console.WriteLine($"Verification: {isValid}");
