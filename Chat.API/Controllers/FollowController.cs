﻿using Chat.Application.Features.Like.Command;
using Chat.Application.Features.Like.Query;
using Chat.Application.Helpers.PaginationLikes;
namespace Chat.API.Controllers
{
    public class FollowController(IMediator mediator) : BaseController(mediator)
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Adds a follow for the specified user.
        /// </summary>
        /// <param name="userName">The username of the user to be follow.</param>
        /// <returns>Returns an HTTP response indicating the result of the like operation.</returns>
        [HttpPost("Add-follow/{userName}")]
        public async Task<IActionResult> AddLike(string userName)
        {
            if (userName is not null)
            {
                var command = new AddLikeCommand(userName);
                var response = await _mediator.Send(command);
                if (response is not null)
                {
                    return Ok(response.Message);
                }
                else
                {
                    return BadRequest("Invalid Request");
                }
            }
            return NotFound("UserName Not Found");
        }
        /// <summary>
        /// Retrieves a paginated list of users who have been followed by the current user.
        /// </summary>
        /// <param name="likesParams">Parameters for pagination and filtering.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Returns an HTTP response containing a paginated list of followed users.</returns>
        [HttpGet("get-uesers-follow")]
        public async Task<ActionResult<Pagination<LikeDto>>> GetUsersLike([FromQuery]LikesParams likesParams, CancellationToken ct)
        {
            var query = await _mediator.Send(new GetUsersLikeQuery(likesParams), ct);
            if (query is not null)
            {
                Response.AddPaginationHeaders(query.CurrentPage, query.TotalPages, query.Count, query.PageSize);
                return Ok(query);
            }
            return NotFound("No data found.");
        }
    }
}
