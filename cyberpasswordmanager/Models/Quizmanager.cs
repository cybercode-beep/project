using System;
using System.Collections.Generic;

namespace CyberPasswordManager.Models
{
    public class QuizManager
    {
        private List<QuizQuestion> questions;
        private int currentQuestionIndex;
        private int correctAnswers;
        
        public int CurrentQuestionIndex => currentQuestionIndex;
        public int TotalQuestions => questions?.Count ?? 0;
        public int CorrectAnswers => correctAnswers;
        
        public QuizManager()
        {
            questions = new List<QuizQuestion>();
            LoadQuestions();
            ResetQuiz();
        }
        
        private void LoadQuestions()
        {
            questions.Add(QuizQuestionFactory.CreateQuestion("password",
                "What makes a password strong?",
                new string[] { "A) Length only", "B) Complexity only", 
                              "C) Length and complexity", "D) Common words" },
                2, "Strong passwords need both length (12+ characters) and complexity (mixed characters)."));
                
            questions.Add(QuizQuestionFactory.CreateQuestion("phishing",
                "What is a common sign of a phishing email?",
                new string[] { "A) Perfect grammar", "B) Urgent action required", 
                              "C) Company logo", "D) Long message" },
                1, "Phishing emails often create urgency to make you act without thinking."));
                
            questions.Add(QuizQuestionFactory.CreateQuestion("security",
                "What is two-factor authentication?",
                new string[] { "A) Two passwords", "B) Password + something else", 
                              "C) Two usernames", "D) Two websites" },
                1, "2FA adds an extra layer beyond just a password, like a phone code."));
                
            questions.Add(QuizQuestionFactory.CreateQuestion("password",
                "How often should you change important passwords?",
                new string[] { "A) Never", "B) Every day", 
                              "C) When compromised", "D) Every hour" },
                2, "Change passwords when there's evidence of compromise or breach."));
                
            questions.Add(QuizQuestionFactory.CreateQuestion("security",
                "What is the purpose of encryption?",
                new string[] { "A) Speed up data", "B) Protect data from unauthorized access", 
                              "C) Delete data", "D) Copy data" },
                1, "Encryption scrambles data so only authorized parties can read it."));
        }
        
        public QuizQuestion? GetCurrentQuestion()
        {
            if (currentQuestionIndex < questions.Count)
                return questions[currentQuestionIndex];
            return null;
        }
        
        public bool AnswerQuestion(int selectedAnswer)
        {
            if (currentQuestionIndex >= questions.Count)
                return false;
                
            bool isCorrect = questions[currentQuestionIndex].Options[selectedAnswer] == questions[currentQuestionIndex].CorrectAnswer;
            if (isCorrect)
                correctAnswers++;
                
            return isCorrect;
        }

        public void MoveToNextQuestion()
        {
            if (currentQuestionIndex < questions.Count)
                currentQuestionIndex++;
        }
        
        public bool IsQuizComplete()
        {
            return currentQuestionIndex >= questions.Count;
        }
        
        public void ResetQuiz()
        {
            currentQuestionIndex = 0;
            correctAnswers = 0;
        }
        
        public double GetScorePercentage()
        {
            if (questions.Count == 0) return 0;
            return (double)correctAnswers / questions.Count * 100;
        }
    }
}