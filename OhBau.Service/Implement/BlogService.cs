using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Enum;
using OhBau.Model.Exception;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Blog;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Blog;
using OhBau.Model.Payload.Response.LikeBlog;
using OhBau.Model.Payload.Response.Order;
using OhBau.Model.Utils;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class BlogService : BaseService<BlogService>, IBlogService
    {
        private readonly HtmlSanitizerUtil _sanitizer;
        private readonly IMemoryCache _cache;
        private readonly GenericCacheInvalidator<Blog> _blogCacheInvalidator;
        public BlogService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<BlogService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor,
            HtmlSanitizerUtil sanitizer,
            GenericCacheInvalidator<Blog> blogCacheInvalidator,
            IMemoryCache cache) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _sanitizer = sanitizer;
            _cache = cache;
            _blogCacheInvalidator = blogCacheInvalidator;
        }


        public async Task<BaseResponse<CreateNewBlogResponse>> CreateBlog(CreateBlogRequest request)
        {
            Guid? userId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.Id.Equals(userId) && a.Active == true);
            if (account == null)
            {
                throw new NotFoundException("Tài khoản không tồn tại");
            }

            var blog = new Blog
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = _sanitizer.Sanitize(request.Content),
                Status = BlogStatusEnum.Pending,
                CreatedDate = TimeUtil.GetCurrentSEATime(),
                UpdatedDate = TimeUtil.GetCurrentSEATime(),
                isDelete = false,
                AccountId = account.Id
            };

            await _unitOfWork.GetRepository<Blog>().InsertAsync(blog);
            await _unitOfWork.CommitAsync();

            _blogCacheInvalidator.InvalidateEntityList();

            return new BaseResponse<CreateNewBlogResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Đăng kí blog thành công",
                data = new CreateNewBlogResponse
                {
                    Id = blog.Id,
                    Title = blog.Title,
                    Content = blog.Content,
                    Status = blog.Status
                }
            };
        }

        public async Task<BaseResponse<string>> DeleteBlog(Guid blogId)
        {
            var checkDelete = await _unitOfWork.GetRepository<Blog>().GetByConditionAsync(x => x.Id == blogId);
            if (checkDelete == null)
            {
                return new BaseResponse<string>
                {
                    status = StatusCodes.Status404NotFound.ToString(),
                    message = "Blog not found",
                    data = null
                };
            }
            checkDelete.isDelete = true;
            checkDelete.DeletedDate = DateTime.Now;
            _unitOfWork.GetRepository<Blog>().UpdateAsync(checkDelete);
            await _unitOfWork.CommitAsync();

            _blogCacheInvalidator.InvalidateEntity(blogId);
            _blogCacheInvalidator.InvalidateEntityList();

            return new BaseResponse<string>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Delete blog success",
                data = null
            };
        }

        public async Task<BaseResponse<GetBlog>> GetBlog(Guid blogId)
        {

            var cachedBlog = _blogCacheInvalidator.GetEntityCache<GetBlog>(blogId);
            if (cachedBlog != null)
            {
                return new BaseResponse<GetBlog>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get blog success(cache)",
                    data = cachedBlog
                };
            }
            var getBlog = await _unitOfWork.GetRepository<Blog>().SingleOrDefaultAsync(
                predicate: x => x.Id == blogId,
                include: i => i.Include(a => a.Account).Include(x => x.LikeBlog).Include(x => x.Comments)
                );

            if (getBlog == null)
            {
                return new BaseResponse<GetBlog>
                {
                    status = StatusCodes.Status404NotFound.ToString(),
                    message = "Get blog success",
                    data = null
                };
            }

            var mapItem = new GetBlog
            {
                Id = blogId,
                Title = getBlog.Title,
                Content = getBlog.Content,
                CreatedDate = getBlog.CreatedDate,
                UpdatedDate = getBlog.UpdatedDate,
                DeletedDate = getBlog.DeletedDate,
                IsDelete = getBlog.isDelete,
                Email = getBlog.Account.Email,
                Status = getBlog.Status,
                ReasonReject = getBlog.ReasonReject,
                TotalLike = getBlog.LikeBlog.Count,
                TotalComment = getBlog.Comments.Count,
                likeBlogs = getBlog.LikeBlog.Select(l => new LikeBlogs
                {
                    AccountId = l.AccountID,
                    isLiked = l.isLiked

                }).ToList()
            };

            _blogCacheInvalidator.SetEntityCache(blogId, mapItem, TimeSpan.FromMinutes(30));
            return new BaseResponse<GetBlog>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Get blog Success",
                data = mapItem
            };
        }

        public async Task<BaseResponse<Paginate<GetBlogs>>> GetBlogs(int pageNumber, int pageSize, string? title)
        {

            var listParams = new ListParameters<Blog>(pageNumber, pageSize);
            listParams.AddFilter("Title", title);

            var cacheKey = _blogCacheInvalidator.GetCacheKeyForList(listParams);

            if (_cache.TryGetValue(cacheKey, out Paginate<GetBlogs> cachedResult))
            {
                return new BaseResponse<Paginate<GetBlogs>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Lấy danh sách blog thành công (từ cache)",
                    data = cachedResult
                };
            }

            Expression<Func<Blog, bool>> predicate = x => x.isDelete == false;

            if (!string.IsNullOrEmpty(title))
            {
                predicate = x => x.Title.Contains(title) && x.isDelete == false;
            }

            var getBlogs = await _unitOfWork.GetRepository<Blog>().GetPagingListAsync(
                include: i => i.Include(c => c.Comments).Include(x => x.LikeBlog).Include(a => a.Account),
                predicate: predicate,
                page: pageNumber,
                size: pageSize
                );

            var mapItems = getBlogs.Items.Select(b => new GetBlogs
            {
                Id = b.Id,
                Title = b.Title,
                Content = b.Content,
                CreatedDate = (DateTime)b.CreatedDate!,
                UpdatedDate = (DateTime)b.UpdatedDate!,
                TotalComment = b.Comments.Count(),
                TotalLike = b.LikeBlog.Count,
                AuthorId = b.Account.Id,
                AuthorEmail = b.Account.Email,
                LikeBlogs = b.LikeBlog.Select(l => new LikeBlogs
                {
                    AccountId = l.AccountID,
                    isLiked = l.isLiked
                }).ToList()

            }).OrderByDescending(x => x.UpdatedDate).ToList();

            var pagedResponse = new Paginate<GetBlogs>
            {

                Items = mapItems,
                Page = pageNumber,
                Size = pageSize,
                Total = getBlogs.Total,
                TotalPages = (int)Math.Ceiling((double)getBlogs.Total / pageSize)
            };

            var cacheOption = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
            };

            _cache.Set(cacheKey, pagedResponse,cacheOption);
            _blogCacheInvalidator.AddToListCacheKeys(cacheKey);

            return new BaseResponse<Paginate<GetBlogs>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Get blogs success",
                data = pagedResponse
            };
        }

        public async Task<BaseResponse<UpdateBlogResponse>> UpdateBlog(Guid id, UpdateBlogRequest request)
        {
            Guid? userId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.Id.Equals(userId) && a.Active == true);
            if (account == null)
            {
                throw new NotFoundException("Tài khoản không tồn tại");
            }

            var blog = await _unitOfWork.GetRepository<Blog>().SingleOrDefaultAsync(
                predicate: b => b.Id.Equals(id));
            if (blog == null)
            {
                throw new NotFoundException("Không tìm thấy blog");
            }

            if (!blog.AccountId.Equals(account.Id))
            {
                throw new ForbiddentException("Blog này không phải của tài khoản này");
            }

            blog.Title = string.IsNullOrEmpty(request.Title) ? blog.Title : request.Title;
            blog.Content = string.IsNullOrEmpty(request.Content) ? blog.Content : _sanitizer.Sanitize(request.Content);
            blog.UpdatedDate = TimeUtil.GetCurrentSEATime();

            _unitOfWork.GetRepository<Blog>().UpdateAsync(blog);
            await _unitOfWork.CommitAsync();

            _blogCacheInvalidator.InvalidateEntity(id);
            _blogCacheInvalidator.InvalidateEntityList();

            UpdateBlogResponse response = new UpdateBlogResponse
            {
                Id = blog.Id,
                Title = blog.Title,
                Content = blog.Content,
                Status = blog.Status
            };

            return new BaseResponse<UpdateBlogResponse>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Cập nhật blog thành công",
                data = response
            };
        }


        public async Task<BaseResponse<string>> LikeOrDisLikeBlog(Guid accountId, Guid BlogId)
        {
            try
            {
                var checkLikeBlog = await _unitOfWork.GetRepository<LikeBlog>().GetByConditionAsync(predicate: x => x.BlogId == BlogId && x.AccountID == accountId);
                var blog = await _unitOfWork.GetRepository<Blog>().GetByConditionAsync(predicate: x => x.Id == BlogId);

                if (checkLikeBlog != null)
                {
                    _unitOfWork.GetRepository<LikeBlog>().DeleteAsync(checkLikeBlog);
                    await _unitOfWork.CommitAsync();

                    blog.TotalLike = blog.TotalLike - 1;
                    blog.UpdatedDate = DateTime.Now;

                    _unitOfWork.GetRepository<Blog>().UpdateAsync(blog);
                    await _unitOfWork.CommitAsync();

                    _blogCacheInvalidator.InvalidateEntityList();
                    _blogCacheInvalidator.InvalidateEntity(BlogId);
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status200OK.ToString(),
                        message = "Dislike Blog Success",
                        data = null
                    };
                }

                var newLike = new LikeBlog
                {
                    Id = Guid.NewGuid(),
                    BlogId = BlogId,
                    AccountID = accountId,
                    CreatedDate = DateTime.Now,
                    isLiked = true

                };

                await _unitOfWork.GetRepository<LikeBlog>().InsertAsync(newLike);

                blog.TotalLike += 1;
                blog.UpdatedDate = DateTime.Now;

                _unitOfWork.GetRepository<Blog>().UpdateAsync(blog);
                await _unitOfWork.CommitAsync();

                _blogCacheInvalidator.InvalidateEntityList();
                _blogCacheInvalidator.InvalidateEntity(BlogId);

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Like blog success",
                    data = null
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<BaseResponse<Paginate<GetBlogs>>> GetBlogsByUser(Guid userId, int pageNumber, int pageSize)
        {
            var parameters = new ListParameters<Blog>(pageNumber, pageSize);
            parameters.AddFilter("userId", userId);

            var cache = _blogCacheInvalidator.GetCacheKeyForList(parameters);
            if (_cache.TryGetValue(cache, out Paginate<GetBlogs> BlogsByUser))
            {
                return new BaseResponse<Paginate<GetBlogs>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get blogs success(cache)",
                    data = BlogsByUser
                };
            }

            var getBlogs = await _unitOfWork.GetRepository<Blog>().GetPagingListAsync(
                predicate: x => x.AccountId == userId,
                include: i => i.Include(x => x.Account).Include(x => x.Comments).Include(x => x.LikeBlog),
                page: pageNumber,
                size: pageSize);

            var mapItem = getBlogs.Items.Select(b => new GetBlogs
            {
                Id = b.Id,
                Title = b.Title,
                Content = b.Content,
                CreatedDate = b.CreatedDate,
                UpdatedDate = b.UpdatedDate,
                TotalComment = b.Comments.Count,
                TotalLike = b.LikeBlog.Count,
                AuthorId = b.Account.Id,
                AuthorEmail = b.Account.Email,
                LikeBlogs = b.LikeBlog.Select(l => new LikeBlogs
                {
                    AccountId = l.Id,
                    isLiked = l.isLiked

                }).ToList()
            }).OrderByDescending(x => x.UpdatedDate).ToList();

            var pagedResponse = new Paginate<GetBlogs>
            {
                Items = mapItem,
                Page = pageNumber,
                Size = pageSize,
                Total = getBlogs.Total,
                TotalPages = (int)Math.Ceiling((double)getBlogs.Total / pageSize)
            };

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            _cache.Set(cache, pagedResponse,options);
            _blogCacheInvalidator.AddToListCacheKeys(cache);

            return new BaseResponse<Paginate<GetBlogs>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Get blogs success",
                data = pagedResponse
            };

        }
    }
}
