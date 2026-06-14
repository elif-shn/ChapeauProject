using Chapeau.Enums;
namespace Chapeau.Models

{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNumber { get; set; }
        public EmployeeRole EmployeeOccupation { get; set; }
        public string EmployeePassword { get; set; }
        public bool IsActive { get; set; }

        public Employee() { }

       
        public Employee(Employee employee)
        {
            EmployeeId = employee.EmployeeId;
            EmployeeName = employee.EmployeeName;
            EmployeeNumber = employee.EmployeeNumber;
            EmployeeOccupation = employee.EmployeeOccupation;
            EmployeePassword = employee.EmployeePassword;
            IsActive = employee.IsActive;
        }
    }
}