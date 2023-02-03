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

var students = app.MapGroup("/api/students");

app.MapGet("/", StudentService.GetAllStudents);
app.MapGet("/school/{school}", StudentService.GeStudentsBySchool);
app.MapGet("/{id}", StudentService.GetStudentById);
app.MapPost("/", StudentService.InsertStudent);
app.MapPut("/{id}", StudentService.UpdateStudent);
app.MapDelete("/{id}", StudentService.DeleteStudent);

app.Run();

