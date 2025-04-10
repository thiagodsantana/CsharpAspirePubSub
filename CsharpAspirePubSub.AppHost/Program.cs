var builder = DistributedApplication.CreateBuilder(args);

// Configuração do Publicador (API de Solicitação de Empréstimo)
builder.AddProject<Projects.CsharpAspirePubSub_Emprestimos_API>("emprestimosAPI");

// Configuração do Assinante (Worker que processa solicitações)
builder.AddProject<Projects.CsharpAspirePubSub_Emprestimos_Worker>("emprestimosWorker");

var app = builder.Build();
await app.RunAsync();