using System;
using SplashKitSDK;
using CyberPasswordManager.UI.Base;
using CyberPasswordManager.Models;

namespace CyberPasswordManager.UI.Screens
{
    public class QuizScreen : BaseScreen
    {
        private int selectedQuizAnswer = -1;
        private bool showQuizResult = false;
        private bool quizAnswered = false;
        private string quizResultMessage = "";
        private QuizManager quizManager;
        private const int FONT_SIZE = 16;
        private const int OPTION_HEIGHT = 30;
        private const int OPTION_PADDING = 10;
        private const int MARGIN = 20;
        private const int BUTTON_WIDTH = 120;
        private const int BUTTON_HEIGHT = 30;

        public QuizScreen(Window window) : base(window)
        {
            quizManager = new QuizManager();
        }

        public override void OnEnter()
        {
            selectedQuizAnswer = -1;
            showQuizResult = false;
            quizAnswered = false;
            if (quizManager.IsQuizComplete())
            {
                quizManager.ResetQuiz();
            }
        }

        private void DrawWrappedText(string text, Color color, double x, double y, double maxWidth)
        {
            string[] words = text.Split(' ');
            string currentLine = "";
            double currentY = y;

            foreach (string word in words)
            {
                string testLine = currentLine + (currentLine.Length > 0 ? " " : "") + word;
                if (SplashKit.TextWidth(testLine, "Arial", FONT_SIZE) > maxWidth)
                {
                    SplashKit.DrawTextOnWindow(Window, currentLine, color, x, currentY);
                    currentLine = word;
                    currentY += FONT_SIZE + 4; // Smaller line height
                }
                else
                {
                    currentLine = testLine;
                }
            }
            
            if (currentLine.Length > 0)
            {
                SplashKit.DrawTextOnWindow(Window, currentLine, color, x, currentY);
            }
        }

