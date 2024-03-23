using Chat.Application.Features.Like.Command;
using Chat.Application.Helpers.PaginationLikes;
using Chat.Application.Helpers.Paginations;
using Chat.Domain.Entities;

namespace Chat.Application.Presistance.Contracts
{
    public interface ILikeRepository
    {
        Task<bool> AddLike(string LikedUserId, string sourceUserId);
        Task<UserLike> GetUserLike(string sourceUserId, string likedUSerId);
        
        Task<Pagination<LikeDto>> GetUsersLikes(LikesParams likesParams,string userId);
    }
}
