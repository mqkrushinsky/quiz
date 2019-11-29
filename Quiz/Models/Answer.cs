using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Quiz.Models
{
    public class Answer
    {
        public int QuestionId { get; set; }
        
        public int AnswerId { get; set; }

        [MaxLength(5000)]
        [Required]
        public string AnswerText { get; set; }

        public bool CaseSensitive { get; set; }
    }
}
