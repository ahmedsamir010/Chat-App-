using AutoMapper;
using Chat.Application.Helpers.FileSettings;
using Chat.Application.Presistance;
using Chat.Application.Response;
using Chat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
namespace Chat.Application.Features.Post.Command.UpdatePost
{
    public class UpdatePostCommand(UpdatePostDto updatePostDto) : IRequest<BaseCommonResponse>
    {
        public UpdatePostDto UpdatePostDto { get; } = updatePostDto;

        public class Handler(IUnitOfWork unitOfWork, IMapper mapper,IWebHostEnvironment webHost,UserManager<AppUser> userManager,IHttpContextAccessor httpContext) : IRequestHandler<UpdatePostCommand, BaseCommonResponse>
        {
            private readonly IUnitOfWork _unitOfWork = unitOfWork;
            private readonly IMapper _mapper = mapper;
            private readonly IWebHostEnvironment _webHost = webHost;
            private readonly UserManager<AppUser> _userManager = userManager;
            private readonly IHttpContextAccessor _httpContext = httpContext;
            public async Task<BaseCommonResponse> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
            {
                BaseCommonResponse response = new BaseCommonResponse();
                var existingPost = await _unitOfWork.Repository<Domain.Entities.Post>().GetFirstOrDefaultAsync(x => x.Id == request.UpdatePostDto.Id);
                var currentUserProvider = new CurrentUserProvider(_httpContext, _userManager);
                var currentUser = await currentUserProvider.GetCurrentUserAsync();

                if (existingPost != null)
                {
                    bool checkCorrectPost = existingPost.User.Id == currentUser?.Id;
                    if (!checkCorrectPost)
                    {
                        response.ResponseStatus = ResponseStatus.Unauthorized;
                        response.Message = "You are not authorized to update this post.";
                        return response;
                    }

                    if (request.UpdatePostDto.Picture != null)
                    {
                        var newPictureSrc = ManageFile.UploadPhoto(_webHost, request.UpdatePostDto.Picture, "Posts");
                        if (existingPost.PictureUrl != null)
                        {
                            ManageFile.RemovePhoto(_webHost, existingPost.PictureUrl);
                        }
                        existingPost.PictureUrl = newPictureSrc;
                    }

                    existingPost.ModifiedDate = DateTime.Now;
                    _mapper.Map(request.UpdatePostDto, existingPost);

                    await _unitOfWork.Repository<Domain.Entities.Post>().UpdatedAsync(existingPost);
                    await _unitOfWork.CompleteAsync();

                    response.ResponseStatus = ResponseStatus.Success;
                    response.Message = "Post updated successfully.";
                    return response;
                }

                response.ResponseStatus = ResponseStatus.NotFound;
                response.Message = "Post not found.";
                return response;
            }

        }
    }
}
