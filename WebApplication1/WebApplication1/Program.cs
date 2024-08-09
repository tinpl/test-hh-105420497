using Microsoft.EntityFrameworkCore;
using Thunk.Services.ExceptionsJournal;
using Thunk.Services.Tree;
using WebApi.Middleware;
using WebApi.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ExceptionsJournalFacade>(options =>
  options.UseNpgsql(builder.Configuration.GetConnectionString("ExceptionsJournal"))
    .UseSnakeCaseNamingConvention()
);
builder.Services.AddScoped<ExceptionsJournalService>();

builder.Services.AddDbContext<TreeFacade>(options =>
  options.UseNpgsql(builder.Configuration.GetConnectionString("Tree"))
    .UseSnakeCaseNamingConvention());
builder.Services.AddScoped<TreeService>();

builder.Services.AddScoped<IExceptionSender, ExceptionSender>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
  });
  app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();

app.Run();