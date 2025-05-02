using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Entity;
using OhBau.Model.Paginate;
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
        
        Task<BaseResponse<Paginate<GetDoctorsResponse>>> GetDoctors(int pageSize, int pageNumber);

        Task<BaseResponse<GetDoctorResponse>> GetDoctorInfo(Guid doctorId);
    }
}
