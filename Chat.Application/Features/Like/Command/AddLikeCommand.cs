using Chat.Application.Presistance.Contracts;
using Chat.Application.Response;
using Chat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
namespace Chat.Application.Features.Like.Command
{
    public class AddLikeCommand(string userName) : IRequest<BaseCommonResponse>
    {
        public string UserName { get; } = userName;

        public class Handler(ILikeRepository likeRepository, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager) : IRequestHandler<AddLikeCommand, BaseCommonResponse>
        {
            private readonly ILikeRepository _likeRepository = likeRepository;
            private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
            private readonly UserManager<AppUser> _userManager = userManager;

            public async Task<BaseCommonResponse> Handle(AddLikeCommand request, CancellationToken cancellationToken)
            {
                BaseCommonResponse response = new();

                var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return null!;
                }

                var currentUser = await _userManager.FindByIdAsync(userId);
                var likedUser = await _userManager.FindByNameAsync(request.UserName);

                if (!IsValidUsers(currentUser!, likedUser!))
                {
                    response.Statues = false;
                    response.Message = GetInvalidUserMessage(currentUser!, likedUser!);
                    return response;
                }

                if (await _likeRepository.GetUserLike(currentUser!.Id, likedUser!.Id) != null)
                {
                    response.Statues = false;
                    response.Message = "You have already liked this user. Duplicate likes are not allowed.";
                    return response;
                }

                await _likeRepository.AddLike(likedUser.Id.ToString(), currentUser.Id.ToString());

                response.IsSuccess = true;
                response.Message = $"Like added successfully. You now have a connection with {likedUser.UserName}.";
                return response;
            }

            private bool IsValidUsers(AppUser currentUser, AppUser likedUser)
            {
                return currentUser != null && likedUser != null && currentUser.UserName != likedUser.UserName;
            }

            private string GetInvalidUserMessage(AppUser currentUser, AppUser likedUser)
            {
                if (currentUser == null)
                {
                    return "Invalid current user. Please make sure you are logged in.";
                }

                if (likedUser == null)
                {
                    return GetInvalidUserMessage(currentUser, likedUser!);
                }

                return "Cannot like yourself. Please choose another user.";
            }
        }
    }


}
