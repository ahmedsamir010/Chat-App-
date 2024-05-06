using AutoMapper;
using Chat.Application.Helpers.FileSettings;
using Chat.Application.Presistance;
using Chat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
namespace Chat.Application.Features.Comment.Command.AddComment
{
    public class AddCommentCommand(AddCommentDto addCommentDto) : IRequest<bool>
    {
        private readonly AddCommentDto _addCommentDto = addCommentDto;

        class Handler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContext, UserManager<AppUser> userManager, IMapper mapper, IWebHostEnvironment webHost) : IRequestHandler<AddCommentCommand, bool>
        {
            private readonly IUnitOfWork _unitOfWork = unitOfWork;
            private readonly IHttpContextAccessor _httpContext = httpContext;
            private readonly UserManager<AppUser> _userManager = userManager;
            private readonly IMapper _mapper = mapper;
            private readonly IWebHostEnvironment _webHost = webHost;
            public async Task<bool> Handle(AddCommentCommand request, CancellationToken cancellationToken)
            {
                var existPost = await _unitOfWork.Repository<Domain.Entities.Post>().GetFirstOrDefaultAsync(x => x.Id == request._addCommentDto.PostId);
                if (existPost is null) { return false; }
                var mappedpost = _mapper.Map<Domain.Entities.Comment>(request._addCommentDto);
                if (request._addCommentDto.Picture is not null)
                {
                    var PostPicture = ManageFile.UploadPhoto(_webHost, request._addCommentDto.Picture, "Comment");
                    mappedpost.PictureUrl = PostPicture;
                }
                var currentUserProvider = new CurrentUserProvider(_httpContext, _userManager);
                var currentUser = await currentUserProvider.GetCurrentUserAsync();
                if (currentUser is not null)
                {
                    mappedpost.User = currentUser;
                    mappedpost.Post = existPost;
                }
                var resultComment = await _unitOfWork.Repository<Domain.Entities.Comment>().AddAsync(mappedpost);
                if (resultComment is true) 
                    return true; 
                return false;

            }
        }


    }
}
