var builder = DistributedApplication.CreateBuilder(args);

// Configura��o do Publicador (API de Solicita��o de Empr�stimo)
builder.AddProject<Projects.CsharpAspirePubSub_Emprestimos_API>("emprestimosAPI");

// Configura��o do Assinante (Worker que processa solicita��es)
builder.AddProject<Projects.CsharpAspirePubSub_Emprestimos_Worker>("emprestimosWorker");

var app = builder.Build();
await app.RunAsync();