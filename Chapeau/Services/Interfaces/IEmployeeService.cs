using Chapeau.Models;

namespace Chapeau.Services.Interfaces
{
    public interface IEmployeeService
    {
        List<Employee> GetAllEmployees();
        Employee GetEmployeeById(int id);
        Employee GetByUsernameAndPassword(string username, string password);
        void AddEmployee(Employee employee);
        void UpdateEmployee(Employee employee);
        void ActivateEmployee(int id);
        void DeactivateEmployee(int id);
        
    }
}