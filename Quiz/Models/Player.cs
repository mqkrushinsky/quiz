using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Quiz.Models
{
    public class Player
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlayerId { get; set; }

        [MinLength(3)]
        [MaxLength(3)]
        [Required]
        public string Initials { get; set; }

        public int HighScore { get; set; }

        [NotMapped]
        public bool Returning { get; set; }

        [NotMapped]
        public int CurrentScore { get; set; } = 0;
    }
}
