using HRManagementBackend.Data;
using HRManagementBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Dapper context and EmployeeService
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<LeaveService>();
builder.Services.AddScoped<HolidayService>();



var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
