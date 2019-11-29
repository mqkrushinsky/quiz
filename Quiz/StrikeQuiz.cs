using Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quiz
{
    public class StrikeQuiz : Quiz
    {
        private QuizContext _context { get; set; }
        private Category QuizCategory { get; set; }

        private List<Question> Questions { get; set; }
        private List<Answer> Answers { get; set; }

        public int Strikes;

        public StrikeQuiz(QuizContext context, int strikes, Category category)
        {
            _context = context;
            QuizCategory = category;
            Questions = _context.Questions.Where(q => q.CategoryId == QuizCategory.CategoryId).ToList();
            Answers = _context.Answers.Where(a => Questions.Select(q => q.QuestionId).Contains(a.QuestionId)).ToList();
            Strikes = strikes;
        }

        public Question GetNextQuestion()
        {
            Question returnQuestion = null;

            if (Questions.Any() && Strikes > 0)
            {
                int index = 0;

                while (returnQuestion == null)
                {
                    if (Randomizer.IsSelected(1.0 / (double)Questions.Count))
                    {
                        returnQuestion = Questions.ElementAt(index);
                        Questions.RemoveAt(index);
                    }

                    index++;

                    if (index >= Questions.Count)
                    {
                        index = 0;
                    }
                }
            }

            return returnQuestion;
        }

        public bool CheckAnswer(Question question, string answer)
        {
            bool isCorrect = false;
            List<string> answers = Answers.Where(a => a.QuestionId == question.QuestionId).Select(b => b.AnswerText).ToList();

            if (answers.Any(a => a.IndexOf(answer, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                isCorrect = true;
            }
            else
            {
                Strikes--;
                Console.WriteLine($"{Strikes} strikes left.");
            }

            return isCorrect;
        }

        public string GetAnswers(Question question)
        {
            return string.Join(",", Answers.Where(a => a.QuestionId == question.QuestionId).Select(b => b.AnswerText));
        }
    }
}
