using SplashKitSDK;
using CyberPasswordManager.UI.Base;
using CyberPasswordManager.UI.Controls;

namespace CyberPasswordManager.UI.Screens
{
    public class GeneratorScreen : BaseScreen
    {
        private readonly Checkbox _includeUppercase;
        private readonly Checkbox _includeNumbers;
        private readonly Checkbox _includeSymbols;
        private readonly TextInput _strengthCheckInput;
        private int _passwordLength = 12;
        private string _generatedPassword = string.Empty;
        private (int Score, string Feedback) _passwordStrength;

        public event Action<string>? OnPasswordGenerated;

        public GeneratorScreen(Window window) : base(window)
        {
            _includeUppercase = new Checkbox("Include Uppercase", 40, 60);
            _includeNumbers = new Checkbox("Include Numbers", 40, 90);
            _includeSymbols = new Checkbox("Include Symbols", 40, 120);
            _strengthCheckInput = new TextInput(40, 400, 300);

            _includeUppercase.IsChecked = true;
            _includeNumbers.IsChecked = true;
            _includeSymbols.IsChecked = true;
        }

        public override void Update()
        {
            _includeUppercase.Update();
            _includeNumbers.Update();
            _includeSymbols.Update();
            _strengthCheckInput.Update();

            if (SplashKit.KeyTyped(KeyCode.SpaceKey))
            {
                GeneratePassword();
            }

            // Update strength check for entered password
            if (!string.IsNullOrEmpty(_strengthCheckInput.Text))
            {
                _passwordStrength = PasswordManager.CheckPasswordStrength(_strengthCheckInput.Text);
            }
        }

        public override void Draw(Window window)
        {
            // Draw title
            DrawText("Password Generator", 20, 20);

            // Draw options
            _includeUppercase.Draw();
            _includeNumbers.Draw();
            _includeSymbols.Draw();

            // Draw length selector
            DrawText($"Password Length: {_passwordLength}", 40, 160);
            DrawButton("-", 200, 160, () => _passwordLength = Math.Max(8, _passwordLength - 1));
            DrawButton("+", 240, 160, () => _passwordLength = Math.Min(32, _passwordLength + 1));

            // Draw generate button
            DrawButton("Generate", 40, 200, GeneratePassword);

            // Draw generated password field
            DrawText("Generated Password:", 40, 250);
            DrawText(_generatedPassword, 40, 280);
            
            // Draw "Use Password" button when password is generated
            if (!string.IsNullOrEmpty(_generatedPassword))
            {
                DrawButton("Use Password", 40, 320, () => OnPasswordGenerated?.Invoke(_generatedPassword));
            }

            // Draw strength checker section
            DrawText("Password Strength Checker", 40, 370);
            _strengthCheckInput.Draw();

            // Draw strength result
            if (_passwordStrength.Score > 0)
            {
                DrawPasswordStrength(_passwordStrength.Score, 40, 440);
                DrawText(_passwordStrength.Feedback, 40, 470);
            }
        }

        private void GeneratePassword()
        {
            _generatedPassword = PasswordManager.GeneratePassword(
                _passwordLength,
                _includeNumbers.IsChecked,
                _includeSymbols.IsChecked,
                _includeUppercase.IsChecked
            );
        }
    }
}