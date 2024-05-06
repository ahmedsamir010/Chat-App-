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
        public object Data { get; set; } = default!;
        public string PhotoUrl { get; set; } = default!;
        public List<string> Errors { get; set; } = default!;
        public bool Statues { get; set; }
        public bool IsSuccess { get; set; }
        public string  Token { get; set; } = default!;
        public ResponseStatus ResponseStatus { get; set; } 

    }
    public enum ResponseStatus
    {
        Success,
        Unauthorized,
        NotFound,
        BadRequest, 
        NotActivate,
        IsBlocked
    }

}
