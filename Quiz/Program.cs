using MySql.Data.MySqlClient;
using Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quiz
{
    class Program
    {
        private static QuizContext context = null;
        private static Player player = null;

        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            context = new QuizContext();

            Console.WriteLine("Quiz");
            Console.WriteLine("----");
            Console.WriteLine();

            Console.WriteLine("Enter initials for record keeping:");

            var initials = Console.ReadLine();

            Player player = InitiatePlayer(initials);

            if (player.Returning)
            {
                Console.WriteLine("Welcome back!");
            }
            else
            {
                Console.WriteLine("Good luck!");
            }

            LoadMenu();
        }

        public static Player InitiatePlayer(string initials)
        {
            initials = initials.ToUpper();

            player = context.Players.FirstOrDefault(p => p.Initials == initials);

            if (player != null)
            {
                player.Returning = true;
            }
            else
            {
                player = new Player()
                {
                    Initials = initials,
                    Returning = false
                };

                context.Players.Add(player);

                context.SaveChanges();
            }

            return player;
        }

        public static void LoadMenu()
        {
            //Reset player's score in case they came from a previous quiz
            player.CurrentScore = 0;

            Console.WriteLine();
            Console.WriteLine("Main Menu");
            Console.WriteLine("----");
            Console.WriteLine("1. 10-Question Quiz");
            Console.WriteLine("2. Three-Strikes Quiz");
            Console.WriteLine("3. One-Strike Quiz");
            Console.WriteLine("4. Add New Question");
            Console.WriteLine("5. Add New Category");
            Console.WriteLine("6. Exit");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RunQuiz("TenQuestion");
                    break;

                case "2":
                    RunQuiz("Strike", 3);
                    break;

                case "3":
                    RunQuiz("Strike", 1);
                    break;

                case "4":
                    PromptForAddQuestion();
                    break;

                case "5":
                    PromptForAddCategory();
                    break;

                case "6":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Invalid selection.");
                    LoadMenu();
                    break;
            }
        }

        public static void RunQuiz(string quizType, int? numStrikes = null)
        {
            var categoryChosen = PromptForCategoryChoice();
            Quiz quiz = null;

            if (quizType == "TenQuestion")
            {
                quiz = new TenQuestionQuiz(context, categoryChosen);
            }
            else if (quizType == "Strike")
            {
                quiz = new StrikeQuiz(context, numStrikes.Value, categoryChosen);
            }

            if (quiz != null)
            {
                Question nextQuestion = quiz.GetNextQuestion();

                while (nextQuestion != null)
                {
                    Console.WriteLine(nextQuestion.QuestionText);

                    var answer = Console.ReadLine();

                    Console.WriteLine();

                    if (quiz.CheckAnswer(nextQuestion, answer))
                    {
                        Console.WriteLine("Correct!");
                        player.CurrentScore += 1;
                    }
                    else
                    {
                        Console.WriteLine($"Incorrect. Correct answer(s): {quiz.GetAnswers(nextQuestion)}");
                    }

                    Console.WriteLine();
                    nextQuestion = quiz.GetNextQuestion();
                }

                Console.WriteLine();
                Console.WriteLine($"Quiz Complete. Correct Answers: {player.CurrentScore}");

                if (player.CurrentScore > player.HighScore)
                {
                    Console.WriteLine("New high score!!");
                    player.HighScore = player.CurrentScore;
                    context.SaveChanges();
                }
            }

            LoadMenu();
        }

        public static void PromptForAddQuestion()
        {
            var categoryChosen = PromptForCategoryChoice();

            int categoryId = 1;

            if (categoryChosen != null)
            {
                categoryId = categoryChosen.CategoryId;
            }

            Console.WriteLine("Enter question text:");

            var questionText = Console.ReadLine();

            Console.WriteLine("Enter answers:");

            List<string> answerTexts = new List<string>();
            var answerText = Console.ReadLine();

            while (answerText != "")
            {
                answerTexts.Add(answerText);
                answerText = Console.ReadLine();
            }

            Question newQuestion = new Question()
            {
                QuestionText = questionText,
                CategoryId = categoryId
            };

            context.Add(newQuestion);
            context.SaveChanges();

            int answerId = 1;
            List<Answer> newAnswers = new List<Answer>();

            foreach (var answer in answerTexts)
            {
                newAnswers.Add(new Answer()
                {
                    QuestionId = newQuestion.QuestionId,
                    AnswerId = answerId,
                    AnswerText = answer
                });

                answerId++;
            }

            context.AddRange(newAnswers);
            context.SaveChanges();

            Console.WriteLine("Question and answers successfully added.");

            LoadMenu();
        }

        public static void PromptForAddCategory()
        {
            Console.WriteLine("Enter category name:");

            var categoryName = Console.ReadLine();

            if (categoryName != null && !context.Categories.Any(c => c.CategoryName.ToUpper().Equals(categoryName.ToUpper())))
            {
                Category newCategory = new Category()
                {
                    CategoryName = categoryName
                };

                context.Add(newCategory);
                context.SaveChanges();

                Console.WriteLine("Category successfully added.");
            }
            else
            {
                Console.WriteLine("Invalid category name.");
            }

            LoadMenu();
        }

        public static Category PromptForCategoryChoice()
        {
            Console.WriteLine("Choose category:");

            foreach (Category category in context.Categories)
            {
                Console.WriteLine($"   - {category.CategoryName}");
            }

            var categoryChoice = Console.ReadLine();

            return context.Categories.FirstOrDefault(c => c.CategoryName.ToUpper() == categoryChoice.ToUpper());
        }
    }
}
