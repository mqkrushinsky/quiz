using Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quiz
{
    public class TenQuestionQuiz : Quiz
    {
        private QuizContext _context { get; set; }

        private Category QuizCategory { get; set; }
        private List<Question> Questions { get; set; }
        private List<Answer> Answers { get; set; }

        private int CurrentQuestionIndex { get; set; }

        public TenQuestionQuiz(QuizContext context, Category category)
        {
            _context = context;
            QuizCategory = category;
            Questions = new List<Question>();

            double questionsToChoose = 10;
            double totalQuestions = _context.Questions.Count();
            CurrentQuestionIndex = 1;

            while (Questions.Count < 10)
            {
                Question currentQuestion = _context.Questions.Where(c => c.CategoryId == QuizCategory.CategoryId).FirstOrDefault(q => q.QuestionId == CurrentQuestionIndex);

                if (currentQuestion == null)
                {
                    if (CurrentQuestionIndex > _context.Questions.Max(q => q.QuestionId))
                    {
                        CurrentQuestionIndex = 0;
                    }
                }
                else
                {
                    if (!Questions.Contains(currentQuestion))
                    {
                        if (Randomizer.IsSelected(questionsToChoose / totalQuestions))
                        {
                            Questions.Add(currentQuestion);
                            questionsToChoose -= 1;
                        }
                    }
                }

                CurrentQuestionIndex++;
            }

            List<int> chosenQuestionIds = Questions.Select(q => q.QuestionId).ToList();

            Answers = _context.Answers.Where(a => chosenQuestionIds.Contains(a.QuestionId)).ToList();

            CurrentQuestionIndex = 0;
        }

        public Question GetNextQuestion()
        {
            Question returnQuestion = null;

            if (CurrentQuestionIndex < 10)
            {
                returnQuestion = Questions[CurrentQuestionIndex];

                CurrentQuestionIndex++;
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

            return isCorrect;
        }

        public string GetAnswers(Question question)
        {
            return string.Join(",", Answers.Where(a => a.QuestionId == question.QuestionId).Select(b => b.AnswerText));
        }
    }
}
