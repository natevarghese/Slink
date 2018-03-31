
using System.Threading.Tasks;
using System.Net;

using Amazon.CognitoIdentity;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon;
using Amazon.Auth.AccessControlPolicy.ActionIdentifiers;
using System;
using System.Text;
using Amazon.Runtime.Internal.Auth;
using PCLStorage;

namespace Slink
{
    public class S3Utils
    {
        private static CognitoAWSCredentials cognitoCredentials;
        private static IAmazonS3 s3Client;

        public const HttpStatusCode NO_SUCH_BUCKET_STATUS_CODE = HttpStatusCode.NotFound;
        public const HttpStatusCode BUCKET_ACCESS_FORBIDDEN_STATUS_CODE = HttpStatusCode.Forbidden;
        public const HttpStatusCode BUCKET_REDIRECT_STATUS_CODE = HttpStatusCode.Redirect;


        public static string COGNITO_POOL_ID = "us-east-2:4c744479-921f-4fd0-8b5e-c89323d8ad7d";
        public static string BUCKET_NAME = "slinka";
        public static string APP_CLIENT_ID = "1h7e5kg2ir8c8j7sfoft22poqv";
        public static string APP_CLIENT_SECRET = "a6qhbbejsnis106rk2gsiqa0dm4soaoa020ngeif67eg8jeukgm";

        public static RegionEndpoint REGION = RegionEndpoint.USEast2;
        public static DateTime DefaultExpiry = DateTime.Now.AddDays(1);

        public static CognitoAWSCredentials Credentials
        {
            get
            {
                if (cognitoCredentials == null)
                {
                    cognitoCredentials = new CognitoAWSCredentials(COGNITO_POOL_ID, REGION);
                }
                return cognitoCredentials;
            }
        }

        public static IAmazonS3 S3Client
        {
            get
            {
                if (s3Client == null)
                {
                    s3Client = new AmazonS3Client(Credentials, REGION);
                }
                return s3Client;
            }
        }

        public static async Task<bool> BucketExist()
        {
            try
            {
                await S3Client.ListObjectsAsync(new ListObjectsRequest()
                {
                    BucketName = BUCKET_NAME.ToLowerInvariant(),
                    MaxKeys = 0
                });
                return true;
            }
            catch (AmazonS3Exception e)
            {
                if ((e.StatusCode.Equals(BUCKET_REDIRECT_STATUS_CODE)) || e.StatusCode.Equals(BUCKET_ACCESS_FORBIDDEN_STATUS_CODE))
                {
                    //bucket exists if there is a redirect errror or forbidden error
                    return true;
                }
                else if (e.StatusCode.Equals(NO_SUCH_BUCKET_STATUS_CODE))
                {
                    return false;
                }
                else
                {
                    throw e;
                }
            }
        }

        async public static Task UploadFile(string localFilePath, string remoteFilePath, string filename)
        {
            try
            {
                bool bucketExists = await BucketExist();
                if (!bucketExists)
                    await S3Utils.CreateBucket();

                var requestObject = new PutObjectRequest();
                requestObject.BucketName = S3Utils.BUCKET_NAME.ToLowerInvariant();
                requestObject.FilePath = localFilePath;
                requestObject.Key = string.Format("{0}/{1}", remoteFilePath.ToLowerInvariant(), filename);
                requestObject.CannedACL = S3CannedACL.AuthenticatedRead;

                await S3Client.PutObjectAsync(requestObject);

                System.Diagnostics.Debug.WriteLine("File uploaded to S3 Bucket");
            }
            catch (AmazonS3Exception s3Exception)
            {
                System.Diagnostics.Debug.WriteLine("Upload failed. " + s3Exception.Message);
            }
        }
        async public static Task<bool> DownloadFile(string localFilePath, string remoteFilePath, string filename)
        {
            bool responseBody = false;

            var request = new GetObjectRequest();
            request.BucketName = S3Utils.BUCKET_NAME.ToLowerInvariant();
            request.Key = string.Format("{0}/{1}", remoteFilePath.ToLowerInvariant(), filename);

            try
            {
                using (var response = await S3Client.GetObjectAsync(request))
                {
                    await response.WriteResponseStreamToFileAsync(localFilePath, true);
                    responseBody = true;
                }

            }
            catch (AmazonS3Exception)
            {
                responseBody = false;

            }
            return responseBody;
        }
        public static async Task CreateBucket()
        {
            string name = BUCKET_NAME.ToLowerInvariant();

            await S3Client.PutBucketAsync(new PutBucketRequest()
            {
                BucketName = name,
                BucketRegion = S3Region.USE2
            });
        }

        public static async Task DeleteBucket()
        {
            string name = BUCKET_NAME.ToLowerInvariant();
            await S3Client.DeleteBucketAsync(new DeleteBucketRequest()
            {
                BucketName = name,
                BucketRegion = S3Region.USE2
            });
        }
        public static string GetAWSSignatureKey(String key, String dateStamp, String regionName, String serviceName)
        {
            return StringUtils.ToHexString(AWS4Signer.ComposeSigningKey(key, regionName, dateStamp, serviceName)).Trim().ToLower();
        }
        public static string GetPresignedURL(string remoteUrl, string fileName, DateTime expiry)
        {

            string url = String.Empty;
            try
            {
                url = S3Client.GeneratePreSignedURL(BUCKET_NAME + "/" + remoteUrl, fileName, expiry, null);
            }
            catch (System.Net.WebException) { }
            catch (Exception e)
            {
                AppCenterManager.Report(e.Message);

            }
            return url;

        }

        async public static void PredownloadImages()
        {


            //all availbale outlets
            //var availableOutlets = RealmServices.GetAllAvailableOutlets();
            //foreach (AvailableOutlet outlet in availableOutlets)
            //{
            //    var localPath = outlet.LocalURL;
            //    var remotePath = outlet.RemoteURL;
            //    var fileName = outlet.Type.ToLowerInvariant() + ".png";
            //    var sucessful = await S3Utils.DownloadFile(localPath, remotePath, fileName);
            //    if (sucessful)
            //    {

            //    }
            //}

        }

        public static async void UploadPhoto(byte[] buffer, string localUrl, string remoteUrl, string fileName, Action completion, Action failure)
        {
            IFile file = await FileSystemUtils.CreateIFileAtPath(localUrl);
            using (System.IO.Stream stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
            {
                stream.Write(buffer, 0, buffer.Length);
            }

            try
            {
                await S3Utils.UploadFile(file.Path, remoteUrl, fileName);
                var sucessful = await S3Utils.DownloadFile(file.Path, remoteUrl, fileName);

                if (sucessful)
                {
                    completion?.Invoke();
                }
                else
                {
                    failure?.Invoke();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                failure?.Invoke();
            }
        }
    }
}
