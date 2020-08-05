using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Xml.Serialization;
using Castle.Components.DictionaryAdapter;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Project")]
    public class ImportProjectsDto
    {
        [MinLength(2)]
        [MaxLength(40)]
        [Required]
        [XmlElement("Name")]
        public string Name { get; set; }

        [Required]
        [XmlElement("OpenDate")]
        public string OpenDate { get; set; }

        [XmlElement("DueDate")]
        public string DueDate { get; set; }

        [XmlArray("Tasks")]
        public ImportProjectTasksDto[] Tasks { get; set; }
    }
}
