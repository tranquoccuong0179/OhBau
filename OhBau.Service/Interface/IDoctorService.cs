﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Entity;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request;
using OhBau.Model.Payload.Request.Doctor;
using OhBau.Model.Payload.Request.Fetus;
using OhBau.Model.Payload.Request.Major;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Feedback;
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

        Task<BaseResponse<string>> EditFetusInformation(Guid fetusId, EditFetusInformationRequest request);

        Task<BaseResponse<Paginate<GetMajors>>> GetMajors(int pageNumber, int pageSize);

        Task<BaseResponse<List<GetFeedbackByDoctorId>>> GetFeedbackByDoctorId(Guid id);

    }
}
