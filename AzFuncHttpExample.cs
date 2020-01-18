using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

using LibAzNaPratica;

namespace Enterprise.Function
{
    public static class AzFuncHttpExample
    {
        [FunctionName("AzFuncHttpExample")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] 
            HttpRequest req, 
            ILogger log
        )
        {
            log.LogInformation("Executando C# HTTP trigger de consulta a cotacoes...");
 
            string codigoAcao = req.Query["acao"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            codigoAcao = codigoAcao ?? data?.name;

            var storageAccount = CloudStorageAccount
                .Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
 
            var acaoTable = storageAccount
                .CreateCloudTableClient().GetTableReference("CotacaoAcoes");
 
            var query = new TableQuery<AcaoEntity>().Where(
                TableQuery.GenerateFilterCondition(
                    "PartitionKey", 
                    QueryComparisons.Equal, 
                    codigoAcao
                )
            );

            List<AcaoEntity> listAcaoEntity = new List<AcaoEntity>();
 
            TableQuerySegment<AcaoEntity> segment;
 
            TableContinuationToken continuationToken = null;
            
            do
            {
                segment = await acaoTable.ExecuteQuerySegmentedAsync(query, continuationToken);
                if (segment == null)
                {
                    break;
                }
                
                listAcaoEntity.AddRange(segment);
 
                continuationToken = segment.ContinuationToken;
 
            } while (continuationToken != null);
            
            var cotacoes = from c in listAcaoEntity
                select new
                {
                    Acao = c.PartitionKey,
                    Horario = c.RowKey,
                    c.Valor
                };

            return codigoAcao != null
                ? (ActionResult) new OkObjectResult(cotacoes)
                : new BadRequestObjectResult(
                    new
                    {
                        Sucesso = false,
                        Mensagem = "Informe um código para a pesquisa das ações"
                    }
                );
            
        }
    }
}

        