using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace P01_StudentSystem.Data.Models
{
    public class StudentCourse
    {
        [Required]
        public int StudentId { get; set; }
        public virtual Student Student { get; set; }

        [Required]
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
    }
}
