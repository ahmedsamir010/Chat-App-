namespace Chat.Application.Features.Comment.Command.DeleteComment
{
    public class DeleteCommentCommand(int id) : IRequest<BaseCommonResponse>
    {
        public int Id { get; } = id;
        public class Handler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IWebHostEnvironment webHost) : IRequestHandler<DeleteCommentCommand, BaseCommonResponse>
        {
            private readonly IUnitOfWork _unitOfWork = unitOfWork;
            private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
            private readonly UserManager<AppUser> _userManager = userManager;
            private readonly IWebHostEnvironment _webHost = webHost;
            public async Task<BaseCommonResponse> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
            {
                BaseCommonResponse Response = new();
                var userComment = await _unitOfWork.Repository<Domain.Entities.Comment>().GetFirstOrDefaultAsync(x => x.Id == request.Id);

                if (userComment is null)
                {
                    Response.ResponseStatus = ResponseStatus.NotFound;
                    Response.Message = "Comment not found.";
                    return Response;
                }
                var currentUserProvider = new CurrentUserProvider(_httpContextAccessor, _userManager);
                var currentUser = await currentUserProvider.GetCurrentUserAsync();

                if (userComment.User.Id != currentUser?.Id)
                {
                    Response.ResponseStatus = ResponseStatus.Unauthorized;
                    Response.Message = "You are not authorized to Delete this post.";
                    return Response;
                }

                if (!string.IsNullOrEmpty(userComment.PictureUrl))
                {
                    ManageFile.RemovePhoto(_webHost, userComment.PictureUrl);
                }

                await _unitOfWork.Repository<Domain.Entities.Comment>().DeleteAsync(userComment.Id);
                await _unitOfWork.CompleteAsync();

                Response.ResponseStatus = ResponseStatus.Success;
                Response.Message = "Comment Deleted successfully.";
                return Response;
            }
        }

    }
}
