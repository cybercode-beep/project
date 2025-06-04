using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CyberPasswordManager.Models
{
    /// <summary>
    /// Manages the cybersecurity quiz game functionality.
    /// This class demonstrates the use of the Single Responsibility Principle by handling only quiz-related operations.
    /// </summary>
    public class GameManager
    {
        private List<QuizQuestion> _questions = new();
        private int _currentQuestionIndex;
        private int _score;
        private const string QUESTIONS_FILE = "questions.json";

        /// <summary>
        /// Initializes a new instance of the GameManager class.
        /// Loads questions from file and prepares the game.
        /// </summary>
        public GameManager()
        {
            LoadQuestions();
            ResetGame();
        }

        /// <summary>
        /// Resets the game state, shuffling questions and clearing the score.
        /// </summary>
        public void ResetGame()
        {
            _currentQuestionIndex = 0;
            _score = 0;
            ShuffleQuestions();
        }

        /// <summary>
        /// Gets the current question in the quiz.
        /// </summary>
        /// <returns>The current QuizQuestion, or null if the game is over.</returns>
        public QuizQuestion? GetCurrentQuestion()
        {
            if (_currentQuestionIndex < _questions.Count)
            {
                return _questions[_currentQuestionIndex];
            }
            return null;
        }

        /// <summary>
        /// Processes a user's answer to the current question.
        /// </summary>
        /// <param name="answer">The user's answer</param>
        /// <returns>True if the answer was correct, false otherwise</returns>
        public bool AnswerQuestion(string answer)
        {
            var currentQuestion = GetCurrentQuestion();
            if (currentQuestion != null)
            {
                bool isCorrect = currentQuestion.CorrectAnswer.Equals(answer, StringComparison.OrdinalIgnoreCase);
                if (isCorrect) _score++;
                _currentQuestionIndex++;
                return isCorrect;
            }
            return false;
        }

        /// <summary>
        /// Gets the current score of the player.
        /// </summary>
        /// <returns>The current score.</returns>
        public int GetScore() => _score;
        
        /// <summary>
        /// Gets the total number of questions in the quiz.
        /// </summary>
        /// <returns>The total number of questions.</returns>
        public int GetTotalQuestions() => _questions.Count;
        
        /// <summary>
        /// Checks if the game is over.
        /// </summary>
        /// <returns>True if the game is over, false otherwise.</returns>
        public bool IsGameOver() => _currentQuestionIndex >= _questions.Count;

        /// <summary>
        /// Gets the index of the current question.
        /// </summary>
        /// <returns>The index of the current question.</returns>
        public int GetCurrentQuestionIndex() => _currentQuestionIndex;

        private void LoadQuestions()
        {
            _questions = new List<QuizQuestion>();
              _questions = new List<QuizQuestion>();
            
            if (File.Exists(QUESTIONS_FILE))
            {
                try
                {
                    var json = File.ReadAllText(QUESTIONS_FILE);
                    var loadedQuestions = JsonSerializer.Deserialize<List<QuizQuestion>>(json);
                    if (loadedQuestions != null)
                    {
                        _questions = loadedQuestions;
                    }
                    else
                    {
                        CreateDefaultQuestions();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading questions: {ex.Message}");
                    CreateDefaultQuestions();
                }
            }
            else
            {
                CreateDefaultQuestions();
                SaveQuestions();
            }
        }

        private void CreateDefaultQuestions()
        {
            _questions = new List<QuizQuestion>
            {
                new QuizQuestion(
                    "What is a strong password characteristic?",
                    new[] {"Short and simple", "Contains only numbers", "Mix of letters, numbers, and symbols", "Same as username"},
                    "Mix of letters, numbers, and symbols"
                ),
                new QuizQuestion(
                    "Which password is strongest?",
                    new[] {"password123", "P@ssw0rd!", "MyP@ssw0rd2023!", "qwerty"},
                    "MyP@ssw0rd2023!"
                ),
                new QuizQuestion(
                    "How often should you change passwords?",
                    new[] {"Never", "Every year", "Every 3-6 months", "Every day"},
                    "Every 3-6 months"
                ),
                new QuizQuestion(
                    "What should you avoid in passwords?",
                    new[] {"Special characters", "Personal information", "Random numbers", "Capital letters"},
                    "Personal information"
                ),
                new QuizQuestion(
                    "What is two-factor authentication?",
                    new[] {
                        "Using two different passwords",
                        "Using a password and a security question",
                        "Using a password and a second verification method",
                        "Using two different usernames"
                    },
                    "Using a password and a second verification method"
                )
            };
        }

        private void SaveQuestions()
        {
            try
            {
                var json = JsonSerializer.Serialize(_questions);
                File.WriteAllText(QUESTIONS_FILE, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving questions: {ex.Message}");
            }
        }

        private void ShuffleQuestions()
        {
            Random rng = new Random();
            int n = _questions.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var temp = _questions[k];
                _questions[k] = _questions[n];
                _questions[n] = temp;
            }
        }
    }    public class QuizQuestion
    {
        public string Category { get; set; } = "";
        public string Question { get; set; } = "";
        public string[] Options { get; set; } = Array.Empty<string>();
        public string CorrectAnswer { get; set; } = "";
        public string Explanation { get; set; } = "";

        public QuizQuestion() { }

        public QuizQuestion(string question, string[] options, string correctAnswer)
        {
            Question = question;
            Options = options;
            CorrectAnswer = correctAnswer;
        }
    }
}
