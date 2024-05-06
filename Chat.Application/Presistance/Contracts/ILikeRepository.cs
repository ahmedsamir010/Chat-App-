namespace Chat.Application.Presistance.Contracts
{
    public interface ILikeRepository
    {
        Task<bool> AddLike(string LikedUserId, string sourceUserId);
        Task<UserLike> GetUserLike(string sourceUserId, string likedUSerId);
        
        Task<Pagination<LikeDto>> GetUsersLikes(LikesParams likesParams,string userId);
    }
}
