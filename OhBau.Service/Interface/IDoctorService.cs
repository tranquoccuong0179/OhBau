using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request;
using OhBau.Model.Payload.Request.Doctor;
using OhBau.Model.Payload.Request.Major;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Major;

namespace OhBau.Service.Interface
{
    public interface IDoctorService
    {
        Task<BaseResponse<CreateMajorResponse>> CreateMajonr(CreateMajorRequest request);
        Task<BaseResponse<string>> CreateDoctor(CreateDoctorRequest request);
        
        Task<BaseResponse<Paginate<GetDoctorsResponse>>> GetDoctors(int pageSize, int pageNumber,string doctorName);

        Task<BaseResponse<GetDoctorResponse>> GetDoctorInfo(Guid doctorId);

        Task<BaseResponse<DoctorRequest>> EditDoctorInfor(Guid doctorId, DoctorRequest request);

        Task<BaseResponse<string>> EditMajor(Guid MajorID, EditMajorRequest request);

        Task<BaseResponse<string>> DeleteDoctor(Guid doctorId);

        Task<BaseResponse<string>> DeleteMajor(Guid majorId);
    }
}
