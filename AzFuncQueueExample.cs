using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

using Newtonsoft.Json;

using LibAzNaPratica;

namespace Enterprise.Function
{
    public static class AzFuncQueueExample
    {
        [FunctionName("AzFuncQueueExample")]
        public static void Run([QueueTrigger("acoes", Connection = "10storagevisualstudio_STORAGE")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processing: {myQueueItem}");

            var cotacao =
                JsonConvert.DeserializeObject<Acao>(myQueueItem);
                
            if (!String.IsNullOrWhiteSpace(cotacao.Codigo) &&
                cotacao.Valor.HasValue && cotacao.Valor > 0)
            {
                cotacao.Codigo = cotacao.Codigo.Trim().ToUpper();
                var storageAccount = CloudStorageAccount
                    .Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
                
                var acaoTable = storageAccount
                    .CreateCloudTableClient()
                    .GetTableReference("CotacaoAcoes");
                
                if (acaoTable.CreateIfNotExistsAsync().Result)
                    log.LogInformation("Criando a tabela CotacaoAcoes...");

                AcaoEntity dadosAcao =
                    new AcaoEntity(
                        cotacao.Codigo,
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    );
                
                dadosAcao.Valor = cotacao.Valor.Value;
                
                var insertOperation = TableOperation.Insert(dadosAcao);
                var resultInsert = acaoTable.ExecuteAsync(insertOperation).Result;
                
                log.LogInformation($"AcoesQueueTrigger: {myQueueItem}");
            }
            else
                log.LogError($"AcoesQueueTrigger - Erro validação: {myQueueItem}");
        }
    }
}
