using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.IO;
using System.Threading.Tasks;


namespace S3_Upload
{
    class UploadFileMPUHighLevelAPITest
    {
        private const string bucketName = "pratik-poc-s3-upload";
        private const string keyName = "*** provide a name for the uploaded object ***";
        private const string filePath = @"C:/My GitHub/Cloud Assignments/S3_Upload/sample.txt";

        private static AmazonS3Config config;
        private static IAmazonS3 s3Client;
        private static TransferUtilityUploadRequest fileTransferUtilityRequest;

        public static void Main()
        {
            Init();
           
            UploadFileAsync().Wait();
        }

        private static void Init()
        {
            config = new AmazonS3Config();
            config.SignatureVersion = "4";
            config.RegionEndpoint = RegionEndpoint.GetBySystemName("ap-south-1");
            config.SignatureMethod = Amazon.Runtime.SigningAlgorithm.HmacSHA256;

            s3Client = new AmazonS3Client("AKIATO6FD7WMPCF3DDXE", "0ZO3uBcyCZD7iB3H2cCGclf+G6298rgbxWTHQg3T", config);

            fileTransferUtilityRequest = new TransferUtilityUploadRequest();
            fileTransferUtilityRequest.BucketName = bucketName;
            fileTransferUtilityRequest.FilePath = filePath;
            fileTransferUtilityRequest.StorageClass = S3StorageClass.StandardInfrequentAccess;
            fileTransferUtilityRequest.PartSize = 6291456; // 6 MB.
            fileTransferUtilityRequest.Key = "uploaded-with-transfer-utility.txt";
            fileTransferUtilityRequest.CannedACL = S3CannedACL.PublicRead;
            fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
            fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
        }
        private static async Task UploadFileAsync()
        {
            try
            {
                var fileTransferUtility = new TransferUtility(s3Client);

                // Option 1. Upload a file. The file name is used as the object key name.
                await fileTransferUtility.UploadAsync(filePath, bucketName);
                Console.WriteLine("Upload 1 completed");

                // Option 2. Specify object key name explicitly.
                await fileTransferUtility.UploadAsync(filePath, bucketName, "renamed_file.txt");
                Console.WriteLine("Upload 2 completed");

                // Option 3. Upload data from a type of System.IO.Stream.
                using (var fileToUpload = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    await fileTransferUtility.UploadAsync(fileToUpload,bucketName,"uploaded_With_permissions.txt");
                
                Console.WriteLine("Upload 3 completed");

                // Option 4. Specify advanced settings.
                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
                Console.WriteLine("Upload 4 completed");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }

        }
    }
}
