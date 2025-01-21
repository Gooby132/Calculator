
using Calculator.Domain.OperationsService;
using Calculator.Domain.Users;
using Calculator.Infrastructure.RemoteOperations;
using Calculator.Persistence.Context;
using Calculator.Persistence.Users;

namespace Calculator.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ApplicationContext>();
            builder.Services.AddTransient<IUserRepository, UsersRepository>();
            builder.Services.AddTransient<IOperationService, RemoteOperationService>();
            builder.Services.AddTransient<IUnitOfWork, ApplicationUnitOfWork>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
