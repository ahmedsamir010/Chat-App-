using Chat.Application.Features.Like.Command;
using Chat.Application.Helpers.PaginationLikes;
namespace Chat.Application.Features.Like.Query
{
    public class GetUsersLikeQuery(LikesParams likesParams) : IRequest<Pagination<LikeDto>>
    {
        private readonly LikesParams _likesParams= likesParams;
        class Handler:IRequestHandler<GetUsersLikeQuery,Pagination<LikeDto>>
        {
            private readonly ILikeRepository _likeRepository;
            private readonly UserManager<AppUser> _userManager;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(ILikeRepository likeRepository,UserManager<AppUser> userManager,IHttpContextAccessor httpContextAccessor)
            {
                _likeRepository = likeRepository;
                _userManager = userManager;
                _httpContextAccessor = httpContextAccessor;
            }
            public async Task<Pagination<LikeDto>> Handle(GetUsersLikeQuery request, CancellationToken cancellationToken)
            {

                var userId = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    //var currentUser=await _userManager.FindByIdAsync(userId);
                    var users=await _likeRepository.GetUsersLikes(request._likesParams, userId);
                    if(users is not null)
                    {
                        return users;
                    }
        
                return null;
            }
        }

    }
}
