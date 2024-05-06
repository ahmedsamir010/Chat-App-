namespace Chat.Application.Presistance.Contracts
{
    public interface IUserRepository
    {
        Task UpdateUser(AppUser user);
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<Pagination<MemberDto>> GetMembersAsync(UserParams userParams);
        Task<AppUser?> GetUserByIdAsync(string id);  
        Task<AppUser?> GetUserByNameAsync(string userName);
        Task<AppUser?> GetUserByEmail(string email);
        Task<bool> UploadFile(IFormFile file,string pathName);
        Task<bool> RemoveFile(int id);
        Task<bool> SetMainPhoto(int id);

    }
}
