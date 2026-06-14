using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class EmployeeManagementViewModel
    {
        public List<Employee> Employees { get; set; } = new List<Employee>();
        public Employee EmployeeToEdit { get; set; } = new Employee();
    }
}