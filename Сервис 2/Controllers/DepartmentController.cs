using Microsoft.AspNetCore.Mvc;
using DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Сервис_А.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly ILogger<DepartmentController> _logger;
        private readonly ApplicationContext _db; 

        public DepartmentController(ILogger<DepartmentController> logger, ApplicationContext db) 
        {
            _logger = logger;
            _db = db;
        }

        // GET: api/Department/{departmentId}/status
        [HttpGet("{departmentId}/status")]
        public async Task<ActionResult<string>> GetDepartmentStatus(int departmentId)
        {
            var department = await _db.Departments.FindAsync(departmentId); // Запрос в БД
            if (department == null)
            {
                return NotFound(); // Возвращаем 404, если подразделение не найдено
            }

            return department.Status; // Возвращаем статус
        }

        // GET: api/Department/statuses
        [HttpGet("statuses")]
        public async Task<ActionResult<IEnumerable<Department>>> GetStatuses()
        {
            var departments = await _db.Departments.ToListAsync(); // Запрос в БД
            return departments; // Возвращаем список подразделений
        }
    }
}
