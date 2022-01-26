using Microsoft.Azure.Cosmos.Table;
using MvcCoreAzureTablesStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCoreAzureTablesStorage.Services
{
    public class ServiceTableStorage
    {
        private CloudTable tablaClientes;

        public ServiceTableStorage(string keys)
        {
            CloudStorageAccount account =
                CloudStorageAccount.Parse(keys);
            CloudTableClient client = account.CreateCloudTableClient();
            this.tablaClientes = client.GetTableReference("clientes");
            this.tablaClientes.CreateIfNotExistsAsync();
        }

        //Método para crear clientes
        public async Task CrearClienteAsync(
            string id, string empresa, string nombre, string edad)
        {
            Cliente cliente = new Cliente();
            cliente.IdCliente = id;
            cliente.Empresa = empresa;
            cliente.Nombre = nombre;
            cliente.Edad = edad;
            //Las consultas de acción se realizan mediante objetos de
            //tipo TableOperation y son ejecutadas posteriormente 
            //sobre cada tabla
            TableOperation insert = TableOperation.Insert(cliente);
            await this.tablaClientes.ExecuteAsync(insert);
        }

        //Método para devolver todos los clientes
        public async Task<List<Cliente>> GetClientesAsync()
        {
            //Para buscar/recuperar elemento de la tabla debemos 
            //utilizar un objeto TableQuery<T>
            TableQuery<Cliente> query = new TableQuery<Cliente>();
            //La consulta en Storage Tables la realiza por segmentos
            //para ir transformando json en clases
            TableQuerySegment<Cliente> segment =
                await this.tablaClientes.ExecuteQuerySegmentedAsync(query, null);
            List<Cliente> clientes = segment.Results;
            return clientes;
        }

        //Método para buscar por empresa
        public async Task<List<Cliente>> GetClientesEmpresaAsync(string empresa)
        {
            TableQuery<Cliente> query = new TableQuery<Cliente>();
            TableQuerySegment<Cliente> segment =
                await this.tablaClientes.ExecuteQuerySegmentedAsync(query, null);
            //Filtramos los campos que necesitemos
            List<Cliente> clientes = segment.Where(x => x.Empresa == empresa).ToList();
            return clientes;

        }

        //Método para buscar por RowKey
        //En Tables no podemos buscar solo por RK, debemos hacerlo en
        //conjunto con su PartitionKey para devolver una sola fila
        public async Task<Cliente> FindClienteAsync(
            string rowkey, string partitionkey)
        {
            TableOperation select =
                TableOperation.Retrieve<Cliente>(partitionkey, rowkey);
            TableResult result =
                await this.tablaClientes.ExecuteAsync(select);
            Cliente cliente = result.Result as Cliente;
            return cliente;
        }

        public async Task DeleteClienteAsync(string rowkey, string partitionkey)
        {
            Cliente cliente = await this.FindClienteAsync(rowkey, partitionkey);
            TableOperation delete = TableOperation.Delete(cliente);
            await this.tablaClientes.ExecuteAsync(delete);
        }

        public async Task UpdateClienteAsync(
            string rowkey, string partitionkey, string nombre, string edad)
        {
            Cliente cliente = await this.FindClienteAsync(rowkey, partitionkey);
            cliente.Nombre = nombre;
            cliente.Edad = edad;
            TableOperation update = TableOperation.Replace(cliente);
            await this.tablaClientes.ExecuteAsync(update);
        }
    }
}
