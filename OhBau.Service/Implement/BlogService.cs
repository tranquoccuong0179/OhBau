using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Enum;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Blog;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class BlogService : BaseService<BlogService>, IBlogService
    {
        public BlogService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<BlogService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {

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
            return new BaseResponse<string>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Delete blog success",
                data = null
            };
        }

        public async Task<BaseResponse<GetBlog>> GetBlog(Guid blogId)
        {
            var getBlog = await _unitOfWork.GetRepository<Blog>().SingleOrDefaultAsync(
                predicate: x => x.Id == blogId,
                include: i => i.Include(a => a.Account)
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
               ReasonReject = getBlog.ReasonReject
            };

            return new BaseResponse<GetBlog>
            {
                status = StatusCodes.Status200OK.ToString(), 
                message = "Get blog Success",
                data = mapItem
            };
        }

        public async Task<BaseResponse<Paginate<GetBlogs>>> GetBlogs(int pageNumber, int pageSize,string? title)
        {
                Expression<Func<Blog, bool>> predicate = x => x.isDelete == false && x.Status.Equals(BlogStatusEnum.Published);

                if (!string.IsNullOrEmpty(title))
                {
                    predicate = x => x.Title.Contains(title) && x.Status.Equals(BlogStatusEnum.Published) && x.isDelete == false;
                }

                var getBlogs = await _unitOfWork.GetRepository<Blog>().GetPagingListAsync(
                    predicate: predicate,
                    page: pageNumber,
                    size: pageSize
                    );

                var mapItems = getBlogs.Items.Select(b => new GetBlogs
                {
                    Id = b.Id,
                    Title = b.Title,
                    CreatedDate = (DateTime)b.CreatedDate!

                }).ToList();

                var pagedResponse = new Paginate<GetBlogs>
                {

                    Items = mapItems,
                    Page = pageNumber,
                    Size = pageSize,
                    Total = mapItems.Count,
                    TotalPages = getBlogs.Total
                };

                return new BaseResponse<Paginate<GetBlogs>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get blogs success",
                    data = pagedResponse
                };
            }
        }
    }
