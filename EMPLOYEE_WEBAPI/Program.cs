using Employee_Service.Service;
using Employee_Service.Contracts;
using Employee_Shared_Service.Service;
using Employee_Shared_Service.Contracts;
using Employee_DataAccessLayer;
using Employee_DataAccessLayer.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<DapperContext>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddTransient<IOperationTransient, Operation>();
builder.Services.AddScoped<IOperationScoped, Operation>();
builder.Services.AddSingleton<IOperationSingleton, Operation>();

builder.Services.AddSingleton<ILoggerManager, LoggerManager>();
builder.Services.AddTransient<ILogHeaders, LogHeaders>();
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


app.Run();
