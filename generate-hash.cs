using System;
using BCrypt.Net;

Console.WriteLine("Generating BCrypt hash for 'Admin123!'");
string password = "Admin123!";
string hash = BCrypt.HashPassword(password);
Console.WriteLine($"Password: {password}");
Console.WriteLine($"Hash: {hash}");

// Verify the hash
bool isValid = BCrypt.Verify(password, hash);
Console.WriteLine($"Verification: {isValid}");
