using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PhoneApp.Domain.DTO
{
    public partial class EmployeesDTO : DataTransferObject
    {
        public string Name { get; private set; }
        public string Phone => _phones.Any() ? _phones.LastOrDefault().Value : "-";
        
        private readonly Dictionary<DateTime, string> _phones = new Dictionary<DateTime, string>();

        public EmployeesDTO()
        {
            
        }
        
        private EmployeesDTO(string name)
        {
            Name = name;
        }

        public static (bool isSuccess, string message, EmployeesDTO employee) TryCreateEmployeesDto(string name, string phone)
        {
            var validationMessage = ValidateInput(name, phone);
            if (!string.IsNullOrEmpty(validationMessage))
            {
                return (false, validationMessage, null);
            }

            var employee = new EmployeesDTO(name);
            employee.AddPhone(phone);
            return (true, $"{name} added to employees", employee);
        }

        private static string ValidateInput(string name, string phone)
        {
            var message = new StringBuilder();

            if (string.IsNullOrWhiteSpace(name))
            {
                message.Append("Name must be provided! ");
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                message.Append("Phone must be provided! ");
            }
            else if (!IsValidPhone(phone))
            {
                message.Append("Invalid phone number format! ");
            }

            return message.ToString().Trim();
        }

        private static bool IsValidPhone(string phone)
        {
            var pattern = @"^\+?\d{1,3}[\s-]?\(?\d{1,4}\)?[\s-]?\d{1,4}[\s-]?\d{1,4}[\s-]?\d{1,4}$";
            return Regex.IsMatch(phone, pattern);
        }

        private void AddPhone(string phone)
        {
            _phones.Add(DateTime.Now, phone);
        }
    }
}