using Chat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Presistance.Contracts
{
    public interface ITokenService
    {
        Task<string> CreateAsync(AppUser appUser); 
    }
}
