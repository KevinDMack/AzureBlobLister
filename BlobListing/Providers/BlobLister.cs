using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlobListing
{
    public class BlobLister
    {
        private string _connectionString;

        private StorageCredentials _credentials;
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _client;

        public BlobLister(string connectionString)
        {
            _connectionString = connectionString;
            
            _storageAccount = CloudStorageAccount.Parse(connectionString);

            _client = _storageAccount.CreateCloudBlobClient();
        }
        
        public async Task<List<string>> GetBlobs()
        {
            List<string> blobs = new List<string>();

            BlobContinuationToken token = null;
            List<CloudBlobContainer> containers = new List<CloudBlobContainer>();

            do
            {
                var response = await _client.ListContainersSegmentedAsync(token);
                token = response.ContinuationToken;
                containers.AddRange(response.Results);
            }
            while (token != null);

            foreach (var container in containers)
            {
                var containerBlobs = await this.GetBlobs(container);
                blobs.AddRange(containerBlobs);
            }

            return blobs;
        }

        public async Task<List<string>> GetBlobs(string containerName)
        {
            var container = _client.GetContainerReference(containerName);
            return await this.GetBlobs(container);
        }
        public async Task<List<string>> GetBlobs(CloudBlobContainer container)
        {
            var blobs = new List<IListBlobItem>();
            
            BlobContinuationToken token = null;

            do
            {
                var response = await container.ListBlobsSegmentedAsync(string.Empty,
                true, BlobListingDetails.Metadata, null, token, null, null);
                token = response.ContinuationToken;
                blobs.AddRange(response.Results);
            }
            while (token != null);

            var returnList = new List<string>();

            foreach (var blobItem in blobs)
            {
                var blob = (CloudBlockBlob)blobItem;
                var entry = string.Format("container\\Blob: {0}\\{1}, blob size: {2}", blob.Container.Name, blob.Name,blob.Properties.Length);
                returnList.Add(entry);
            }

            return returnList;
        }
    }
}
