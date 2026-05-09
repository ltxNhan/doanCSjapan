using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JapanApp.Models
{
    public class QuizResult
    {
        [Key]
        public int QuizResultID { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public int TotalScore { get; set; }

        public int SuggestedSeasonID { get; set; }
        public Season SuggestedSeason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
