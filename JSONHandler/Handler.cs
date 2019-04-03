using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using static JSONHandler.Model;

namespace JSONHandler
{
    public static class Handler
    {
        [FunctionName("Handler")]
        public static async System.Threading.Tasks.Task RunAsync([QueueTrigger("transformed", Connection = "storageconnectionstring")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            CraneEntity entity = JsonConvert.DeserializeObject<CraneEntity>(myQueueItem);
            entity.json = myQueueItem;

            Console.WriteLine(entity.json);

            /*CraneEntity entity = new CraneEntity()
            {
                GeoPositionLongitude = "",
                GeoPositionLatitude = "6 * 3",
                MachineSerialNumber = "", 
                CraneID = ""
            };
            */

            //Get Connectionstring from app settings
            string storageConnectionString = Environment.GetEnvironmentVariable("storageconnectionstring");

            // Retrieve storage account information from connection string.
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(storageConnectionString);

            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            Console.WriteLine("Create a Table for the demo");

            // Create a table client for interacting with the table service 
            CloudTable table = tableClient.GetTableReference("transformed");
            if (await table.CreateIfNotExistsAsync())
            {
                Console.WriteLine("Created Table named: {0}", "transformed");
            }
            else
            {
                Console.WriteLine("Table {0} already exists", "transformed");
            }

            InsertOrMergeEntityAsync(table, entity);
            
        }
        public static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }
        private static async void InsertOrMergeEntityAsync(CloudTable table, CraneEntity entity)
        {
            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                CraneEntity insertedCrane = result.Result as CraneEntity;

            }
            catch (StorageException)
            {
                                
            }
        }
    }
}
