using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Cart;
using OhBau.Model.Payload.Response.Order;

namespace OhBau.Service.Interface
{
    public interface ICartService
    {
        Task<BaseResponse<string>> AddCourseToCart(Guid courseId,Guid accountId);
        Task<BaseResponse<Paginate<GetCartByAccount>>>GetCartItemByAccount(Guid accountId,int pageNumber, int pageSize);
        Task<BaseResponse<Paginate<GetCartDetailResponse>>>GetCartDetails(Guid accountId,int pageNumber, int pagesize);
    }
}
