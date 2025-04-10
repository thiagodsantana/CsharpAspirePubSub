# Solução de Processamento de Empréstimos com Google Pub/Sub e .NET Aspire

Este projeto implementa uma solução de solicitação e processamento de empréstimos utilizando Google Cloud Pub/Sub e .NET Aspire.

## Tecnologias Utilizadas

- .NET Aspire
- ASP.NET Core
- Google Cloud Pub/Sub
- Swagger para documentação da API

## Configuração

1. **Criar um projeto no Google Cloud Platform (GCP)**
   - Acesse [Google Cloud Console](https://console.cloud.google.com/)
   - Crie um novo projeto ou utilize um existente
   
2. **Criar um tópico e uma inscrição no Pub/Sub**
   - No GCP, navegue até Pub/Sub
   - Crie um tópico chamado `solicitacoes-emprestimos`
   - Crie uma inscrição chamada `solicitacoes-emprestimos-sub`

3. **Gerar credenciais de autenticação**
   - No GCP, acesse IAM & Admin > Contas de Serviço
   - Crie uma nova conta de serviço e gere uma chave JSON
   - Salve o arquivo JSON e ajuste o caminho no código

4. **Configurar a solução**
   - No arquivo de código, substitua `seu-projeto-gcp` pelo ID do seu projeto
   - Substitua o caminho do arquivo JSON no Worker e na API

## Como Executar

### Executar a API (Publicador)
No terminal, dentro da pasta da API, execute:
```sh
 dotnet run
```
Isso iniciará um servidor local na porta padrão.

### Executar o Worker (Assinante)
No terminal, dentro da pasta do Worker, execute:
```sh
 dotnet run
```
O Worker irá se inscrever no tópico do Pub/Sub e processar mensagens recebidas.

## Testando a API
Acesse o Swagger via:
```
http://localhost:5000/swagger
```
Envie uma requisição POST para `/solicitar-emprestimo` com um payload JSON como este:
```json
{
    "idCliente": "123456",
    "valor": 10000.50,
    "prazoMeses": 24
}
```
Se configurado corretamente, o Worker processará a solicitação e exibirá logs no console.

## Considerações Finais

- Certifique-se de que as credenciais do GCP estejam corretas.
- O Worker deve estar rodando para processar as mensagens.
- Para ambiente de produção, considere melhorar o tratamento de erros e logs.

## Autor
Este projeto foi desenvolvido para demonstrar integração entre .NET Aspire e Google Pub/Sub para processamento de empréstimos.

