using CsharpAspirePubSub.Emprestimos.API;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using System.Text.Json;

namespace CsharpAspirePubSub.Emprestimos.Worker;

/// <summary>
/// Worker que consome mensagens do Pub/Sub e processa empr�stimos.
/// </summary>
public class Worker(ILogger<Worker> logger) : BackgroundService
{
    private const string IdProjeto = "united-perigee-140020";
    private const string IdSubscription = "solicitacoes-emprestimos-sub";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Autentica com ADC
        GoogleCredential credenciais = await GoogleCredential.GetApplicationDefaultAsync(stoppingToken);

        var subscription = SubscriptionName.FromProjectSubscription(IdProjeto, IdSubscription);

        // Cria o assinante do Pub/Sub
        var subscriber = await new SubscriberClientBuilder
        {
            SubscriptionName = subscription,
            Credential = credenciais
        }.BuildAsync(stoppingToken);

        logger.LogInformation("Worker iniciado e aguardando mensagens do Pub/Sub...");

        await subscriber.StartAsync(async (mensagem, cancel) =>
        {
            try
            {
                var json = mensagem.Data.ToStringUtf8();
                var emprestimo = JsonSerializer.Deserialize<SolicitacaoEmprestimo>(json);

                if (emprestimo == null)
                {
                    logger.LogWarning("Mensagem nula ou mal formatada.");
                    return SubscriberClient.Reply.Nack;
                }

                await ProcessarSolicitacaoAsync(emprestimo);

                return SubscriberClient.Reply.Ack;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao processar mensagem.");
                return SubscriberClient.Reply.Nack;
            }
        });
    }

    private Task ProcessarSolicitacaoAsync(SolicitacaoEmprestimo solicitacao)
    {
        // Simula processamento da solicita��o
        logger.LogInformation("Processando empr�stimo: Cliente {IdCliente}, Valor: {Valor}, Prazo: {Prazo} meses",
            solicitacao.IdCliente,
            solicitacao.Valor,
            solicitacao.PrazoMeses);

        // Aqui entraria integra��o com banco, Redis, etc.
        return Task.CompletedTask;
    }
}
