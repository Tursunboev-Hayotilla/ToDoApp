
using Serilog;
using Todo.API.Middlewares;
using Todo.Domain.Entities.Auth;
using Todo.Infrsatructure;
using Todo.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Todo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {


            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(option =>
            {
                option.AddPolicy(name: MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.AllowAnyHeader().
                        AllowAnyOrigin().
                        AllowAnyMethod();
                    });
            });
            var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();
            //builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);

            // Add services to the container.

            builder.Services.AddInfrastructure(builder.Configuration);


            builder.Services.AddControllers();


            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ToDoDbContext>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseHttpsRedirection();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
