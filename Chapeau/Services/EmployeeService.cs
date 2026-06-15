using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.Repositories.Interfaces;
using System.Security.Cryptography;
using System.Text;
namespace Chapeau.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }
        public List<Employee> GetAllEmployees()
        {
            try
            {
                return _employeeRepository.GetAll();
            }
            catch { throw; }
        }
        public Employee GetEmployeeById(int id)
        {
            try
            {
                return _employeeRepository.GetById(id);
            }
            catch { throw; }
        }

        public Employee? GetByUsernameAndPassword(string username, string password)
        {
            return _employeeRepository.GetByUsernameAndPassword(username, password);
        }
        public void AddEmployee(Employee employee)
        {
            try
            {
                if (_employeeRepository.EmployeeNumberExists(employee.EmployeeNumber))
                    throw new Exception("Employee number is already in use!");

                Employee copyEmployee = new Employee(employee);
                copyEmployee.EmployeePassword = HashPassword(employee.EmployeePassword);
                _employeeRepository.Add(copyEmployee);
            }
            catch { throw; }
        }
        public void UpdateEmployee(Employee employee)
        {
            try
            {
                Employee copyEmployee = new Employee(employee);

                if (string.IsNullOrWhiteSpace(employee.EmployeePassword))
                {
                    // No new password provided -> keep the existing hashed password
                    Employee existing = _employeeRepository.GetById(employee.EmployeeId);
                    copyEmployee.EmployeePassword = existing.EmployeePassword;
                }
                else
                {
                    // New password provided -> hash it
                    copyEmployee.EmployeePassword = HashPassword(employee.EmployeePassword);
                }

                _employeeRepository.Update(copyEmployee);
            }
            catch { throw; }
        }
        public void ActivateEmployee(int id)
        {
            try { _employeeRepository.SetActive(id, true); }
            catch { throw; }
        }
        public void DeactivateEmployee(int id)
        {
            try { _employeeRepository.SetActive(id, false); }
            catch { throw; }
        }

        
    }
}