using Chapeau.Models;

namespace Chapeau.Repositories
{
    public interface IEmployeeRepository
    {
        List<Employee> GetAll();
        Employee GetById(int id);
        Employee? GetByUsernameAndPassword(string username, string password);
        void Add(Employee employee);
        void Update(Employee employee);
        void SetActive(int id, bool isActive);
        bool EmployeeNumberExists(string employeeNumber);

        Employee? GetByUsernameAndPassword(string username, string password);
    }
}