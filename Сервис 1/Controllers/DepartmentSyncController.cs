using DataBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Сервис_Б.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Сервис_Б.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DepartmentSyncController : ControllerBase
    {
        private readonly ILogger<DepartmentSyncController> _logger;
        private readonly ApplicationContext _db;


        public DepartmentSyncController(ILogger<DepartmentSyncController> logger, ApplicationContext db)
        {
            _logger = logger;
            _db = db;
        }
        // Метод для построения иерархии
        private List<DepartmentViewModel> BuildHierarchy(List<DepartmentViewModel> departments, Dictionary<int, DepartmentViewModel> departmentDict)
        {
            var hierarchy = new List<DepartmentViewModel>();

            foreach (var department in departments)
            {
                if (department.ParentDepartmentId == null)
                {
                    hierarchy.Add(department);
                }
                else if (departmentDict.TryGetValue(department.ParentDepartmentId.Value, out var parent))
                {
                    // Добавляем дочерний элемент к родителю
                    if (parent.Children == null)
                        parent.Children = new List<DepartmentViewModel>();

                    parent.Children.Add(department);
                }
            }

            return hierarchy;
        }

        //Метод для генерации токена
        private string GenerateJwtToken()
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, "ServiceB"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aXj9cK2F7s8i4M1At0rDqLs5UbWz3VeYpEoHnXf6ZbIuBlJmQvTzCR9gWkF3Qj0oEtZaPpVYcM2Rf6H5eIx7S0LmJ3TcU8xYlK9VuFh4DiQmN0Yz5TfH2gMmOx8Rna6DwWqSvY1ZsF8Pj3JeRqU5KqL2BvIxY4GmN9ZoR7FqXc9KpCL2FgVrB1Ad3H6QzU8sM1EoG34WX9VfZx7JjHkUm5Rz1LgBc2YpQnTdV8K6PlGjR3FyAtX9VmZ5SkO8ZhN0LfAqD1CtJ7XyY2Oa4Ws3PrB9NeQ7GdKl0E6Kh8Tx5S1BzQvG3YhXz4OpRqF9M5Ct3KxU1Hw8Yp2DfRqW0J"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "YourIssuer",
                audience: "YourAudience",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        //Get: /DepartmentSync?name
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentViewModel>>> GetDepartments(string name)
        {
            var token = GenerateJwtToken(); // Генерация токена  
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var allDepartments = await _db.Departments.ToListAsync(); // Проверяем наличие данных перед фильтрацией
            if (allDepartments.Count == 0)
            {
                _logger.LogWarning("No departments found in the database.");
                return Ok(allDepartments); // сделано, что бы не выкидывало ошибки на фронт
            }

            IQueryable<Department> departmentsQuery = _db.Departments.AsNoTracking(); // Используем AsNoTracking если требуется

            // Если имя не задано или пустое, получаем все подразделения  
            if (!string.IsNullOrWhiteSpace(name))
            {
                departmentsQuery = departmentsQuery.Where(d => d.Name.Contains(name));
            }

            var departments = await departmentsQuery.ToListAsync();
            if (departments.Count == 0)
            {
                _logger.LogWarning("No departments found matching the criteria.");
                return Ok(departments);// сделано, что бы не выкидывало ошибки на фронт
            }

            var result = new List<DepartmentViewModel>();
            var departmentDict = new Dictionary<int, DepartmentViewModel>();

            // Заполнение иерархической структуры  
            foreach (var d in departments)
            {
                var response = await httpClient.GetAsync($"https://localhost:5000/Department/{d.Id}/status");
                string status = response.IsSuccessStatusCode
                    ? await response.Content.ReadAsStringAsync()
                    : "Error";

                var departmentViewModel = new DepartmentViewModel
                {
                    Name = d.Name,
                    Status = status,
                    ParentDepartmentId = d.ParentId
                };

                result.Add(departmentViewModel);
                departmentDict[d.Id] = departmentViewModel;
            }
            // Создаем иерархическую структуру  
            var hierarchicalResult = BuildHierarchy(result, departmentDict);

            return Ok(hierarchicalResult);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<DepartmentViewModel>>> GetDepartments()
        {
            var token = GenerateJwtToken(); // Генерация токена  
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var allDepartments = await _db.Departments.ToListAsync(); // Проверяем наличие данных перед фильтрацией
            if (allDepartments.Count == 0)
            {
                _logger.LogWarning("No departments found in the database.");
                return Ok(allDepartments);
            }

            IQueryable<Department> departmentsQuery = _db.Departments.AsNoTracking(); // Используем AsNoTracking если требуется

            var departments = await departmentsQuery.ToListAsync();
            if (departments.Count == 0)
            {
                _logger.LogWarning("No departments found matching the criteria.");
                return Ok(departments);
            }

            var result = new List<DepartmentViewModel>();
            var departmentDict = new Dictionary<int, DepartmentViewModel>();

            // Заполнение иерархической структуры  
            foreach (var d in departments)
            {
                var response = await httpClient.GetAsync($"https://localhost:5000/Department/{d.Id}/status");
                string status = response.IsSuccessStatusCode
                    ? await response.Content.ReadAsStringAsync()
                    : "Error";

                var departmentViewModel = new DepartmentViewModel
                {
                    Name = d.Name,
                    Status = status,
                    ParentDepartmentId = d.ParentId
                };

                result.Add(departmentViewModel);
                departmentDict[d.Id] = departmentViewModel;
            }
            // Создаем иерархическую структуру  
            var hierarchicalResult = BuildHierarchy(result, departmentDict);

            return Ok(hierarchicalResult);
        }

        // POST: /DepartmentSync/sync
        [HttpPost("sync")]
        public async Task<IActionResult> SyncDepartments()
        {
            // Путь к файлу
            var filePath = "file.json";

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Файл не найден.");
            }

            var jsonData = await System.IO.File.ReadAllTextAsync(filePath);
            var departmentsFromFile = JsonConvert.DeserializeObject<List<Department>>(jsonData);

            if (departmentsFromFile == null)
            {
                return BadRequest("Ошибка при десериализации данных из файла.");
            }

            // Логика синхронизации
            foreach (var department in departmentsFromFile)
            {
                var existingDepartment = await _db.Departments.FindAsync(department.Id);

                if (existingDepartment == null)
                {
                    // Если подразделение нет в БД, добавляем его
                    await _db.Departments.AddAsync(department);
                }
                else
                {
                    // Если подразделение есть в БД, обновляем имя и статус
                    existingDepartment.Name = department.Name; // Обновляем имя
                    existingDepartment.Status = department.Status; // Обновляем статус

                    // Проверяем положение в дереве
                    if (existingDepartment.ParentId != department.ParentId) // Сравниваем родительские идентификаторы
                    {
                        existingDepartment.ParentId = department.ParentId; // Обновляем родителя
                    }

                    _db.Departments.Update(existingDepartment); // Обновляем запись
                }
            }

            await _db.SaveChangesAsync(); // Сохраняем изменения в БД
            return Ok("Синхронизация завершена успешно."); // Возвращаем сообщение об успешной синхронизации
        }

    }
}
