using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P01_StudentSystem.Data.Models
{
    public class Course
    {
        public Course()
        {
            this.StudentCourses = new HashSet<StudentCourse>();
            this.StudentsEnrolled = new HashSet<StudentCourse>();
            this.Resources = new HashSet<Resource>();
            this.HomeworkSubmissions = new HashSet<Homework>();
        }
        public int CourseId { get; set; }
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public decimal Price { get; set; }

        public virtual ICollection<StudentCourse> StudentCourses { get; set; }
        public virtual ICollection<StudentCourse> StudentsEnrolled { get; set; }
        public virtual ICollection<Resource> Resources { get; set; }
        public virtual ICollection<Homework> HomeworkSubmissions { get; set; }
    }
}
