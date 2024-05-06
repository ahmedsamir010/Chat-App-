namespace Chat.Application.Features.Post.Query.GetPostById
{
    public class GetPostByIdQuery(int id) : IRequest<ReturnPostDto?>
    {
        private readonly int _id = id;
        public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetPostByIdQuery, ReturnPostDto?>
        {
            private readonly IUnitOfWork _unitOfWork = unitOfWork;
            private readonly IMapper _mapper = mapper;
            public async Task<ReturnPostDto?> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
            {
                var postsUser = await _unitOfWork.Repository<Domain.Entities.Post>().GetFirstOrDefaultAsync(x => x.Id == request._id);
                if (postsUser is not null)
                {
                    var mappedPost = _mapper.Map<ReturnPostDto>(postsUser);
                    mappedPost!.PictureUrl = "https://localhost:7171/" + postsUser.PictureUrl;
                    return mappedPost;
                }
                return null;
            }
        }
    }
}
