using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PostLookupController : ControllerBase
    {
        private readonly ILogger<PostLookupController> _logger;
        private readonly IQueryDispatcher<PostEntity> _queryDispatcher;

        public PostLookupController(ILogger<PostLookupController> logger, IQueryDispatcher<PostEntity> queryDispatcher)
        {
            _logger = logger;
            _queryDispatcher = queryDispatcher;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllPostsAsync() 
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync(new FindAllPostsQuery());

                return SuccessResponse(posts);
            }
            catch (Exception ex)
            {
                var safeErrorMessage = "Error while processing request to retrieve all posts";
                return ErrorResponse(ex, safeErrorMessage);
            }
        }

        [HttpGet("byId/{postId}")]
        public async Task<ActionResult> GetByPostIdAsync(Guid postId)
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync(new FindPostByIdQuery { Id = postId});

                if (posts == null || !posts.Any())
                    return NoContent();

                return Ok(new PostLookupResponse
                {
                    Posts = posts,
                    Message = "Successfully returned post"
                });
            }
            catch (Exception ex)
            {
                var safeErrorMessage = "Error while processing request to find posts by id";
                return ErrorResponse(ex, safeErrorMessage);
            }
        }

        [HttpGet("byAuthor/{author}")]
        public async Task<ActionResult> GetPostsByAuthorAsync(string author)
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync(new FindPostsByAuthorQuery { Author = author });

                return SuccessResponse(posts);
            }
            catch (Exception ex)
            {
                var safeErrorMessage = "Error while processing request to find posts by author";
                return ErrorResponse(ex, safeErrorMessage);
            }
        }

        [HttpGet("withComments")]
        public async Task<ActionResult> GetPostsWithCommentsAsync()
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync(new FindPostsWithCommentsQuery());

                return SuccessResponse(posts);
            }
            catch (Exception ex)
            {
                var safeErrorMessage = "Error while processing request to find posts with comments";
                return ErrorResponse(ex, safeErrorMessage);
            }
        }

        [HttpGet("withLikes")]
        public async Task<ActionResult> GetPostsWithLikesAsync(int numberOfLikes)
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync(new FindPostsWithLikesQuery { NumberOfLikes = numberOfLikes });

                return SuccessResponse(posts);
            }
            catch (Exception ex)
            {
                var safeErrorMessage = "Error while processing request to find posts with likes";
                return ErrorResponse(ex, safeErrorMessage);
            }
        }

        private ActionResult ErrorResponse(Exception ex, string safeErrorMessage)
        {
            _logger.LogError(ex, safeErrorMessage);

            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
            {
                Message = safeErrorMessage
            });
        }

        private ActionResult SuccessResponse(List<PostEntity> posts)
        {
            if (posts == null || !posts.Any())
                return NoContent();

            return Ok(new PostLookupResponse
            {
                Posts = posts,
                Message = $"Successfully returned {posts.Count} post {(posts.Count > 1 ? "s" : String.Empty)}"
            });
        }


    }
}
