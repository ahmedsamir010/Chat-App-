using Chat.Application.Helpers.FileSettings;
using Chat.Application.Presistance;
using Chat.Application.Response;
using Chat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Application.Features.Post.Command.DeletePost
{
    public class DeletePostCommand(int id) : IRequest<BaseCommonResponse>
    {
        public int Id { get; } = id;

        public class Handler(IUnitOfWork unitOfWork,IHttpContextAccessor httpContextAccessor,UserManager<AppUser> userManager,IWebHostEnvironment webHost) : IRequestHandler<DeletePostCommand, BaseCommonResponse>
        {
            private readonly IUnitOfWork _unitOfWork = unitOfWork;
            private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
            private readonly UserManager<AppUser> _userManager = userManager;
            private readonly IWebHostEnvironment _webHost = webHost;

            public async Task<BaseCommonResponse> Handle(DeletePostCommand request, CancellationToken cancellationToken)
            {
                BaseCommonResponse Response = new();
                var userPost = await _unitOfWork.Repository<Domain.Entities.Post>().GetFirstOrDefaultAsync(x => x.Id == request.Id);

                if (userPost is null)
                {
                    Response.ResponseStatus = ResponseStatus.NotFound;
                    Response.Message = "Post not found.";
                    return Response;
                }
                var currentUserProvider = new CurrentUserProvider(_httpContextAccessor, _userManager);
                var currentUser = await currentUserProvider.GetCurrentUserAsync();

                if (userPost.User.Id != currentUser?.Id)
                {
                    Response.ResponseStatus = ResponseStatus.Unauthorized;
                    Response.Message = "You are not authorized to Delete this post.";
                    return Response;
                }

                // Delete associated picture
                if (!string.IsNullOrEmpty(userPost.PictureUrl))
                {
                    ManageFile.RemovePhoto(_webHost, userPost.PictureUrl);
                }

                await _unitOfWork.Repository<Domain.Entities.Post>().DeleteAsync(userPost.Id);
                await _unitOfWork.CompleteAsync();

                Response.ResponseStatus = ResponseStatus.Success;
                Response.Message = "Post Deleted successfully.";
                return Response;
            }
        }
    }
}
