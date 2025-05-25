using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Fetus;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Fetus;
using OhBau.Model.Payload.Response.FetusResponse;

namespace OhBau.Service.Interface
{
    public interface IFetusService
    {
        Task<BaseResponse<CreateFetusResponse>> CreateFetus(CreateFetusRequest request);
        Task<BaseResponse<IPaginate<GetFetusResponse>>> GetAllFetus(int page, int size);
        Task<BaseResponse<GetFetusResponse>> GetFetusById(Guid id);
        Task<BaseResponse<GetFetusResponse>> GetFetusByCode(string code);
        Task<BaseResponse<bool>> DeleteFetus(Guid id);
        Task<BaseResponse<GetFetusDetailResponse>> UpdateFetusDetail(Guid id, EditFetusInformationRequest request);
        Task<BaseResponse<UpdateFetusResponse>> UpdateFetus(Guid id, UpdateFetusRequest request);
    }
}