        public override void Draw(Window window)
        {
            // Clear the window with dark blue background
            SplashKit.ClearWindow(window, Color.DarkBlue);

            // Title and back button
            SplashKit.DrawTextOnWindow(window, "Cybersecurity Quiz", Color.White, MARGIN, MARGIN);
              Rectangle backButton = new Rectangle { X = window.Width - BUTTON_WIDTH - MARGIN, Y = MARGIN, Width = BUTTON_WIDTH - 20, Height = BUTTON_HEIGHT };
            SplashKit.FillRectangleOnWindow(window, Color.Gray, backButton);
            SplashKit.DrawTextOnWindow(window, "Back", Color.White, backButton.X + 10, backButton.Y + 8);
            
            // Quiz progress - only show when quiz is not complete
            if (!quizManager.IsQuizComplete())
            {
                SplashKit.DrawTextOnWindow(window, 
                    $"Question {quizManager.CurrentQuestionIndex + 1} of {quizManager.TotalQuestions}", 
                    Color.White, MARGIN, MARGIN + 40);
            }
            
            QuizQuestion? currentQuestion = quizManager.GetCurrentQuestion();
            
            if (currentQuestion != null && !quizManager.IsQuizComplete())
            {
                // Display question
                SplashKit.DrawTextOnWindow(window, "Question:", Color.White, MARGIN, MARGIN + 60);
                
                // Wrap text for long questions
                if (currentQuestion.Question != null)
                {
                    DrawWrappedText(currentQuestion.Question, Color.White, MARGIN, MARGIN + 80, window.Width - 2 * MARGIN);
                }
                
                // Display options
                int yPos = MARGIN + 140;
                if (currentQuestion.Options != null)
                {
                    for (int i = 0; i < currentQuestion.Options.Length; i++)
                    {
                        Rectangle optionRect = new Rectangle { 
                            X = MARGIN, 
                            Y = yPos, 
                            Width = window.Width - 2 * MARGIN, 
                            Height = OPTION_HEIGHT 
                        };
                        
                        Color bgColor = Color.LightGray;
                        if (selectedQuizAnswer == i)
                            bgColor = Color.LightBlue;
                            
                        SplashKit.FillRectangleOnWindow(window, bgColor, optionRect);
                        SplashKit.DrawRectangleOnWindow(window, Color.Black, optionRect);
                        
                        if (currentQuestion.Options[i] != null)
                        {
                            SplashKit.DrawTextOnWindow(window, currentQuestion.Options[i], Color.Black, 
                                optionRect.X + OPTION_PADDING, optionRect.Y + 8);
                        }
                        
                        yPos += OPTION_HEIGHT + 5; // Reduced spacing between options
                    }
                }
                
                // Submit answer button
                Rectangle submitButton = new Rectangle { 
                    X = MARGIN, 
                    Y = yPos + 10, 
                    Width = BUTTON_WIDTH, 
                    Height = BUTTON_HEIGHT 
                };
                Color submitColor = (selectedQuizAnswer >= 0) ? Color.Green : Color.Gray;
                SplashKit.FillRectangleOnWindow(window, submitColor, submitButton);
                SplashKit.DrawTextOnWindow(window, "Submit Answer", Color.White, submitButton.X + 10, submitButton.Y + 8);
                
                // Show result if answered
                if (showQuizResult)
                {
                    SplashKit.DrawTextOnWindow(window, quizResultMessage, Color.Yellow, submitButton.X + submitButton.Width + 20, submitButton.Y + 8);
                    SplashKit.DrawTextOnWindow(window, "Explanation:", Color.White, MARGIN, submitButton.Y + BUTTON_HEIGHT + 20);
                    if (currentQuestion.Explanation != null)
                    {
                        DrawWrappedText(currentQuestion.Explanation, Color.LightGray, MARGIN, 
                            submitButton.Y + BUTTON_HEIGHT + 40, window.Width - 2 * MARGIN);
                    }
                    
                    // Next question button
                    Rectangle nextButton = new Rectangle { 
                        X = MARGIN, 
                        Y = submitButton.Y + BUTTON_HEIGHT + 80, 
                        Width = BUTTON_WIDTH, 
                        Height = BUTTON_HEIGHT 
                    };
                    SplashKit.FillRectangleOnWindow(window, Color.Blue, nextButton);
                    SplashKit.DrawTextOnWindow(window, "Next Question", Color.White, nextButton.X + 10, nextButton.Y + 8);
                }
            }
            else if (quizManager.IsQuizComplete())
            {
                double scorePercentage = quizManager.GetScorePercentage();
                int centerX = (int)(window.Width / 2);
                int centerY = (int)(window.Height / 2);
                
                // Show final results centered
                SplashKit.DrawTextOnWindow(window, "Quiz Complete!", Color.White, centerX - 100, centerY - 100);
                SplashKit.DrawTextOnWindow(window, 
                    $"Final Score: {quizManager.CorrectAnswers}/{quizManager.TotalQuestions}", 
                    Color.White, centerX - 100, centerY - 70);
                SplashKit.DrawTextOnWindow(window, 
                    $"Percentage: {scorePercentage:F1}%", Color.White, centerX - 100, centerY - 40);
                
                // Performance message
                string performanceMsg;
                Color performanceColor;
                
                if (scorePercentage >= 80)
                {
                    performanceMsg = "Excellent! You have strong cybersecurity knowledge.";
                    performanceColor = Color.Green;
                }
                else if (scorePercentage >= 60)
                {
                    performanceMsg = "Good job! Consider reviewing some cybersecurity basics.";
                    performanceColor = Color.Orange;
                }
                else
                {
                    performanceMsg = "Keep learning! Cybersecurity knowledge is important.";
                    performanceColor = Color.Red;
                }
                
                SplashKit.DrawTextOnWindow(window, performanceMsg, performanceColor, centerX - 200, centerY);
                
                // Restart quiz button centered
                Rectangle restartButton = new Rectangle { 
                    X = centerX - BUTTON_WIDTH/2, 
                    Y = centerY + 50, 
                    Width = BUTTON_WIDTH, 
                    Height = BUTTON_HEIGHT 
                };
                SplashKit.FillRectangleOnWindow(window, Color.Blue, restartButton);
                SplashKit.DrawTextOnWindow(window, "Restart Quiz", Color.White, restartButton.X + 10, restartButton.Y + 8);
            }
        }

