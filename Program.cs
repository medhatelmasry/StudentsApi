using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using StudentsApi.Data;
using StudentsApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SchoolDbContext>(option => option.UseSqlite(connectionString));

// Add Cors
builder.Services.AddCors(o => o.AddPolicy("Policy", builder => {
  builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
}));


var app = builder.Build();

app.UseCors("Policy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/students", [EnableCors("Policy")] async (SchoolDbContext db) =>
    await db.Students.ToListAsync());

app.MapGet("/api/students/school/{school}", [EnableCors("Policy")] async (string school, SchoolDbContext db) =>
    await db.Students.Where(t => t.School!.ToLower() == school.ToLower()).ToListAsync());

app.MapGet("/api/students/{id}", [EnableCors("Policy")] async (int id, SchoolDbContext db) =>
    await db.Students.FindAsync(id)
        is Student student ? Results.Ok(student) : Results.NotFound());

app.MapPost("/api/students", [EnableCors("Policy")] async (Student student, SchoolDbContext db) =>
{
    db.Students.Add(student);
    await db.SaveChangesAsync();

    return Results.Created($"/students/{student.StudentId}", student);
});

app.MapPut("/api/students/{id}", [EnableCors("Policy")] async (int id, Student inputStudent, SchoolDbContext db) =>
{
    var student = await db.Students.FindAsync(id);

    if (student is null) return Results.NotFound();

    student.FirstName = inputStudent.FirstName;
    student.LastName = inputStudent.LastName;
    student.School = inputStudent.School;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/api/students/{id}", [EnableCors("Policy")] async (int id, SchoolDbContext db) =>
{
    if (await db.Students.FindAsync(id) is Student student)
    {
        db.Students.Remove(student);
        await db.SaveChangesAsync();
        return Results.Ok(student);
    }

    return Results.NotFound();
});


app.Run();

