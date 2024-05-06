using AutoMapper;
using Chat.Application.Helpers.FileSettings;
using Chat.Application.Presistance;
using Chat.Application.Response;
using Chat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
namespace Chat.Application.Features.Comment.Command.UpdateComment
{
    public class UpdateCommentCommand(UpdateCommentDto updateCommentDto) : IRequest<BaseCommonResponse>
    {
        private readonly UpdateCommentDto _updateCommentDto = updateCommentDto;
        public class Handler(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment webHost, UserManager<AppUser> userManager, IHttpContextAccessor httpContext) : IRequestHandler<UpdateCommentCommand, BaseCommonResponse>
        {
            private readonly IUnitOfWork _unitOfWork = unitOfWork;
            private readonly IMapper _mapper = mapper;
            private readonly IWebHostEnvironment _webHost = webHost;
            private readonly UserManager<AppUser> _userManager = userManager;
            private readonly IHttpContextAccessor _httpContext = httpContext;

            public async Task<BaseCommonResponse> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
            {
                BaseCommonResponse response = new();
                var existingComment= await _unitOfWork.Repository < Domain.Entities.Comment>().GetFirstOrDefaultAsync(x => x.Id == request._updateCommentDto.Id);
                if(existingComment is null)
                {
                    response.ResponseStatus = ResponseStatus.NotFound;
                    response.Message = "Comment not found.";
                    return response;
                }
                var currentUserProvider = new CurrentUserProvider(_httpContext, _userManager);
                var currentUser = await currentUserProvider.GetCurrentUserAsync();
                bool checkCorrectPost = existingComment.User.Id == currentUser?.Id;
                if (!checkCorrectPost)
                {
                    response.ResponseStatus = ResponseStatus.Unauthorized;
                    response.Message = "You are not authorized to update this comment.";
                    return response;
                }
                if (request._updateCommentDto.Picture != null)
                {
                    var newPictureSrc = ManageFile.UploadPhoto(_webHost, request._updateCommentDto.Picture, "Comments");
                    if (existingComment.PictureUrl != null)
                    {
                        ManageFile.RemovePhoto(_webHost, existingComment.PictureUrl);
                    }
                    existingComment.PictureUrl = newPictureSrc;
                }
                existingComment.ModifiedDate = DateTime.Now;
                _mapper.Map(request._updateCommentDto, existingComment);

                await _unitOfWork.Repository<Domain.Entities.Comment>().UpdatedAsync(existingComment);
                await _unitOfWork.CompleteAsync();
                response.ResponseStatus = ResponseStatus.Success;
                response.Message = "Comment updated successfully.";
                return response;
            }
        }

    }
}
