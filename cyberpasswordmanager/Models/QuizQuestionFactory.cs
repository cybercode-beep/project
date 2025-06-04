using System;

namespace CyberPasswordManager.Models
{
    public static class QuizQuestionFactory
    {
        public static QuizQuestion CreateQuestion(string category, string question, string[] options, int correctAnswerIndex, string explanation)
        {
            return new QuizQuestion
            {
                Category = category,
                Question = question,
                Options = options,
                CorrectAnswer = options[correctAnswerIndex],
                Explanation = explanation
            };
        }
    }
}
