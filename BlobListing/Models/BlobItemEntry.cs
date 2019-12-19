using System;
using System.Collections.Generic;
using System.Text;

namespace BlobListing
{
    public class BlobItemEntry
    {
        public string StorageAccountName { get; set; }
        public string ContainerName { get; set; }
        public string BlobName { get; set; }
        public double Size { get; set; }
    }
}
