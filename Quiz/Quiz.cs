using Quiz.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quiz
{
    public interface Quiz
    {
        Question GetNextQuestion();
        bool CheckAnswer(Question question, string answer);
        string GetAnswers(Question question);
    }
}
