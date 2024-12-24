using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Channels;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            Console.WriteLine(GetEmployeesInPeriod(context).ToString());

        }

        
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {

            var employees = context.Employees.OrderBy(e => e.EmployeeId).Select
                (e => new 
                {
                    e.FirstName, 
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary
                }
                ).ToArray();
            StringBuilder allEmployees = new StringBuilder();

            foreach (var employee in employees)
            {
                allEmployees.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
            }
            
            return allEmployees.ToString().TrimEnd();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder selectedEmployees = new StringBuilder();
            var employees = context.Employees.Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    DepartmentName = e.Department.Name,
                    e.Salary
                })
                .OrderBy(e => e.Salary)
                .ThenBy(e => e.FirstName)
                .ToArray();

            foreach (var employee in employees) 
            {
                selectedEmployees.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.DepartmentName} - ${employee.Salary:f2}");
            
            }

            return selectedEmployees.ToString().TrimEnd();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            var nakov = context.Employees.FirstOrDefault(e => e.LastName == "Nakov");

            nakov!.Address = address;

            context.SaveChanges();

            StringBuilder sb = new StringBuilder();

            var employees = context.Employees.OrderByDescending(e => e.AddressId).Take(10).Select(e => e.Address!.AddressText).ToArray();

           foreach(var employee in employees) 
            {
                sb.AppendLine(employee);
            }

           return sb.ToString();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                //.Where(e => e.EmployeesProjects
                //.Any(p => p.Project!.StartDate.Year >= 2001 && 
                //     p.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager!.FirstName,
                    ManagerLastName = e.Manager!.LastName,
                    Projects = e.EmployeesProjects
                    .Where(ep => ep.Project!.StartDate.Year >= 2001 && 
                           ep.Project.StartDate.Year <= 2003)
                    .Select(p => new
                    {
                        ProjectName = p.Project!.Name,
                        StartDate = p.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                        EndDate = p.Project.EndDate.HasValue ? p.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture) : "not finished"

                    }).ToArray(),
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();
             
            foreach(var employee in employees) 
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - Manager: {employee.ManagerFirstName} {employee.ManagerLastName}");
                foreach(var project in employee.Projects)
                {
                    sb.AppendLine($"--{project.ProjectName} - {project.StartDate} - {project.EndDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            var emProject = context.EmployeesProjects.Where(ep => ep.ProjectId == 2);
            context.EmployeesProjects.RemoveRange(emProject);

            var project = context.Projects.Where(p => p.ProjectId == 2);
            context.Projects.Remove(project.FirstOrDefault()!);
            context.SaveChanges();

            var projectList = context.Projects.Take(10).Select(p => p.Name).ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var pr in projectList)
            {
                sb.AppendLine(pr);
            }

            return sb.ToString().TrimEnd();
        }
    }
}