using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Category;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Category;

namespace OhBau.Service.Interface
{
    public interface ICategoryService
    {
        Task<BaseResponse<Paginate<CategoryResponse>>> GetCategories(int pageNumber, int pageSize);
        Task<BaseResponse<string>> EditCategory(Guid categoryId,EditCategoryRequest request);
        Task<BaseResponse<string>> DeleteCategory(Guid categoryId);

        Task<BaseResponse<string>> CreateCategory(CreateCategoryRequest request);

    }
}
