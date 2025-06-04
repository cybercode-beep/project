using SplashKitSDK;
using CyberPasswordManager.Models;

namespace CyberPasswordManager.UI.Base
{
    /// <summary>
    /// Base screen implementation that provides common functionality for all screens.
    /// </summary>
    public abstract class BaseScreen : IScreen
    {
        protected static readonly Color BACKGROUND_COLOR = Color.White;
        protected static readonly Color TEXT_COLOR = Color.Black;
        protected static readonly Color BUTTON_COLOR = Color.RGBColor(0, 122, 204);
        protected static readonly Color BUTTON_HOVER_COLOR = Color.RGBColor(0, 102, 184);
        protected static readonly Color SUCCESS_COLOR = Color.Green;
        protected static readonly Color ERROR_COLOR = Color.Red;

        protected readonly Window Window;
        protected readonly CredentialVault Vault;
        protected readonly GameManager GameManager;
        protected readonly PasswordManager PasswordManager;

        protected BaseScreen(Window window)
        {
            Window = window ?? throw new ArgumentNullException(nameof(window));
            Vault = CredentialVault.Instance;
            GameManager = new GameManager();
            PasswordManager = new PasswordManager();
        }

        public abstract void Update();
        public abstract void Draw(Window window);
        
        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual bool HandleKeyPress(KeyCode key) => false;

        protected void DrawText(string text, double x, double y)
        {
            SplashKit.DrawText(text, TEXT_COLOR, x, y);
        }

        protected void DrawButton(string text, double x, double y, Action onClick)
        {
            var width = 120.0;
            var height = 30.0;
            var mousePos = SplashKit.MousePosition();
            var isHovered = mousePos.X >= x && mousePos.X <= x + width &&
                          mousePos.Y >= y && mousePos.Y <= y + height;

            var color = isHovered ? BUTTON_HOVER_COLOR : BUTTON_COLOR;
            SplashKit.FillRectangle(color, x, y, width, height);
            SplashKit.DrawText(text, Color.White, x + 10, y + 5);

            if (isHovered && SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                onClick?.Invoke();
            }
        }

        protected void DrawProgressBar(double x, double y, float progress)
        {
            var width = 300.0;
            var height = 20.0;
            
            // Draw background
            SplashKit.FillRectangle(Color.LightGray, x, y, width, height);
            
            // Draw progress
            SplashKit.FillRectangle(BUTTON_COLOR, x, y, width * progress, height);
            
            // Draw border
            SplashKit.DrawRectangle(Color.Black, x, y, width, height);
        }

        protected void DrawPasswordStrength(int score, double x, double y)
        {
            var width = 200.0;
            var height = 20.0;

            // Draw background
            SplashKit.FillRectangle(Color.LightGray, x, y, width, height);

            // Draw strength bar
            var strengthColor = score switch
            {
                >= 80 => Color.Green,
                >= 60 => Color.Yellow,
                _ => Color.Red
            };

            SplashKit.FillRectangle(strengthColor, x, y, width * score / 100, height);
            SplashKit.DrawRectangle(Color.Black, x, y, width, height);
        }
    }
}