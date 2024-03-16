using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Response
{
    public class BaseCommonResponse
    {
        public int Id { get; set; }
        public string Message { get; set; } = default!;
        public object Data { get; set; }
        public string PhotoUrl { get; set; } = default!;
        public List<string> Errors { get; set; } = default!;
        public bool Statues { get; set; }
        public bool IsSuccess { get; set; }
        public ResponseStatus responseStatus { get; set; } 

    }
    public enum ResponseStatus
    {
        Success,
        Unauthorized,
        NotFound,
        BadRequest, 
    }

}
