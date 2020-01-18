using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;


namespace LibAzNaPratica {
    public class DisponibilidadeEntity : TableEntity
    {
        public DisponibilidadeEntity(string local, string horario)
        {
            PartitionKey = local;
            RowKey = horario;
        }
        
        public DisponibilidadeEntity() { }

        public string Mensagem { get; set; }
    }

    public class AcaoEntity : TableEntity
    {
        public AcaoEntity(string codigo, string horario)
        {
            PartitionKey = codigo;
            RowKey = horario;
        }
        public AcaoEntity() { }

        public double Valor { get; set; }
    }

    public class Acao
    {
        public string Codigo { get; set; }
        public double? Valor { get; set; }
    }
}
