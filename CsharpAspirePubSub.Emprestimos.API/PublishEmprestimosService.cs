using Google.Cloud.PubSub.V1;
using System.Text.Json;

namespace CsharpAspirePubSub.Emprestimos.API
{
    /// <summary>
    /// Serviço responsável por publicar solicitações de empréstimo no Google Pub/Sub.
    /// </summary>

    public class PublishEmprestimosService
    {
        private readonly PublisherServiceApiClient _cliente;
        private readonly TopicName _topico;

        public PublishEmprestimosService(PublisherServiceApiClient cliente, TopicName topico)
        {
            _cliente = cliente;
            _topico = topico;
        }

        public async Task PublicarAsync(SolicitacaoEmprestimo solicitacao)
        {
            var json = JsonSerializer.Serialize(solicitacao);
            var mensagem = new PubsubMessage
            {
                Data = Google.Protobuf.ByteString.CopyFromUtf8(json)
            };

            var requisicao = new PublishRequest
            {
                Topic = _topico.ToString(),
                Messages = { mensagem }
            };

            await _cliente.PublishAsync(requisicao);
        }
    }
}
