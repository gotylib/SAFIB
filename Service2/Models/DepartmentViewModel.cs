using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Сервис_Б.Models
{
    public class DepartmentViewModel
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public int? ParentDepartmentId { get; set; } // Внешний ключ для родительского подразделения

        // Навигационное свойство для родительского подразделения 
        public virtual DepartmentViewModel Parent { get; set; }

        // Навигационное свойство для дочерних подразделений 
        public virtual ICollection<DepartmentViewModel> Children { get; set; } = new List<DepartmentViewModel>();
    }
}
