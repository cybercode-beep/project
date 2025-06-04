using System;
using SplashKitSDK;
using CyberPasswordManager.Models;
using CyberPasswordManager.UI;

namespace CyberPasswordManager
{
    /// <summary>
    /// Main entry point for the Cyber Password Manager application.
    /// 
    /// This application demonstrates several key features:
    /// 1. Secure password management with a master password
    /// 2. Password generation with customizable options
    /// 3. Password strength checking with visual feedback
    /// 4. Cybersecurity quiz game for educational purposes
    /// 
    /// Design Patterns Used:
    /// - Singleton Pattern (CredentialVault): Ensures single instance of credential storage
    /// - Strategy Pattern (PasswordManager): Allows different password generation algorithms
    /// - State Pattern (UIManager): Manages different application screens and states
    /// 
    /// The application follows SOLID principles:
    /// - Single Responsibility: Each class has a focused purpose
    /// - Open/Closed: New password generation strategies can be added without modification
    /// - Liskov Substitution: Password generation strategies are interchangeable
    /// - Interface Segregation: Clean interfaces for password generation
    /// - Dependency Inversion: High-level modules depend on abstractions
    /// </summary>
    public class Program
    {
        // Constants for window dimensions
        private const int WINDOW_WIDTH = 800;
        private const int WINDOW_HEIGHT = 600;
        private const string WINDOW_TITLE = "Cyber Password Manager";

        public static void Main()
        {
            // Initialize window
            var window = new Window(WINDOW_TITLE, WINDOW_WIDTH, WINDOW_HEIGHT);
            var uiManager = new UIManager(window);

            // Main game loop
            while (!window.CloseRequested)
            {
                // Process user input
                SplashKit.ProcessEvents();

                // Update game state
                uiManager.Update();

                // Clear screen and draw
                window.Clear(Color.White);
                uiManager.Draw();
                window.Refresh();
            }

            // Cleanup
            if (SplashKit.ReadingText())
            {
                SplashKit.EndReadingText();
            }
            window.Close();
        }
    }
}
