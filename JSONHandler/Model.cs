using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace JSONHandler
{
    public static class Model
    {


        public class CraneEntity : TableEntity
        {

            public CraneEntity()
            {
                PartitionKey = "Crane";
                RowKey = Guid.NewGuid().ToString();
            }

            public string GeoPositionLongitude { get; set; }
            public string GeoPositionLatitude { get; set; }
            public string CraneID { get; set; }
            public string MachineSerialNumber { get; set; }
            public string json { get; set; }

        }
    }
}
