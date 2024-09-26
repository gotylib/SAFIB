using DataBase;
using Microsoft.EntityFrameworkCore;

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
