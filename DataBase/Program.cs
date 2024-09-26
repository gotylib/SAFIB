// Создаем объект контекста
using DataBase;

using (var db = new ApplicationContext())
{
    // Заполняем таблицу Departments данными
    db.Departments.AddRange(
        new Department { Name = "Отдел продаж", ParentId = null, Status = "Активно" },
        new Department { Name = "Отдел маркетинга", ParentId = 1, Status = "Заблокировано" },
        new Department { Name = "Отдел разработки", ParentId = null, Status = "Активно" }
    );
    db.SaveChanges();
}

Console.WriteLine("База данных создана и заполнена данными.");