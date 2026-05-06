using System.ComponentModel.DataAnnotations;

namespace JapanApp.Models
{
    public class QuizAnswer
    {
        [Key] // 👈 thêm cái này
        public int AnswerID { get; set; }

        public int QuestionID { get; set; }
        public QuizQuestion Question { get; set; }

        public string AnswerText { get; set; }

        public int SuggestSeasonID { get; set; }
        public Season Season { get; set; }
    }
}