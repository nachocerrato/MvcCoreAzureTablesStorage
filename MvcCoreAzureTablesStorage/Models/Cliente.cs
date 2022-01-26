using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCoreAzureTablesStorage.Models
{
    public class Cliente : TableEntity
    {
        //Row Key: IdCliente
        //Cuando escriban el ID de cliente, almacenamos Row Key
        private string _IdCliente;
        public string IdCliente
        {
            get { return this._IdCliente; }
            set
            {
                this._IdCliente = value;
                this.RowKey = value;
            }
        }

        //Partition Key: Empresa
        //Cuando escriban la empresa, almacenamos el Partition Key

        private string _Empresa;
        public string Empresa
        {
            get { return this._Empresa; }
            set
            {
                this._Empresa = value;
                this.PartitionKey = value;
            }
        }

        public string Nombre { get; set; }
        public string Edad { get; set; }
    }
}
