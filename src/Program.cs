using src.controllers;
using src.services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

builder.Services.AddOpenApi();
builder.Services.AddScoped<CifraService>();
builder.Services.AddScoped<DicionarioAbertoValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapCifraEndpoints();

app.Run();