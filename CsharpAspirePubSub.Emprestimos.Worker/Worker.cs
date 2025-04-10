using CsharpAspirePubSub.Emprestimos.API;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using System.Text.Json;

namespace CsharpAspirePubSub.Emprestimos.Worker
{
    /// <summary>
    /// Classe Worker respons�vel por escutar mensagens do Pub/Sub e processar solicita��es de empr�stimos.
    /// </summary>
    public class Worker(ILogger<Worker> logger) : BackgroundService
    {
        // Identificador do projeto no Google Cloud
        public const string idProjeto = "united-perigee-140020";

        // Identificador da inscri��o (subscription) que o Worker ir� escutar
        public const string idInscricao = "solicitacoes-emprestimos-sub";

        // Caminho para o arquivo de credenciais do Google Cloud
        private readonly string caminhoCredenciais = "D:\\united-perigee-140020-9b5785432997.json";

        /// <summary>
        /// M�todo que inicia a execu��o do Worker para escutar mensagens do Pub/Sub.
        /// </summary>
        /// <param name="stoppingToken">Token para cancelar a execu��o do Worker.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Carregar credenciais do arquivo JSON para autentica��o no Google Cloud
            GoogleCredential credenciais = await GoogleCredential.FromFileAsync(caminhoCredenciais, stoppingToken);

            // Criar o nome da inscri��o (subscription) no Pub/Sub
            var nomeInscricao = SubscriptionName.FromProjectSubscription(idProjeto, idInscricao);

            // Criar um cliente assinante (SubscriberClient) utilizando as credenciais carregadas
            var subscriber = await new SubscriberClientBuilder
            {
                SubscriptionName = nomeInscricao,
                Credential = credenciais
            }.BuildAsync(stoppingToken);

            logger.LogInformation("Worker iniciado e aguardando mensagens...");

            // Iniciar a escuta das mensagens no t�pico assinado
            await subscriber.StartAsync((mensagem, cancel) =>
            {
                // Desserializar a mensagem recebida do Pub/Sub
                var solicitacaoEmprestimo = JsonSerializer.Deserialize<SolicitacaoEmprestimo>(mensagem.Data.ToStringUtf8());

                // Log das informa��es do empr�stimo processado
                logger.LogInformation($"Processando empr�stimo: Cliente {solicitacaoEmprestimo!.IdCliente}, " +
                                      $"Valor {solicitacaoEmprestimo.Valor}, " +
                                      $"Prazo {solicitacaoEmprestimo.PrazoMeses} meses");

                // Confirmar que a mensagem foi processada com sucesso (ACK)
                return Task.FromResult(SubscriberClient.Reply.Ack);
            });
        }
    }
}
