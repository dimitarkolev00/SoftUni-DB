using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using TeisterMask.Data.Models;
using TeisterMask.DataProcessor.ExportDto;

namespace TeisterMask.DataProcessor
{
    using System;
    using Data;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            StringBuilder sb = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportProjectDto[]),
                new XmlRootAttribute("Projects"));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            using (StringWriter stringWriter = new StringWriter(sb))
            {
                var projects = context
                    .Projects
                    .ToArray()
                    .Where(p => p.Tasks.Count >= 1)
                    .Select(p => new ExportProjectDto()
                    {
                        Name = p.Name,
                        TaskCount = p.Tasks.Count,
                        HasEndDate = p.DueDate.HasValue ? "Yes" : "No",
                        Tasks = p.Tasks
                            .ToArray()
                            .OrderBy(t => t.Name)
                            .Select(t => new ExportProjectTaskDto()
                            {
                                Name = t.Name,
                                LabelType = t.LabelType.ToString()
                            })
                            
                            .ToArray()
                    })
                    .ToArray()
                    .OrderByDescending(p => p.TaskCount)
                    .ThenBy(p => p.Name)
                    .ToArray();

                xmlSerializer.Serialize(stringWriter,projects,namespaces);
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees = context
                .Employees
                .ToArray()
                .Where(e => e.EmployeesTasks.Any(et => et.Task.OpenDate >= date))
                .Select(e => new
                {
                    Username = e.Username,
                    Tasks = e.EmployeesTasks
                        .Where(et => et.Task.OpenDate >= date)
                        .ToArray()
                        .OrderByDescending(et => et.Task.DueDate)
                        .ThenBy(et => et.Task.Name)
                        .Select(et => new
                        {
                            TaskName = et.Task.Name,
                            OpenDate = et.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                            DueDate = et.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                            LabelType = et.Task.LabelType.ToString(),
                            ExecutionType = et.Task.ExecutionType.ToString()
                        })
                        .ToArray()
                })
                .ToArray()
                .OrderByDescending(e => e.Tasks.Length)
                .ThenBy(e => e.Username)
                .Take(10)
                .ToArray();

            string jsn = JsonConvert.SerializeObject(employees, Formatting.Indented);

            return jsn;
        }

    }
}