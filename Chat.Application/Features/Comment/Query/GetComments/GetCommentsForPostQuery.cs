using AutoMapper;
using Chat.Application.Presistance;
using MediatR;
namespace Chat.Application.Features.Comment.Query.GetComments
{
    public class GetCommentsForPostQuery(int postId) : IRequest<IEnumerable<CommentsDto>>
    {
        public int PostId { get; } = postId;

        public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetCommentsForPostQuery, IEnumerable<CommentsDto>>
        {
            private readonly IUnitOfWork _unitOfWork = unitOfWork;
            private readonly IMapper _mapper = mapper;

            public async Task<IEnumerable<CommentsDto>> Handle(GetCommentsForPostQuery request, CancellationToken cancellationToken)
            {

                var existPost = await _unitOfWork.Repository<Domain.Entities.Post>().GetFirstOrDefaultAsync(x => x.Id == request.PostId);
                if (existPost is not null)
                {
                    var getComments = await _unitOfWork.Repository<Domain.Entities.Comment>().GetAllWithIncludeAsync(p => p.Post.Id == request.PostId,x=>x.Post,x=>x.User);
                    var mappedComments = _mapper.Map<IEnumerable<CommentsDto?>>(getComments);
                    if (mappedComments.Any())
                    {
                        foreach (var comment in mappedComments.Where(p => p?.PictureUrl != null))
                        {
                            if (comment?.PictureUrl is not null)
                            {
                                comment.PictureUrl = "https://localhost:7171/" + comment.PictureUrl;
                            }
                        }
                    }
                    return mappedComments!;
                }
                return null;
            }
        }
    }
}
