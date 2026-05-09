using System.ComponentModel.DataAnnotations;

namespace JapanApp.Models
{
    public class QuizQuestion
    {
        [Key]
        public int QuestionID { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public List<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();
    }
}