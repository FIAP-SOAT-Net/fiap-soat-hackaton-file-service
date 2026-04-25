var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/healthcheck");

app.MapPost("/files/upload", (IFormFile file) =>
{
    if (file.Length == 0)
    {
        return Results.BadRequest(new { message = "Empty file." });
    }

    var isPdf = string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase);
    var isImage = file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);

    if (!isPdf && !isImage)
    {
        return Results.BadRequest(new { message = "Only PDF or image files are allowed." });
    }

    return Results.Ok(new
    {
        message = "File received successfully.",
        fileName = file.FileName,
        contentType = file.ContentType,
        size = file.Length
    });
})
.Accepts<IFormFile>("multipart/form-data")
.WithName("UploadFile");

await app.RunAsync();
