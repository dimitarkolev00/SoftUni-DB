using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using TeisterMask.Data.Models;
using TeisterMask.Data.Models.Enums;
using TeisterMask.DataProcessor.ImportDto;

namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    using Data;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportProjectsDto[]),
                new XmlRootAttribute("Projects"));

            List<Project> projectsToAdd = new List<Project>();

            using (StringReader stringReader = new StringReader(xmlString))
            {
                ImportProjectsDto[] projectsDto = (ImportProjectsDto[])xmlSerializer.Deserialize(stringReader);

                foreach (ImportProjectsDto projectDto in projectsDto)
                {
                    if (!IsValid(projectDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime projectOpenDate;
                    bool isProjectOpenDateValid = DateTime.TryParseExact(projectDto.OpenDate,
                        "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out projectOpenDate);

                    if (!isProjectOpenDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime? projectDueDate;
                    if (!String.IsNullOrEmpty(projectDto.DueDate))
                    {
                        DateTime projectDueDateValue;
                        bool isProjectDueDateValid = DateTime.TryParseExact(projectDto.DueDate,
                            "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out projectDueDateValue);

                        if (!isProjectDueDateValid)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }
                        projectDueDate = projectDueDateValue;
                    }
                    else
                    {
                        projectDueDate = null;
                    }

                    Project pr = new Project
                    {
                        Name = projectDto.Name,
                        OpenDate = projectOpenDate,
                        DueDate = projectDueDate,
                    };


                    foreach (ImportProjectTasksDto taskDto in projectDto.Tasks)
                    {
                        if (!IsValid(taskDto))
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        DateTime taskOpenDate;
                        bool isTaskOpenDateValid = DateTime.TryParseExact(taskDto.OpenDate, "dd/MM/yyyy",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out taskOpenDate);

                        if (!isTaskOpenDateValid)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        DateTime taskDueDate;
                        bool isTaskDueDateValid = DateTime.TryParseExact(taskDto.DueDate, "dd/MM/yyyy",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out taskDueDate);

                        if (!isTaskDueDateValid)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        if (taskOpenDate < projectOpenDate)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        if (projectDueDate.HasValue)
                        {
                            if (taskDueDate > projectDueDate.Value)
                            {
                                sb.AppendLine(ErrorMessage);
                                continue;
                            }
                        }

                        pr.Tasks.Add(new Task()
                        {
                            Name = taskDto.Name,
                            OpenDate = taskOpenDate,
                            DueDate = taskDueDate,
                            ExecutionType = (ExecutionType)taskDto.ExecutionType,
                            LabelType = (LabelType)taskDto.LabelType
                        });
                    }

                    projectsToAdd.Add(pr);
                    sb.AppendLine(String.Format(SuccessfullyImportedProject, pr.Name, pr.Tasks.Count));
                }

                context.Projects.AddRange(projectsToAdd);
                context.SaveChanges();
            }

            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var employeesDto = JsonConvert.DeserializeObject<ImportEmployeesDto[]>(jsonString);

            List<Employee> employeesToAdd = new List<Employee>();

            foreach (var employeeDto in employeesDto)
            {
                if (!IsValid(employeeDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!IsUsernameValid(employeeDto.Username))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Employee employee = new Employee
                {
                    Username = employeeDto.Username,
                    Email = employeeDto.Email,
                    Phone = employeeDto.Phone,
                };

                foreach (int taskId in employeeDto.Tasks.Distinct())
                {
                    Task task = context
                        .Tasks
                        .FirstOrDefault(t=>t.Id == taskId);

                    if (task == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    employee.EmployeesTasks.Add(new EmployeeTask
                    {
                        Employee = employee,
                        Task = task
                    });
                }
                employeesToAdd.Add(employee);
                sb.AppendLine(String.Format(SuccessfullyImportedEmployee, employee.Username, employee.EmployeesTasks.Count));
            }
            context.Employees.AddRange(employeesToAdd);
            context.SaveChanges();

            return sb.ToString().TrimEnd();

        }

        private static bool IsUsernameValid(string username)
        {
            foreach (char ch in username)
            {
                if (!Char.IsLetterOrDigit(ch))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}