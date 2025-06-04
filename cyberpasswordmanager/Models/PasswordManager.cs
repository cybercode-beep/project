using System;
using System.Text;
using System.Linq;

namespace CyberPasswordManager.Models
{
    /// <summary>
    /// Interface defining the contract for password generation strategies.
    /// Part of the Strategy design pattern implementation.
    /// </summary>
    public interface IPasswordGenerationStrategy
    {
        string GeneratePassword(int length, bool includeNumbers, bool includeSymbols, bool includeUppercase);
    }

    /// <summary>
    /// Default implementation of the password generation strategy.
    /// Demonstrates the Strategy pattern by encapsulating the password generation algorithm.
    /// </summary>
    public class DefaultPasswordGenerator : IPasswordGenerationStrategy
    {
        private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Numbers = "0123456789";
        private const string Symbols = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        public string GeneratePassword(int length, bool includeNumbers, bool includeSymbols, bool includeUppercase)
        {
            var charSet = new StringBuilder(Lowercase);
            if (includeUppercase) charSet.Append(Uppercase);
            if (includeNumbers) charSet.Append(Numbers);
            if (includeSymbols) charSet.Append(Symbols);

            var random = new Random();
            var password = new StringBuilder();

            // Ensure at least one character from each selected type
            if (includeUppercase) password.Append(Uppercase[random.Next(Uppercase.Length)]);
            if (includeNumbers) password.Append(Numbers[random.Next(Numbers.Length)]);
            if (includeSymbols) password.Append(Symbols[random.Next(Symbols.Length)]);

            // Fill remaining length with random characters
            while (password.Length < length)
            {
                password.Append(charSet[random.Next(charSet.Length)]);
            }

            // Shuffle the password
            return new string(password.ToString().ToCharArray().OrderBy(x => random.Next()).ToArray());
        }
    }

    /// <summary>
    /// Manages password generation and strength checking functionality.
    /// 
    /// Design Pattern: Strategy
    /// - Uses IPasswordGenerationStrategy to define different password generation algorithms
    /// - Allows switching generation strategies at runtime
    /// - Encapsulates algorithm implementation details from the client code
    /// </summary>
    public class PasswordManager
    {
        private IPasswordGenerationStrategy _generationStrategy;

        public PasswordManager()
        {
            _generationStrategy = new DefaultPasswordGenerator();
        }

        /// <summary>
        /// Changes the password generation strategy at runtime.
        /// Demonstrates the flexibility of the Strategy pattern.
        /// </summary>
        public void SetGenerationStrategy(IPasswordGenerationStrategy strategy)
        {
            _generationStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

        /// <summary>
        /// Generates a password using the current strategy.
        /// </summary>
        public string GeneratePassword(int length = 12, bool includeNumbers = true, bool includeSymbols = true, bool includeUppercase = true)
        {
            return _generationStrategy.GeneratePassword(length, includeNumbers, includeSymbols, includeUppercase);
        }

        /// <summary>
        /// Checks the strength of a password and provides detailed feedback.
        /// Returns a tuple containing the strength score (0-100) and feedback message.
        /// </summary>
        public (int Score, string Feedback) CheckPasswordStrength(string password)
        {
            int score = 0;
            var feedback = new StringBuilder();

            if (string.IsNullOrEmpty(password))
                return (0, "Password cannot be empty");

            // Length check
            if (password.Length < 8)
            {
                feedback.AppendLine("Password is too short");
            }
            else if (password.Length >= 12)
            {
                score += 2;
                feedback.AppendLine("Good length");
            }
            else
            {
                score += 1;
            }

            // Complexity checks
            if (password.Any(char.IsUpper))
            {
                score += 1;
                feedback.AppendLine("Contains uppercase letters");
            }
            if (password.Any(char.IsLower))
            {
                score += 1;
                feedback.AppendLine("Contains lowercase letters");
            }
            if (password.Any(char.IsDigit))
            {
                score += 1;
                feedback.AppendLine("Contains numbers");
            }
            if (password.Any(c => !char.IsLetterOrDigit(c)))
            {
                score += 1;
                feedback.AppendLine("Contains symbols");
            }

            // Normalize score to 0-100
            int normalizedScore = Math.Min((score * 100) / 6, 100);

            string strengthFeedback = normalizedScore switch
            {
                >= 80 => "Strong password",
                >= 60 => "Moderate password",
                _ => "Weak password"
            };

            feedback.Insert(0, $"{strengthFeedback}\n");

            return (normalizedScore, feedback.ToString());
        }
    }
}
