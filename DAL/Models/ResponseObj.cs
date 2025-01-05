using System.Net;

namespace DAL.Models
{
    public class ResponseObj
    {
        public ResponseObj() { }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}
