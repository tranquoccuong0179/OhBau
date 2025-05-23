using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Order;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Order;

namespace OhBau.Service.Interface
{
    public interface IOrderSerivce
    {
        Task<BaseResponse<CreateOrderResponse>> CreateOrder(CreateOrderRequest request);
        Task<BaseResponse<Paginate<GetOrders>>> GetOrders(Guid accountId, int pageNumber, int pageSize);
        Task<BaseResponse<Paginate<GetOrderDetails>>> GetOrderDetails(Guid accountId,Guid orderId, int pageNumber, int pageSize);
    }
}
