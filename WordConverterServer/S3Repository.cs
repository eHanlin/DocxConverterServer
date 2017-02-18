using System;
using System.Configuration;
using System.Linq;
using System.Net;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace WordConverterServer
{
    public class S3Repository
    {
        private readonly AmazonS3Client _client;

        public S3Repository()
        {
            _client = new AmazonS3Client(ConfigurationManager.AppSettings["AWS_ACCESS_KEY"],ConfigurationManager.AppSettings["AWS_SECRET_KEY"],RegionEndpoint.GetBySystemName(ConfigurationManager.AppSettings["AWS_END_POINT"]));
        }

        public string UploadFile(string filePath,string taskid)
        {
            string bucket = ConfigurationManager.AppSettings["AWS_S3_Bucket"];
            string returnUrl = $"s3-{ConfigurationManager.AppSettings["AWS_END_POINT"]}.amazonaws.com/{bucket}/";
            string key = $"Words/{taskid}/{filePath.Split('\\').Last()}";
            PutObjectRequest request = new PutObjectRequest()
            {
                BucketName = bucket,
                CannedACL = S3CannedACL.PublicReadWrite,
                FilePath = filePath,
                Key = key
            };
            PutObjectResponse response = RetryHelper.Do(()=>_client.PutObject(request),TimeSpan.FromMinutes(1));
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                returnUrl = $"{returnUrl}{key}";
            }
            return Amazon.S3.Util.AmazonS3Util.UrlEncode(returnUrl,false);
        }
    }
}