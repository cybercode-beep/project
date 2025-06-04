using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CyberPasswordManager.Models
{
    /// <summary>
    /// Implements the Singleton design pattern to ensure only one instance of the credential vault exists.
    /// This class manages the secure storage and retrieval of user credentials.
    /// 
    /// Design Pattern: Singleton
    /// - Ensures only one instance of the vault exists throughout the application
    /// - Controls access to the shared resource (credentials)
    /// - Provides a global point of access via Instance property
    /// </summary>
    public class CredentialVault
    {
        private static CredentialVault? _instance;
        private static readonly object _lock = new object();
        private readonly List<Credential> _credentials = new();
        private const string VAULT_FILE = "vault.txt";
        private string _masterPassword = string.Empty;

        /// <summary>
        /// Private constructor to prevent direct instantiation.
        /// Part of the Singleton pattern implementation.
        /// </summary>
        private CredentialVault()
        {
            LoadCredentials();
        }

        /// <summary>
        /// Gets the single instance of the CredentialVault.
        /// Implements double-check locking for thread safety.
        /// </summary>
        public static CredentialVault Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new CredentialVault();
                    }
                }
                return _instance;
            }
        }

        public bool ValidateMasterPassword(string password)
        {
            // If no master password is set, this is first time setup
            if (string.IsNullOrEmpty(_masterPassword))
            {
                if (string.IsNullOrEmpty(password))
                {
                    return false; // Don't allow empty master password
                }
                _masterPassword = password;
                SaveCredentials();
                return true;
            }

            // Otherwise, validate against existing master password
            return _masterPassword == password;
        }

        public void AddCredential(string website, string username, string password)
        {
            _credentials.Add(new Credential(website, username, password));
            SaveCredentials();
        }

        public void RemoveCredential(int index)
        {
            if (index >= 0 && index < _credentials.Count)
            {
                _credentials.RemoveAt(index);
                SaveCredentials();
            }
        }

        public List<Credential> GetCredentials()
        {
            return _credentials;
        }

        private void LoadCredentials()
        {
            if (File.Exists(VAULT_FILE))
            {
                try
                {
                    var json = File.ReadAllText(VAULT_FILE);
                    var data = JsonSerializer.Deserialize<VaultData>(json);
                    if (data != null)
                    {
                        if (data.Credentials != null)
                        {
                            _credentials.Clear();
                            _credentials.AddRange(data.Credentials);
                        }
                        _masterPassword = data.MasterPassword ?? string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading credentials: {ex.Message}");
                }
            }
        }

        private void SaveCredentials()
        {
            try
            {
                var data = new VaultData
                {
                    MasterPassword = _masterPassword,
                    Credentials = _credentials
                };
                var json = JsonSerializer.Serialize(data);
                File.WriteAllText(VAULT_FILE, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving credentials: {ex.Message}");
            }
        }
    }

    public class Credential
    {
        public string Website { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsPasswordVisible { get; set; }

        public Credential() { }

        public Credential(string website, string username, string password)
        {
            Website = website;
            Username = username;
            Password = password;
            IsPasswordVisible = false;
        }

        public string GetDisplayPassword()
        {
            return IsPasswordVisible ? Password : new string('*', Password.Length);
        }

        public void TogglePasswordVisibility()
        {
            IsPasswordVisible = !IsPasswordVisible;
        }
    }

    public class VaultData
    {
        public string? MasterPassword { get; set; }
        public List<Credential>? Credentials { get; set; }
    }
}
