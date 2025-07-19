using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;


var builder = WebApplication.CreateBuilder(args);




builder.Services.AddDependencies(builder.Configuration);

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();

app.UseHttpsRedirection();




app.UseHangfireDashboard("/Jobs", new DashboardOptions
{
    Authorization =
    [
        new HangfireCustomBasicAuthenticationFilter
        {
            Pass = app.Configuration.GetValue<string>("HangfireSettings:Username"),
            User = app.Configuration.GetValue<string>("HangfireSettings:Password")
        }
    ]
});


var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

RecurringJob.AddOrUpdate("SendNewPollsNotification", () => notificationService.SendNewPollsNotification(null), Cron.Daily);



app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.MapHealthChecks("/heath", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
});



app.Run();