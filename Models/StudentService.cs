using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentsApi.Models;

[EnableCors("Policy")]
public class StudentService
{
    public static async Task<IResult> GetAllStudents(SchoolDbContext db)
    {
        return TypedResults.Ok(await db.Students.ToListAsync());
    }

    public static async Task<IResult> GeStudentsBySchool(string school, SchoolDbContext db)
    {
        return TypedResults.Ok(await db.Students.Where(t => t.School!.ToLower() == school.ToLower()).ToListAsync());
    }

    public static async Task<IResult> GetStudentById(int id, SchoolDbContext db)
    {
        return TypedResults.Ok(await db.Students.FindAsync(id)
            is Student student ? Results.Ok(student) : Results.NotFound());
    }

    public static async Task<IResult> InsertStudent(Student student, SchoolDbContext db)
    {
        db.Students.Add(student);
        await db.SaveChangesAsync();

        return TypedResults.Created($"/students/{student.StudentId}", student);
    }

    public static async Task<IResult> UpdateStudent(int id, Student inputStudent, SchoolDbContext db)
    {
        var student = await db.Students.FindAsync(id);

        if (student is null) return Results.NotFound();

        student.FirstName = inputStudent.FirstName;
        student.LastName = inputStudent.LastName;
        student.School = inputStudent.School;

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    public static async Task<IResult> DeleteStudent(int id, SchoolDbContext db)
    {
        if (await db.Students.FindAsync(id) is Student student)
        {
            db.Students.Remove(student);
            await db.SaveChangesAsync();
            return TypedResults.Ok(student);
        }

        return TypedResults.NotFound();
    }
}
