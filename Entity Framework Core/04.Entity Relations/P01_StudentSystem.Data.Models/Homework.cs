using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace P01_StudentSystem.Data.Models
{
    public class Homework
    {
        public int HomeworkId { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public ContentType ContentType { get; set; }

        [Required]
        public DateTime SubmissionTime { get; set; }

        [Required]
        public int StudentId { get; set; }
        public virtual Student Student { get; set; }

        [Required]
        public virtual Course Course { get; set; }
        public int CourseId { get; set; }
    }
}
