using System.Collections.Generic;
using System.Linq;
using PhoneApp.Domain.DTO;
using PhoneApp.Domain.Interfaces;
using System.Net.Http;
using EmployeesLoaderFromApiPlugin.Dtos;
using PhoneApp.Domain.Attributes;

namespace EmployeesLoaderFromApiPlugin
{
    [Author(Name = "Landysh Habibullina")]
    public class Plugin : IPluggable
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public IEnumerable<DataTransferObject> Run(IEnumerable<DataTransferObject> args)
        {
            var dataUrl = "https://dummyjson.com/users";
            logger.Info($"Loading employees from {dataUrl}");

            var client = new HttpClient();
            var response = client.GetAsync(dataUrl).Result;
    
            var json = response.Content.ReadAsStringAsync().Result;
            var usersApiResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<UsersApiResponse>(json);

            var users = usersApiResponse.Users;
            var employeesList = new List<EmployeesDTO>();
    
            foreach (var user in users)
            {
                var name = $"{user.FirstName} {user.LastName}";
                var creationResult = EmployeesDTO.TryCreateEmployeesDto(name, user.Phone);
                if (!creationResult.isSuccess)
                {
                    logger.Warn(creationResult.message);
                    continue;
                }
                
                employeesList.Add(creationResult.employee);
            }
    
            logger.Info($"Loaded {employeesList.Count} employees");
            return employeesList.Cast<DataTransferObject>();
        }
    }
}