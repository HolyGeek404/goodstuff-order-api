using GoodStuff.OrderApi.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerConfig(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAzureConfig(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GoodStuff Order Api v1");
    c.OAuthClientId(builder.Configuration["Swagger:SwaggerClientId"]);
    c.OAuthUsePkce();
    c.OAuthScopeSeparator(" ");
}
    );
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();