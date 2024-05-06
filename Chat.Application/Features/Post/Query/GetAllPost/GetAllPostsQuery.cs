using System.Linq.Expressions;
namespace Chat.Application.Features.Post.Query.GetAllPost
{
    public class GetAllPostsQuery : IRequest<IEnumerable<ReturnPostDto?>>
    {
        public class Handler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper, UserManager<AppUser> userManager) : IRequestHandler<GetAllPostsQuery, IEnumerable<ReturnPostDto?>>
        {
            private readonly IUnitOfWork _unitOfWork = unitOfWork;
            private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
            private readonly IMapper _mapper = mapper;
            private readonly UserManager<AppUser> _userManager = userManager;

            public async Task<IEnumerable<ReturnPostDto?>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
            {
                var currentUserProvider = new CurrentUserProvider(_httpContextAccessor, _userManager);
                var currentUser = await currentUserProvider.GetCurrentUserAsync();

                if (currentUser != null)
                {
                    Expression<Func<Domain.Entities.Post, bool>> filter = x => x.User.Id == currentUser.Id;
                    var postsUser = await _unitOfWork.Repository<Domain.Entities.Post>().GetAllWithIncludeAsync(filter, p => p.User);

                    var mappedPosts = _mapper.Map<IEnumerable<ReturnPostDto?>>(postsUser);

                    if (mappedPosts.Any())
                    {
                        foreach (var post in mappedPosts.Where(p => p?.PictureUrl != null))
                        {
                            if (post?.PictureUrl is not null)
                            {
                                post!.PictureUrl = "https://localhost:7171/" + post.PictureUrl;
                            }
                        }
                    }
                    return mappedPosts;
                }

                return null;
            }


        }
    }
}
