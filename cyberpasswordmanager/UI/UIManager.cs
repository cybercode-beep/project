using System;
using SplashKitSDK;
using CyberPasswordManager.Models;
using CyberPasswordManager.UI.Screens;
using CyberPasswordManager.UI.Base;

namespace CyberPasswordManager.UI
{
    /// <summary>
    /// Defines the different screens/states of the application.
    /// </summary>
    public enum Screen
    {
        Login,
        Vault,
        Generator,
        Quiz
    }

    /// <summary>
    /// Manages the user interface and integrates all components of the application.
    /// 
    /// This class demonstrates:
    /// 1. Component Integration - Combines Vault, Generator, and Quiz functionality
    /// 2. State Management - Handles UI state and screen transitions
    /// 3. Event Handling - Processes user input and updates the display
    /// </summary>
    public class UIManager
    {        private readonly Window _window;
        private Screen _currentScreen = Screen.Login;
        private bool _isLoggedIn;
        private IScreen? _activeScreen;

        private readonly LoginScreen _loginScreen;
        private readonly VaultScreen _vaultScreen;
        private readonly GeneratorScreen _generatorScreen;
        private readonly QuizScreen _quizScreen;

        public UIManager(Window window)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));

            // Initialize screens
            _loginScreen = new LoginScreen(window);
            _vaultScreen = new VaultScreen(window);
            _generatorScreen = new GeneratorScreen(window);
            _quizScreen = new QuizScreen(window);

            // Wire up event handlers
            _loginScreen.OnLoginSuccess += () =>
            {
                _isLoggedIn = true;
                SwitchScreen(Screen.Vault);
            };

            _vaultScreen.OnGeneratePasswordRequest += () => SwitchScreen(Screen.Generator);

            _generatorScreen.OnPasswordGenerated += password =>
            {
                _vaultScreen.SetGeneratedPassword(password);
                SwitchScreen(Screen.Vault);
            };

            // Set initial screen
            SwitchScreen(Screen.Login);
        }

        public void Update()
        {
            // Handle global keyboard shortcuts for navigation
            if (_isLoggedIn)
            {
                if (SplashKit.KeyTyped(KeyCode.VKey))
                {
                    SwitchScreen(Screen.Vault);
                }
                else if (SplashKit.KeyTyped(KeyCode.GKey))
                {
                    SwitchScreen(Screen.Generator);
                }
                else if (SplashKit.KeyTyped(KeyCode.QKey))
                {
                    SwitchScreen(Screen.Quiz);
                }
                else if (SplashKit.KeyTyped(KeyCode.EscapeKey) && _currentScreen != Screen.Vault)
                {
                    SwitchScreen(Screen.Vault);
                }
            }

            // Update active screen
            _activeScreen?.Update();
        }

        public void Draw()
        {
            // Draw navigation only if logged in
            if (_isLoggedIn)
            {
                DrawNavigation();
            }

            // Draw active screen
            _activeScreen?.Draw(_window);
        }

        private void DrawNavigation()
        {
            double y = _window.Height - 40;
            SplashKit.DrawText("Navigation: [V]ault | [G]enerator | [Q]uiz | [Esc] Back", Color.Black, 20, y);
        }

        private void SwitchScreen(Screen screen)
        {
            // Don't switch to non-login screens if not logged in
            if (!_isLoggedIn && screen != Screen.Login)
            {
                return;
            }

            _activeScreen?.OnExit();
            _currentScreen = screen;

            _activeScreen = screen switch
            {
                Screen.Login => _loginScreen,
                Screen.Vault => _vaultScreen,
                Screen.Generator => _generatorScreen,
                Screen.Quiz => _quizScreen,
                _ => throw new ArgumentException($"Unknown screen: {screen}")
            };

            _activeScreen.OnEnter();
        }
    }
}
