using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace SampletS3Test.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private IConfiguration Configuration;
        public FileUploadController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        [Route("api/uploadFile")]
        [HttpPost]
        public ActionResult UploadFile(string filePath)
        {
            // Specify your bucket region (an example region is shown).  
            string bucketName = Configuration.GetValue<string>("BucketName");
            RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
            string accesskey = Configuration.GetValue<string>("AWSAccessKey");
            string secretkey = Configuration.GetValue<string>("AWSSecretKey");

            var s3Client = new AmazonS3Client(accesskey, secretkey, bucketRegion);

            var fileTransferUtility = new TransferUtility(s3Client);
            try
            {
                if (filePath.Length > 0)
                {
                   // var filePath = Path.Combine(Server.MapPath("~/Files"), Path.GetFileName(file.FileName));
                    var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        FilePath = filePath,
                        StorageClass = S3StorageClass.StandardInfrequentAccess,
                        PartSize = 6291456, // 6 MB.  
                        Key = "SampleTest",
                        CannedACL = S3CannedACL.PublicRead
                    };
                    fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                    fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
                    fileTransferUtility.Upload(fileTransferUtilityRequest);
                    fileTransferUtility.Dispose();
                }
                //ViewBag.Message = "File Uploaded Successfully!!";
            }

            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    //ViewBag.Message = "Check the provided AWS Credentials.";
                }
                else
                {
                   // ViewBag.Message = "Error occurred: " + amazonS3Exception.Message;
                }
            }
            return RedirectToAction("S3Sample");
        }
    }
}