using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TeisterMask.DataProcessor.ImportDto
{
    public class ImportEmployeesDto
    {
        [MinLength(3)]
        [MaxLength(40)]
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [RegularExpression(@"^(\d{3})\-(\d{3})\-(\d{4})$")]
        [Required]
        public string Phone { get; set; }

        public int[] Tasks { get; set; }
    }
}