        public override void Update()
        {
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                Point2D mousePos = SplashKit.MousePosition();
                Rectangle backButton = new Rectangle { 
                    X = Window.Width - BUTTON_WIDTH - MARGIN, 
                    Y = MARGIN, 
                    Width = BUTTON_WIDTH - 20, 
                    Height = BUTTON_HEIGHT 
                };

                // Back button
                if (SplashKit.PointInRectangle(mousePos, backButton))
                {
                    OnExit();
                    return;
                }

                if (!quizManager.IsQuizComplete())
                {
                    var currentQuestion = quizManager.GetCurrentQuestion();
                    
                    // Option selection
                    if (!showQuizResult && currentQuestion?.Options != null)
                    {
                        int yPos = MARGIN + 140;
                        bool optionClicked = false;

                        // Check option clicks first
                        for (int i = 0; i < currentQuestion.Options.Length; i++)
                        {
                            Rectangle optionRect = new Rectangle { 
                                X = MARGIN, 
                                Y = yPos, 
                                Width = Window.Width - 2 * MARGIN, 
                                Height = OPTION_HEIGHT 
                            };
                            if (SplashKit.PointInRectangle(mousePos, optionRect))
                            {
                                selectedQuizAnswer = i;
                                optionClicked = true;
                                break;
                            }
                            yPos += OPTION_HEIGHT + 5;
                        }

                        // Only check submit button if no option was clicked
                        if (!optionClicked)
                        {
                            // Calculate submit button position using options count
                            int submitY = MARGIN + 140 + (currentQuestion.Options.Length * (OPTION_HEIGHT + 5)) + 10;
                            Rectangle submitButton = new Rectangle { 
                                X = MARGIN, 
                                Y = submitY, 
                                Width = BUTTON_WIDTH, 
                                Height = BUTTON_HEIGHT 
                            };
                            if (SplashKit.PointInRectangle(mousePos, submitButton) && selectedQuizAnswer >= 0)
                            {
                                bool isCorrect = quizManager.AnswerQuestion(selectedQuizAnswer);
                                quizResultMessage = isCorrect ? "Correct!" : "Incorrect!";
                                showQuizResult = true;
                                quizAnswered = true;
                            }
                        }
                    }
                    else if (showQuizResult && currentQuestion?.Options != null)
                    {
                        // Calculate next button position using options count
                        int submitY = MARGIN + 140 + (currentQuestion.Options.Length * (OPTION_HEIGHT + 5)) + 10;
                        int nextY = submitY + BUTTON_HEIGHT + 80;
                        
                        Rectangle nextButton = new Rectangle { 
                            X = MARGIN, 
                            Y = nextY, 
                            Width = BUTTON_WIDTH, 
                            Height = BUTTON_HEIGHT 
                        };
                        if (SplashKit.PointInRectangle(mousePos, nextButton))
                        {
                            selectedQuizAnswer = -1;
                            showQuizResult = false;
                            quizAnswered = false;
                            quizManager.MoveToNextQuestion();
                        }
                    }
                }
                else
                {
                    // Restart button in completed quiz screen - centered
                    int centerX = (int)(Window.Width / 2);
                    int centerY = (int)(Window.Height / 2);
                    Rectangle restartButton = new Rectangle { 
                        X = centerX - BUTTON_WIDTH/2, 
                        Y = centerY + 50, 
                        Width = BUTTON_WIDTH, 
                        Height = BUTTON_HEIGHT 
                    };
                    if (SplashKit.PointInRectangle(mousePos, restartButton))
                    {
                        quizManager.ResetQuiz();
                        selectedQuizAnswer = -1;
                        quizAnswered = false;
                        showQuizResult = false;
                    }
                }
            }
        }
    }
}