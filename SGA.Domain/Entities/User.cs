using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SGA.Domain.Entities
{
    public class User
    {
        public int Id { get; private set; }
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }
        public string Email { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLogin { get; private set; }        public int FailedLoginAttempts { get; private set; }
        public DateTime? LockoutEnd { get; private set; }
        public int UserTypeId { get; private set; }
        public int? TeacherId { get; private set; }
        public UserType UserType { get; private set; }
        public Teacher Teacher { get; private set; }

        // For EF Core
        protected User() { }

        public User(string username, string passwordHash, string email, int userTypeId)
        {
            ValidateUsername(username);
            ValidateEmail(email);

            Username = username;
            PasswordHash = passwordHash;
            Email = email;
            UserTypeId = userTypeId;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            FailedLoginAttempts = 0;
        }

        public void IncrementFailedLoginAttempts()
        {
            FailedLoginAttempts++;
            
            // Apply lockout after 3 failed attempts (as per requirements)
            if (FailedLoginAttempts >= 3)
            {
                LockoutEnd = DateTime.UtcNow.AddMinutes(15); // 15-minute lockout
            }
        }

        public void ResetFailedLoginAttempts()
        {
            FailedLoginAttempts = 0;
            LockoutEnd = null;
        }

        public void UpdateLastLogin()
        {
            LastLogin = DateTime.UtcNow;
            ResetFailedLoginAttempts();
        }

        public bool IsLockedOut()
        {
            return LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;
        }        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
            {
                throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));
            }

            PasswordHash = newPasswordHash;
        }

        public void AssociateWithTeacher(int teacherId)
        {
            TeacherId = teacherId;
        }

        public void RemoveTeacherAssociation()
        {
            TeacherId = null;
        }

        private void ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be empty", nameof(username));
            }

            if (username.Length < 3 || username.Length > 50)
            {
                throw new ArgumentException("Username must be between 3 and 50 characters", nameof(username));
            }
        }

        private void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be empty", nameof(email));
            }

            // Basic email validation
            if (!new EmailAddressAttribute().IsValid(email))
            {
                throw new ArgumentException("Invalid email format", nameof(email));
            }
        }
    }
}
