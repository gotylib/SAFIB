using DataBase;
using Microsoft.EntityFrameworkCore;

using (var db = new ApplicationContext())
{

    // Проверяем, есть ли уже данные в базе
    if (!db.Departments.Any())
    {
        // Создание тестовых данных
        var parentDept = new Department { Name = "Test Department", Status = "Активно" };
        var childDept1 = new Department { Name = "Test Department 1", Status = "Активно", ParentId = parentDept.Id };
        var childDept2 = new Department { Name = "Test Department 2", Status = "Неактивно", ParentId = parentDept.Id };

        parentDept.Children.Add(childDept1);
        parentDept.Children.Add(childDept2);

        db.Departments.Add(parentDept);
        db.SaveChanges(); // Сохраняем изменения в базе
    }
}

    // Выводим данные из базы
    using (var db = new ApplicationContext())
{
    var departments = db.Departments.Include(d => d.Children).ToList();
    Console.WriteLine("Список подразделений:");

    foreach (var department in departments.Where(d => d.ParentId == null))
    {
        PrintDepartmentHierarchy(department, 0);
    }
}
Console.WriteLine("База данных создана и заполнена данными.");

void PrintDepartmentHierarchy(Department department, int indentLevel)
{
    // Выводим информацию о текущем подразделении
    Console.WriteLine($"{new string(' ', indentLevel * 4)}ID: {department.Id}, Name: {department.Name}, Status: {department.Status}");

    // Рекурсивно выводим всех дочерних подразделений
    foreach (var child in department.Children)
    {
        PrintDepartmentHierarchy(child, indentLevel + 1);
    }
}