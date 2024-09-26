using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; } // Внешний ключ для родительского подразделения
        public string Status { get; set; }

        // Навигационное свойство для родительского подразделения
        public virtual Department Parent { get; set; }

        // Навигационное свойство для дочерних подразделений
        public virtual ICollection<Department> Children { get; set; } = new List<Department>();
    }

}
