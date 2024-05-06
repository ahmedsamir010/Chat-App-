using AutoMapper;
using Chat.Application.Helpers.FileSettings;
using Chat.Application.Presistance;
using Chat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Chat.Application.Features.Post.Command.AddPost
{
    public class AddPostCommand(AddPostDto addPostDto) : IRequest<bool>
    {
        public AddPostDto AddPostDto { get; } = addPostDto;

        public class Handler(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment webHost, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager) : IRequestHandler<AddPostCommand, bool>
        {
            private readonly IUnitOfWork _unitOfWork = unitOfWork;
            private readonly IMapper _mapper = mapper;
            private readonly IWebHostEnvironment _webHost = webHost;
            private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
            private readonly UserManager<AppUser> userManager = userManager;

            public async Task<bool> Handle(AddPostCommand request, CancellationToken cancellationToken)
            {
                var addPost = _mapper.Map<Domain.Entities.Post>(request.AddPostDto);
                if (request.AddPostDto.Picture is not null)
                {
                    var PostPicture = ManageFile.UploadPhoto(_webHost,request.AddPostDto.Picture, "Posts");
                    addPost.PictureUrl = PostPicture;
                }
                var currentUserProvider = new CurrentUserProvider(httpContextAccessor, userManager);
                var currentUser = await currentUserProvider.GetCurrentUserAsync();
                if (currentUser is not null)
                {
                    addPost.User = currentUser;
                }
                var Addresult = await _unitOfWork.Repository<Domain.Entities.Post>().AddAsync(addPost);
                if (Addresult is true)
                    return true;
                return false;
            }
        }
    }
}
