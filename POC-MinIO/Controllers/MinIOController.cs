using System.Net;

using Microsoft.AspNetCore.Mvc;

using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using Minio.DataModel.Result;
using Minio.Exceptions;

namespace POC_MinIO.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MinIOController(ILogger<MinIOController> logger, IMinioClient minioClient) : Controller
    {
        [HttpGet]
        [ProducesResponseType(typeof(ListAllMyBucketsResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListBuckets()
        {
            var result = await minioClient.ListBucketsAsync()
                                          .ConfigureAwait(false);

            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ObjectStat), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckObjectStatus(string bucketID, string objectName)
        {
            logger.LogInformation(message: "Finding object name '{objectName}' in bucket '{bucketID}'", objectName, bucketID);

            var args = new StatObjectArgs().WithBucket(bucketID)
                                           .WithObject(objectName);

            try
            {
                minioClient.SetTraceOn();

                var result = await minioClient.StatObjectAsync(args)
                                              .ConfigureAwait(false);

                return Ok(result);
            }
            catch (Exception exception) when (exception is BucketNotFoundException || exception is ObjectNotFoundException)
            {
                return NotFound(objectName);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetObjectContent(string bucketID, string objectName)
        {
            logger.LogInformation(message: "Finding object name '{objectName}' in bucket '{bucketID}'", objectName, bucketID);

            async void WriteAsTextAsync(Stream stream)
            {
                using (var streamReader = new StreamReader(stream))
                {
                    Response.ContentType = "text/plain";
                    Response.StatusCode = (int)HttpStatusCode.OK;

                    await Response.WriteAsync($"Result from stream is '{streamReader.ReadToEnd()}'");
                }
            }

            var args = new GetObjectArgs().WithBucket(bucketID)
                                          .WithObject(objectName)
                                          .WithCallbackStream(WriteAsTextAsync);

            try
            {
                var result = await minioClient.GetObjectAsync(args)
                                              .ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is BucketNotFoundException || exception is ObjectNotFoundException)
            {
                return NotFound(objectName);
            }

            return Empty;
        }

        [HttpGet]
        [ProducesResponseType(typeof(VirtualFileResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> DownloadAsFile(string bucketID, string objectName)
        {
            logger.LogInformation(message: "Finding object name '{objectName}' in bucket '{bucketID}'", objectName, bucketID);

            var tempFilePath = Path.Combine("Temps", objectName);
            var args = new GetObjectArgs().WithBucket(bucketID)
                                          .WithObject(objectName)
                                          .WithFile(tempFilePath); // Note: this will save as a physical file.
            try
            {
                var result = await minioClient.GetObjectAsync(args)
                                              .ConfigureAwait(false);

                return File(System.IO.File.ReadAllBytes(tempFilePath), result.ContentType, result.ObjectName);
            }
            catch (Exception exception) when (exception is BucketNotFoundException || exception is ObjectNotFoundException)
            {
                return NotFound(objectName);
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(PutObjectResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> PutToBucket(string bucketID, IFormFile file)
        {
            logger.LogInformation(message: "Putting file name '{fileName}' to bucket '{bucketID}'", file.FileName, bucketID);

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            using (var stream = file.OpenReadStream())
            {
                var args = new PutObjectArgs().WithBucket(bucketID)
                                              .WithObject(file.FileName)
                                              .WithObjectSize(file.Length)
                                              .WithContentType(file.ContentType)
                                              .WithStreamData(stream);

                var result = await minioClient.PutObjectAsync(args)
                                              .ConfigureAwait(false);

                return Ok(result);
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> CreateBucket(string bucketID)
        {
            logger.LogInformation(message: "Creating bucket '{bucketID}'", bucketID);

            var args = new MakeBucketArgs().WithBucket(bucketID);

            await minioClient.MakeBucketAsync(args)
                             .ConfigureAwait(false);

            return NoContent();
        }

        [HttpGet]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> BucketExists(string bucketID)
        {
            logger.LogInformation(message: "Creating bucket '{bucketID}'", bucketID);

            var args = new BucketExistsArgs().WithBucket(bucketID);

            var result = await minioClient.BucketExistsAsync(args)
                                          .ConfigureAwait(false);

            return Ok(result);
        }
    }
}
