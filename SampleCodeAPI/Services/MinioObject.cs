using Minio;

namespace SampleCodeAPI.Services
{
    public class MinioObject
    {
        private readonly IConfiguration _configuration;
        private readonly MinioClient _minio;
        public MinioObject(IConfiguration configuration)
        {
            _configuration = configuration;
            _minio = new MinioClient(_configuration["MinioConfig:MinioServer"], _configuration["MinioConfig:MinioAccessKey"],
                 _configuration["MinioConfig:MinioSecretKey"]

                ).WithSSL();

        }
        public async Task<string> GetUrl_CDN(string bucketID, string objectname)
        {
            return await _minio.PresignedGetObjectAsync(bucketID, objectname, 5 * 60).ConfigureAwait(false);
        }
    }
}
