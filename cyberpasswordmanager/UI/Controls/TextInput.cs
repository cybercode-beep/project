using SplashKitSDK;

namespace CyberPasswordManager.UI.Controls
{
    public class TextInput
    {
        private const int HEIGHT = 25;
        private const int CURSOR_BLINK_MS = 500;

        private string _text = string.Empty;
        public string Text 
        { 
            get => _text;
            set 
            {
                _text = value;
                if (IsFocused && SplashKit.ReadingText())
                {
                    SplashKit.EndReadingText();
                    SplashKit.StartReadingText(_inputRect, _text);
                }
            }
        }
        public bool IsFocused { get; set; }
        public bool IsPassword { get; }

        private readonly double _x;
        private readonly double _y;
        private readonly double _width;
        private DateTime _lastCursorBlink = DateTime.Now;
        private bool _showCursor = true;
        private static TextInput? _activeInput;
        private readonly Rectangle _inputRect;

        public TextInput(double x, double y, double width, bool isPassword = false)
        {
            _x = x;
            _y = y;
            _width = width;
            IsPassword = isPassword;
            _inputRect = new Rectangle { X = x, Y = y, Width = width, Height = HEIGHT };
        }

        public void Update()
        {
            // Update cursor blink state
            if ((DateTime.Now - _lastCursorBlink).TotalMilliseconds >= CURSOR_BLINK_MS)
            {
                _showCursor = !_showCursor;
                _lastCursorBlink = DateTime.Now;
            }

            // Handle input focus
            var mousePos = SplashKit.MousePosition();
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                bool isMouseOver = mousePos.X >= _x && mousePos.X <= _x + _width &&
                                 mousePos.Y >= _y && mousePos.Y <= _y + HEIGHT;

                if (isMouseOver)
                {
                    if (!IsFocused || _activeInput != this)
                    {
                        _activeInput?.EndInput();
                        _activeInput = this;
                        IsFocused = true;
                        SplashKit.StartReadingText(_inputRect, Text);
                    }
                }
                else if (_activeInput == this)
                {
                    EndInput();
                }
            }

            // Handle text input
            if (IsFocused && _activeInput == this)
            {
                if (!SplashKit.ReadingText())
                {
                    SplashKit.StartReadingText(_inputRect, Text);
                }
                else
                {
                    string newText = SplashKit.TextInput();
                    if (newText != Text)
                    {
                        _text = newText; // Update text directly to avoid recursion
                    }
                }
            }
        }

        public void Draw()
        {
            // Draw box
            var boxColor = IsFocused ? Color.LightGray : Color.White;
            SplashKit.FillRectangle(boxColor, _x, _y, _width, HEIGHT);
            SplashKit.DrawRectangle(Color.Black, _x, _y, _width, HEIGHT);

            // Draw text or password dots
            string displayText = IsPassword ? new string('•', Text.Length) : Text;
            if (!string.IsNullOrEmpty(displayText))
            {
                SplashKit.DrawText(displayText, Color.Black, _x + 5, _y + 5);
            }

            // Draw cursor
            if (IsFocused && _showCursor)
            {
                float textWidth = SplashKit.TextWidth(displayText, "Arial", 12);
                SplashKit.DrawLine(Color.Black, _x + 5 + textWidth, _y + 3, _x + 5 + textWidth, _y + HEIGHT - 3);
            }
        }

        public void EndInput()
        {
            if (IsFocused && SplashKit.ReadingText())
            {
                SplashKit.EndReadingText();
            }
            if (_activeInput == this)
            {
                _activeInput = null;
            }
            IsFocused = false;
        }
    }
}