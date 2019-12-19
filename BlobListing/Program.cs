using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BlobListing
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This utility will get a list of all blobs in a storage account or container");
            Console.WriteLine("");
            Console.WriteLine("Please provide the connection string for the storage account:");

            var connectionString = Console.ReadLine();

            Console.WriteLine("If you want a specific container, please provide it now, if you want the entire storage account, leave blank:");
            var containerName = Console.ReadLine();

            var blobLister = new BlobLister(connectionString);

            var list = new List<string>();
            if (String.IsNullOrEmpty(containerName))
            {
                var response = blobLister.GetBlobs();
                response.Wait();
                list = response.Result;
            }
            else
            {
                var response = blobLister.GetBlobs(containerName);
                response.Wait();
                list = response.Result;
            }

            foreach (var entry in list)
            {
                Console.WriteLine(entry);
            }

            Console.WriteLine("");
            Console.WriteLine("Would you like to write all lines to a text file?  If yes please give the path (including file name) if no, leave blank");
            var path = Console.ReadLine();

            if (!String.IsNullOrEmpty(path))
            {
                if (File.Exists(path))
                {
                    Console.WriteLine("This file exists, do you want to delete it? Y / N (case sensitive)");
                    var answer = Console.ReadLine();
                    if (answer == "Y")
                    {
                        File.Delete(path);
                    }
                }

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
                {
                    foreach (string line in list)
                    {
                        file.WriteLine(line);
                    }
                }

                Console.WriteLine("");
                Console.WriteLine(string.Format("All lines written to file {0}", path));
            }

            Console.ReadLine();
        }
    }
}
