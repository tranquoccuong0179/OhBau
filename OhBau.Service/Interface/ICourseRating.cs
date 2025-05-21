using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Payload.Request.Like;
using OhBau.Model.Payload.Response;

namespace OhBau.Service.Interface
{
    public interface ICourseRating
    {
        Task<BaseResponse<string>> Rating(Guid accountId,RatingRequest request);
    }
}
