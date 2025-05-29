using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Comment;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Comment;
using OhBau.Model.Utils;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class CommentService : BaseService<CommentService>, ICommentService
    {
        private readonly IMemoryCache _cache;
        private readonly GenericCacheInvalidator<Comments> _commentCacheINvalidator;
        public CommentService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<CommentService> logger, 
            IMapper mapper, IHttpContextAccessor httpContextAccessor, 
            GenericCacheInvalidator<Comments> commentCacheINvalidator,IMemoryCache cache) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _commentCacheINvalidator = commentCacheINvalidator;
            _cache = cache;
        }

        public async Task<BaseResponse<string>> Comment(Guid accountId,CommentRequest request)
        {
            try
            {
                var createComment = new Comments
                {
                    Id = Guid.NewGuid(),
                    Comment = request.Comment,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = null,
                    DeletedDate = null,
                    BlogId = request.BlogId,
                    AccountId = accountId,
                    isDelete = false,
                    ParentId = null,
                };

                await _unitOfWork.GetRepository<Comments>().InsertAsync(createComment);
                await _unitOfWork.CommitAsync();
                
                _commentCacheINvalidator.InvalidateEntityList();
                _commentCacheINvalidator.InvalidateEntity(createComment.Id);

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Comment success",
                    data = null
                };
            }
            catch (Exception ex) { 
            
                throw new Exception(ex.ToString());

            }
        }

        public async Task<BaseResponse<string>> DeleteComment(Guid accountId, Guid commentId)
        {
            try
            {
                var getComment = await _unitOfWork.GetRepository<Comments>().SingleOrDefaultAsync(
                       predicate: x => x.Id == commentId
                    );

                if (getComment == null)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status404NotFound.ToString(),
                        message = "Comment not found"
                    };
                }

                if (getComment != null && getComment.AccountId != accountId)
                {

                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status403Forbidden.ToString(),
                        message = "You do not have the right to delete other people's comments"
                    };
                }

                getComment.isDelete = true;
                getComment.DeletedDate = DateTime.Now;

                _unitOfWork.GetRepository<Comments>().UpdateAsync(getComment);
                await _unitOfWork.CommitAsync();

                _commentCacheINvalidator.InvalidateEntityList();
                _commentCacheINvalidator.InvalidateEntity(commentId);

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Delete comment success"
                };
            }
            catch (Exception ex) {

                throw new Exception(ex.ToString());
            }
        }

        public async Task<BaseResponse<string>> EditComment(Guid accountId, EditComment request)
        {
            try
            {
                var getComment = await _unitOfWork.GetRepository<Comments>().SingleOrDefaultAsync(
                    predicate: x => x.Id == request.CommentId
                    );

                if (getComment == null)
                {
                    return new BaseResponse<string>
                    {
                        status= StatusCodes.Status404NotFound.ToString(),
                        message = "Comment not found"
                    };
                }

                if (getComment != null && getComment.AccountId != accountId) {

                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status403Forbidden.ToString(),
                        message = "You do not have the right to edit other people's comments"
                    };
                }

                getComment.Comment = request.Comment;
                _unitOfWork.GetRepository<Comments>().UpdateAsync(getComment);
                await _unitOfWork.CommitAsync();

                _commentCacheINvalidator.InvalidateEntityList();
                _commentCacheINvalidator.InvalidateEntity(request.CommentId);

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Edit comment success"
                };
            }
            catch (Exception ex) { 
                
                throw new Exception(ex.ToString(), ex);
            }
        }

        public async Task<BaseResponse<Paginate<GetComments>>> GetComments(Guid blogId,int pageNumber, int pageSize)
        {
            var listParameter = new ListParameters<GetComments>(pageNumber, pageSize);

            var cacheKey = _commentCacheINvalidator.GetCacheKeyForList(listParameter);

            if (_cache.TryGetValue(cacheKey, out Paginate<GetComments> getCommentsCache))
            {
                return new BaseResponse<Paginate<GetComments>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get comment success",
                    data = getCommentsCache
                };
            }

            var getComments = await _unitOfWork.GetRepository<Comments>().GetPagingListAsync(
                predicate: x => x.BlogId == blogId && x.isDelete == false,
                include: i => i
                    .Include(c => c.Parent)
                    .ThenInclude(p => p.Account) 
                    .Include(c => c.Account),    
                page: pageNumber,
                size: pageSize
            );

            var commentTrees = CommentTreeUtil.BuildCommentTree(getComments.Items);

            var mapItem = commentTrees.Select(c => new GetComments
            {
                Id = c.Id,
                Comment = c.Comment,
                Email = c.Email,
                CreatedDate = c.CreatedDate,
                UpdatedDate = c.UpdatedDate,
                Replies = c.Replies
            }).ToList();

            var pagedResponse = new Paginate<GetComments>
            {
                Items = mapItem,
                Page = pageNumber,
                Size = pageSize,
                Total = getComments.Total,
            };

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            
            _cache.Set(cacheKey, pagedResponse ,options);
            _commentCacheINvalidator.AddToListCacheKeys(cacheKey);

            return new BaseResponse<Paginate<GetComments>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Get comment success",
                data = pagedResponse
            };
        }

        public async Task<BaseResponse<string>> ReplyComment(Guid accountId, ReplyComment request)
        {
            try
            {
                var reply = new Comments
                {
                    Id = Guid.NewGuid(),
                    Comment = request.Comment,
                    AccountId = accountId,
                    ParentId = request.ParentId,
                    BlogId = request.BlogId,
                    CreatedDate= DateTime.Now
                };

                await _unitOfWork.GetRepository<Comments>().InsertAsync(reply);
                await _unitOfWork.CommitAsync();
                
                _commentCacheINvalidator.InvalidateEntityList();
                _commentCacheINvalidator.InvalidateEntity(reply.Id);

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Reply success",
                    data = null
                };
            }
            catch (Exception ex) { 
                
                throw new Exception(ex.ToString());
            }
        }
    }
}
