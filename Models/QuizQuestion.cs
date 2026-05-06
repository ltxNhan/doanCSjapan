using System.ComponentModel.DataAnnotations;

namespace JapanApp.Models
{
    public class QuizQuestion
    {
        [Key] // ⭐ BẮT BUỘC
        public int QuestionID { get; set; }

        public string QuestionText { get; set; }

        public List<QuizAnswer> Answers { get; set; }
    }
}