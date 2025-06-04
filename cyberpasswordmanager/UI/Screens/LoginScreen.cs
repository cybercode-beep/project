using SplashKitSDK;
using CyberPasswordManager.UI.Base;
using CyberPasswordManager.UI.Controls;

namespace CyberPasswordManager.UI.Screens
{
    public class LoginScreen : BaseScreen
    {
        private readonly TextInput _passwordInput;
        private bool _isLoggedIn;

        public event Action? OnLoginSuccess;

        public LoginScreen(Window window) : base(window)
        {
            _passwordInput = new TextInput(window.Width / 2 - 100, 240, 200, true);
        }

        public override void OnEnter()
        {
            _isLoggedIn = false;
            _passwordInput.Text = "";
            _passwordInput.IsFocused = true;
        }

        public override void OnExit()
        {
            _passwordInput.EndInput();
        }

        public override void Update()
        {
            if (!_isLoggedIn)
            {
                _passwordInput.Update();

                // Handle Return key for login
                if (SplashKit.KeyTyped(KeyCode.ReturnKey) && !string.IsNullOrEmpty(_passwordInput.Text))
                {
                    TryLogin();
                }
            }
        }

        public override void Draw(Window window)
        {
            // Center align the UI elements
            float centerX = window.Width / 2;
            
            // Draw title
            DrawText("Cyber Password Manager", centerX - 100, 100);
            DrawText("Enter Master Password:", centerX - 80, 200);
            
            // Draw password input field
            _passwordInput.Draw();
            
            // Draw login button if password is entered
            if (!string.IsNullOrEmpty(_passwordInput.Text))
            {
                DrawButton("Login", centerX - 40, 280, TryLogin);
            }
        }

        private void TryLogin()
        {
            if (Vault.ValidateMasterPassword(_passwordInput.Text))
            {
                _isLoggedIn = true;
                _passwordInput.EndInput();
                OnLoginSuccess?.Invoke();
            }
            else
            {
                _passwordInput.Text = "";
                _passwordInput.IsFocused = true;
            }
        }
    }
}