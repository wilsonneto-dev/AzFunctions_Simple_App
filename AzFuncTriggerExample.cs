using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

using LibAzNaPratica;

namespace Enterprise.Function
{
    public static class AzFuncTriggerExample
    {
        [FunctionName("AzFuncTriggerExample")]
        public static void Run([TimerTrigger("*/30 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# timer runned");

            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            var disponibilidadeTable = storageAccount
                .CreateCloudTableClient()
                .GetTableReference("LogDisponibilidade");

            if (disponibilidadeTable.CreateIfNotExistsAsync().Result)
                log.LogInformation("Criando a tabela LogDisponibilidade...");
            
            DisponibilidadeEntity dadosDisponibilidade =
                new DisponibilidadeEntity(
                    Environment.GetEnvironmentVariable("LocalExecucao"),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                );
            
            dadosDisponibilidade.Mensagem = "FunctionAppAcoes em execucao";
            
            var insertOperation = TableOperation.Insert(dadosDisponibilidade);
            
            var resultInsert = disponibilidadeTable.ExecuteAsync(insertOperation).Result;

            log.LogInformation($"**** Teste de disponibilidade executado em : {DateTime.Now}");
        }
    }
}

        