using ExpenseTracker.DataLayer.Context;
using ExpenseTracker.DataLayer.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ExpenseTracker.DataLayer.Entities;
using ExpenseTracker.DataLayer.Repositories;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ExpenseTracker.Application.Validators;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using ExpenseTrackerApi.GlobalExceptionHandler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();

// Add AutoValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryDtoValidator>();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//DataBase Set
builder.Services.AddDbContext<AppDbContext>(option => 
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Add Service Mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//DI Repository
builder.Services.AddScoped<IAuthInfoRepository,AuthInfoRepository>();
builder.Services.AddScoped<ICategoryInfoRepository, CategoryInfoRepository>();
builder.Services.AddScoped<IExpenseInfoRepository, ExpenseInfoRepository>();


#region Jwt Config
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? "shhhhhhh you know?") 
                ),
        };
    });
#endregion

var app = builder.Build();

#region Pipline
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
#endregion