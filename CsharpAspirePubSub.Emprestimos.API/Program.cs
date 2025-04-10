using CsharpAspirePubSub.Emprestimos.API;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Caminho do arquivo de credenciais para autenticação no Google Cloud
string caminhoCredenciais = "D:\\united-perigee-140020-9b5785432997.json";

// Carregar as credenciais do Google Cloud a partir do arquivo JSON
GoogleCredential credenciais = GoogleCredential.FromFile(caminhoCredenciais);

// Criar o cliente do Pub/Sub utilizando as credenciais carregadas
var publisherClient = new PublisherServiceApiClientBuilder { Credential = credenciais }.Build();

// Registrar o cliente do Pub/Sub como um serviço singleton na injeção de dependências
builder.Services.AddSingleton(publisherClient);

// Adicionar suporte ao Swagger para documentação da API
builder.Services.AddSwaggerGen();

var app = builder.Build();

/// <summary>
/// Endpoint para solicitar um empréstimo, publicando a solicitação no Google Pub/Sub.
/// </summary>
app.MapPost("/solicitar-emprestimo", async (SolicitacaoEmprestimo solicitacaoEmprestimo, PublisherServiceApiClient cliente) =>
{
    // Identificador do projeto e do tópico no Pub/Sub
    string idProjeto = "united-perigee-140020";
    string idTopico = "solicitacoes-emprestimos";
    var nomeTopico = TopicName.FromProjectTopic(idProjeto, idTopico);

    // Serializar o objeto de solicitação de empréstimo em JSON
    string mensagem = JsonSerializer.Serialize(solicitacaoEmprestimo);

    // Criar a mensagem do Pub/Sub com os dados serializados
    var mensagemPubSub = new PubsubMessage { Data = Google.Protobuf.ByteString.CopyFromUtf8(mensagem) };

    // Criar a requisição para publicar a mensagem no tópico especificado
    var requisicaoPublicacao = new PublishRequest { Topic = nomeTopico.ToString(), Messages = { mensagemPubSub } };

    // Publicar a mensagem no Pub/Sub
    await cliente.PublishAsync(requisicaoPublicacao);

    return Results.Ok("Solicitação de empréstimo enviada");
});

// Configurar e ativar a interface do Swagger para visualização e teste dos endpoints
app.UseSwagger();
app.UseSwaggerUI();

// Iniciar a aplicação
app.Run();
