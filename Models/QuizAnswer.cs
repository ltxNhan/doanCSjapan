using System.ComponentModel.DataAnnotations;

namespace JapanApp.Models
{
    public class QuizAnswer
    {
        [Key]
        public int AnswerID { get; set; }

        public int QuestionID { get; set; }

        public QuizQuestion? Question { get; set; }

        public string AnswerText { get; set; } = string.Empty;

        public int Points { get; set; }
    }
}