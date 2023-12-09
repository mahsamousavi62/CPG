namespace CPG.Domain.SharedKernel.ApplicationSettings;

    public class ApplicationConfigViewModel
    {
        public string Authority { get; set; }
        public string ClientApiKey { get; set; }
        public string ClientApiSecret { get; set; }
        public string ServerApiKey { get; set; }
        public string ServerApiSecret { get; set; }
        public string ServerScope { get; set; }
        public string ClientId { get; set; }
        public int ClockSkew { get; set; }
        public bool ValidateIssuer { get; set; }
        public bool ValidateAudience { get; set; }
        public bool ValidateLifetime { get; set; }
        public string IdpGetProfileUrl { get; set; }
        public int ExpireTime { get; set; }
        public string Minio_AccessKey { get; set; }
        public string Minio_SecretKey { get; set; }
        public bool Minio_WithSSL { get; set; }
        public string Minio_EndPoint { get; set; }

    }

