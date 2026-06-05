using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class EmployeeManagementViewModel
    {
        public List<Employee> Employees { get; set; }
        public Employee EmployeeToEdit { get; set; } = new Employee();
    }
}