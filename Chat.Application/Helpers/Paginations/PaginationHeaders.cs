using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Helpers.Paginations
{
    public class PaginationHeaders
    {
        public PaginationHeaders(int currentPage, int totalPages, int totalSize, int itemPerPage)
        {
            CurrentPage = currentPage;
            TotalPages = totalPages;
            TotalSize = totalSize;
            ItemPerPage = itemPerPage;
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalSize { get; set; }
        public int ItemPerPage { get; set; }
    }
}
