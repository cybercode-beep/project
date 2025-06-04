using SplashKitSDK;

namespace CyberPasswordManager.UI.Base
{
    /// <summary>
    /// Base interface for all screens in the application.
    /// Each screen should implement this interface to provide consistent behavior.
    /// </summary>
    public interface IScreen
    {
        /// <summary>
        /// Updates the screen's state based on user input and time
        /// </summary>
        void Update();

        /// <summary>
        /// Draws the screen's content
        /// </summary>
        /// <param name="window">The window to draw on</param>
        void Draw(Window window);

        /// <summary>
        /// Called when this screen becomes active
        /// </summary>
        void OnEnter();

        /// <summary>
        /// Called when this screen is no longer active
        /// </summary>
        void OnExit();

        /// <summary>
        /// Handles keyboard input specific to this screen
        /// </summary>
        /// <param name="key">The key that was pressed</param>
        /// <returns>True if the key was handled, false otherwise</returns>
        bool HandleKeyPress(KeyCode key);
    }
}