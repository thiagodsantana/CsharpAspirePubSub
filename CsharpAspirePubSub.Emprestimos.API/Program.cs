using CsharpAspirePubSub.Emprestimos.API;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Adiciona padrões de serviços do .NET Aspire (logs, métricas, tracing)
builder.AddServiceDefaults();

// Configuração do Pub/Sub
const string IdProjeto = "united-perigee-140020";
const string IdTopico = "solicitacoes-emprestimos";
var nomeTopico = TopicName.FromProjectTopic(IdProjeto, IdTopico);

// Carrega credenciais padrão via ADC
GoogleCredential credenciais = await GoogleCredential.GetApplicationDefaultAsync();

// Cria o cliente do Pub/Sub
var publisherClient = new PublisherServiceApiClientBuilder
{
    Credential = credenciais
}.Build();

// Injeta dependências
builder.Services.AddSingleton(publisherClient);
builder.Services.AddSingleton(nomeTopico);
builder.Services.AddScoped<PublishEmprestimosService>();

// Health check básico para Aspire monitorar
builder.Services.AddHealthChecks();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Empréstimos",
        Version = "v1"
    });
});

var app = builder.Build();

// Roteamento e Swagger
app.MapDefaultEndpoints(); // Aspire: health, metrics, etc.
app.UseSwagger();
app.UseSwaggerUI();

/// <summary>
/// Endpoint que publica a solicitação de empréstimo no Pub/Sub.
/// </summary>
app.MapPost("/solicitar-emprestimo", async (SolicitacaoEmprestimo solicitacao,
    PublishEmprestimosService publicador,
    ILogger<Program> logger) =>
{
    logger.LogInformation("Solicitação recebida para cliente {Id}", solicitacao.IdCliente);

    await publicador.PublicarAsync(solicitacao);

    return Results.Ok("Solicitação de empréstimo enviada.");
});

app.Run();