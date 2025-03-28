﻿namespace Chat.Application.Helpers.Paginations
{
    public class UserParams : PaginationParams
    {     
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 100;
        public string OrderBy { get; set; } = "lastactive";
    }
}
