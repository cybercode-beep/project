using SplashKitSDK;

namespace CyberPasswordManager.UI.Controls
{
    public class Checkbox
    {
        public string Label { get; }
        private bool _isChecked;
        private readonly double _x;
        private readonly double _y;
        private readonly double _size;

        public event Action<bool>? CheckedChanged;

        public Checkbox(string label, double x, double y, double size = 20)
        {
            Label = label;
            _x = x;
            _y = y;
            _size = size;
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    CheckedChanged?.Invoke(_isChecked);
                }
            }
        }        public void Draw()
        {
            // Draw checkbox background
            SplashKit.FillRectangle(Color.White, _x, _y, _size, _size);
            SplashKit.DrawRectangle(Color.Black, _x, _y, _size, _size);
            
            // Draw checkmark if checked
            if (_isChecked)
            {
                SplashKit.DrawLine(Color.Black, _x + 2, _y + _size / 2, _x + _size / 2, _y + _size - 2);
                SplashKit.DrawLine(Color.Black, _x + _size / 2, _y + _size - 2, _x + _size - 2, _y + 2);
            }
            
            // Draw label with better spacing
            SplashKit.DrawText(Label, Color.Black, _x + _size + 10, _y + (_size - 15) / 2);
        }

        public void Update()
        {
            var mousePos = SplashKit.MousePosition();
            if (SplashKit.MouseClicked(MouseButton.LeftButton) &&
                mousePos.X >= _x && mousePos.X <= _x + _size &&
                mousePos.Y >= _y && mousePos.Y <= _y + _size)
            {
                IsChecked = !IsChecked;
            }
        }
    }
}