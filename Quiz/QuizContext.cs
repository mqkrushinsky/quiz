using Microsoft.EntityFrameworkCore;
using Quiz.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quiz
{
    public class QuizContext : DbContext
    {
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Question> Questions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(@"Server=localhost;database=quiz;user=root;password=test");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Answer>().HasKey(c => new { c.QuestionId, c.AnswerId });
        }
    }
}
