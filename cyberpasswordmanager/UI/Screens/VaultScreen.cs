using SplashKitSDK;
using CyberPasswordManager.UI.Base;
using CyberPasswordManager.UI.Controls;
using System.Linq;

namespace CyberPasswordManager.UI.Screens
{
    public class VaultScreen : BaseScreen
    {
        private readonly TextInput _websiteInput;
        private readonly TextInput _usernameInput;
        private readonly TextInput _passwordInput;
        private int _selectedCredentialIndex = -1;

        public event Action? OnGeneratePasswordRequest;

        public VaultScreen(Window window) : base(window)
        {
            _websiteInput = new TextInput(120, 90, 300);
            _usernameInput = new TextInput(120, 120, 300);
            _passwordInput = new TextInput(120, 150, 300, true);
        }

        public override void Update()
        {
            _websiteInput.Update();
            _usernameInput.Update();
            _passwordInput.Update();

            // Handle credential deletion
            if (SplashKit.KeyTyped(KeyCode.DeleteKey) && _selectedCredentialIndex >= 0)
            {
                Vault.RemoveCredential(_selectedCredentialIndex);
                _selectedCredentialIndex = -1;
            }

            // Handle Tab navigation
            if (SplashKit.KeyTyped(KeyCode.TabKey))
            {
                if (!_websiteInput.IsFocused && !_usernameInput.IsFocused && !_passwordInput.IsFocused)
                {
                    SwitchFocusTo(_websiteInput);
                }
                else if (_websiteInput.IsFocused)
                {
                    SwitchFocusTo(_usernameInput);
                }
                else if (_usernameInput.IsFocused)
                {
                    SwitchFocusTo(_passwordInput);
                }
                else if (_passwordInput.IsFocused)
                {
                    SwitchFocusTo(_websiteInput);
                }
            }

            // Handle Enter key for quick add
            if (SplashKit.KeyTyped(KeyCode.ReturnKey) && 
                !string.IsNullOrEmpty(_websiteInput.Text) && 
                !string.IsNullOrEmpty(_usernameInput.Text) && 
                !string.IsNullOrEmpty(_passwordInput.Text))
            {
                AddNewCredential();
            }
        }

        public override void Draw(Window window)
        {
            DrawText("Password Vault", 20, 20);

            // Draw add new credential section
            DrawText("Add New Credential", 40, 60);
            
            // Input fields
            DrawText("Website:", 40, 90);
            _websiteInput.Draw();
            
            DrawText("Username:", 40, 120);
            _usernameInput.Draw();
            
            DrawText("Password:", 40, 150);
            _passwordInput.Draw();

            // Generate password button
            DrawButton("Generate Password", 440, 150, () => OnGeneratePasswordRequest?.Invoke());

            // Add button
            if (!string.IsNullOrEmpty(_websiteInput.Text) && 
                !string.IsNullOrEmpty(_usernameInput.Text) && 
                !string.IsNullOrEmpty(_passwordInput.Text))
            {
                DrawButton("Add", 40, 180, () => 
                {
                    Vault.AddCredential(_websiteInput.Text, _usernameInput.Text, _passwordInput.Text);
                    _websiteInput.Text = "";
                    _usernameInput.Text = "";
                    _passwordInput.Text = "";
                    SwitchFocusTo(_websiteInput);
                });
            }

            // Draw credentials list
            var credentials = Vault.GetCredentials();
            if (credentials.Any())
            {
                DrawText("Stored Credentials", 40, 240);
                for (int i = 0; i < credentials.Count; i++)
                {
                    var y = 280 + (i * 60);
                    var cred = credentials[i];
                    
                    // Highlight selected credential
                    if (i == _selectedCredentialIndex)
                    {
                        SplashKit.FillRectangle(Color.LightGray, 40, y - 5, window.Width - 80, 50);
                    }

                    DrawText($"Website: {cred.Website}", 60, y);
                    DrawText($"Username: {cred.Username}", 60, y + 20);
                    
                    // Password with visibility toggle
                    string passwordText = cred.GetDisplayPassword();
                    DrawText($"Password: {passwordText}", 60, y + 40);
                    
                    // Toggle password visibility button
                    DrawButton(cred.IsPasswordVisible ? "Hide" : "Show", 300, y + 40, () => 
                    {
                        cred.TogglePasswordVisibility();
                    });

                    // Handle credential selection
                    var mousePos = SplashKit.MousePosition();
                    if (SplashKit.MouseClicked(MouseButton.LeftButton) &&
                        mousePos.X >= 40 && mousePos.X <= window.Width - 40 &&
                        mousePos.Y >= y - 5 && mousePos.Y <= y + 45)
                    {
                        _selectedCredentialIndex = i;
                    }
                }

                // Draw delete instruction if credential is selected
                if (_selectedCredentialIndex >= 0)
                {
                    DrawText("Press Delete to remove selected credential", 40, window.Height - 60);
                }
            }
        }

        private void AddNewCredential()
        {
            Vault.AddCredential(_websiteInput.Text, _usernameInput.Text, _passwordInput.Text);
            _websiteInput.Text = "";
            _usernameInput.Text = "";
            _passwordInput.Text = "";
            SwitchFocusTo(_websiteInput);
        }

        private void SwitchFocusTo(TextInput input)
        {
            _websiteInput.EndInput();
            _usernameInput.EndInput();
            _passwordInput.EndInput();
            input.IsFocused = true;
        }

        public void SetGeneratedPassword(string password)
        {
            _passwordInput.Text = password;
            SwitchFocusTo(_passwordInput);
        }
    }
}