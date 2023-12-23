using System.Text.Json.Serialization;

namespace CPG.Domain.SharedKernel.Communication.Ipg.AsanPardakht;

public class AsanPardakhtResponseBase : ResponseBase
{
    [JsonPropertyName("error")]
    public Error ErrorResult { get; set; }

    public class Error
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        //[JsonPropertyName("args")]
        //public Args Args { get; set; }
    }
}
