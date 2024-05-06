namespace Chat.Application.Presistance.Contracts
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(AppUser appUser); 
    }
}
