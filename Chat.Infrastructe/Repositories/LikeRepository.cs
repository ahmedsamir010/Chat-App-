using Chat.Application.ExtensionMethods;
using Chat.Application.Features.Like.Command;
using Chat.Application.Helpers.PaginationLikes;
using Chat.Application.Helpers.Paginations;
using Chat.Application.Presistance.Contracts;
using Chat.Domain.Entities;
using Chat.Infrastructe.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Chat.Infrastructe.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LikeRepository(ApplicationDbContext dbContext, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> AddLike(string LikedUserId, string sourceUserId)
        {
            UserLike userLike = new()
            {
                SourceUserId = sourceUserId,
                LikedUserId = LikedUserId,
            };
            await _dbContext.userLikes.AddAsync(userLike);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<UserLike> GetUserLike(string sourceUserId, string likedUSerId)
                => await _dbContext.userLikes.FindAsync(sourceUserId, likedUSerId);

        public async Task<Pagination<LikeDto>> GetUsersLikes(LikesParams likesParams)
        {
            IQueryable<AppUser> users = _dbContext.Users.Include(x => x.Photos).OrderBy(x => x.UserName);
            IQueryable<UserLike> likes = _dbContext.userLikes.AsQueryable();

            if (likesParams.Predicate?.ToLower() == "liked")
            {
                likes = likes.Where(x => x.SourceUserId == likesParams.UserId);
                users = likes.Select(x => x.LikedUser);
            }
            else if (likesParams.Predicate?.ToLower() == "likedby")
            {
                likes = likes.Where(x => x.LikedUserId == likesParams.UserId);
                users = likes.Select(x => x.SourceUser);
            }
            var likeDtos = users.Select(x => new LikeDto
            {
                Id = x.Id,
                Age = x.DateOfBirth.CalculateAge(),
                firstName=x.FirstName,
                lastName = x.LastName,
                photoUrl = x.Photos.FirstOrDefault(x=>x.IsMain)!.Url
            });

            return Pagination<LikeDto>.Create(likeDtos, likesParams.PageNumber, likesParams.PageSize);
        }
       
        public async Task<AppUser> GetUserWithLike(string userId)
        {
            var user = await _dbContext.Users
                .Include(x => x.Likeduser)
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (user is not null) return user;
            else return null;
        }

    }
}
