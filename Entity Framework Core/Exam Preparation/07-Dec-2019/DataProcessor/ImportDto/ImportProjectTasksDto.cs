using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Task")]
    public class ImportProjectTasksDto
    {

        [MinLength(2)]
        [MaxLength(40)]
        [Required]
        [XmlElement("Name")]
        public string Name { get; set; }

        [Required]
        [XmlElement("OpenDate")]
        public string OpenDate { get; set; }

        [Required]
        [XmlElement("DueDate")]
        public string DueDate { get; set; }

        [Range(0, 3)]
        [Required]
        [XmlElement("ExecutionType")]
        public int ExecutionType { get; set; }

        [Range(0, 4)]
        [Required]
        [XmlElement("LabelType")]
        public int LabelType { get; set; }
    }
}
