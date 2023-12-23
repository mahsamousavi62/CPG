namespace CPG.Domain.SharedKernel.ApplicationSettings;

public class MinioConfigViewModel
{
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public bool WithSSL { get; set; }
    public string EndPoint { get; set; }
    public string BucketName { get; set; }
}
