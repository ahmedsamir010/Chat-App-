using Chat.Application.Features.Like.Command;
using Chat.Application.Helpers.PaginationLikes;
using Chat.Domain.Entities;
using Chat.Infrastructe.Helpers;

namespace Chat.Application.Presistance.Contracts
{
    public interface ILikeRepository
    {

        Task<bool> AddLike(string LikedUserId, string sourceUserId);
        Task<UserLike> GetUserLike(string sourceUserId, string likedUSerId);
        
        Task<Pagination<LikeDto>> GetUsersLikes(LikesParams likesParams);
        Task<AppUser> GetUserWithLike(string userId);

    }
}
