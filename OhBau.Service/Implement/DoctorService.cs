﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request;
using OhBau.Model.Payload.Request.Doctor;
using OhBau.Model.Payload.Request.Fetus;
using OhBau.Model.Payload.Request.Major;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Feedback;
using OhBau.Model.Payload.Response.Major;
using OhBau.Model.Utils;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OhBau.Service.Implement
{
    public class DoctorService : BaseService<DoctorService>, IDoctorService
    {
        private readonly IMemoryCache _cache;
        private readonly GenericCacheInvalidator<Doctor> _doctorCacheInvalidator;
        private readonly GenericCacheInvalidator<Major> _majorCacheInvalidator;
        private readonly GenericCacheInvalidator<Fetus> _fetusCacheInvalidator;
        public DoctorService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<DoctorService> logger, 
            IMapper mapper, IHttpContextAccessor httpContextAccessor, 
            GenericCacheInvalidator<Doctor> doctorCacheInvalidator, IMemoryCache cache,
            GenericCacheInvalidator<Major> majorCacheInvalidator,
            GenericCacheInvalidator<Fetus> fetusCacheInvalidator) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _doctorCacheInvalidator = doctorCacheInvalidator;
            _majorCacheInvalidator = majorCacheInvalidator;
            _fetusCacheInvalidator = fetusCacheInvalidator;
            _cache = cache;
        }

        public async Task<BaseResponse<string>> CreateDoctor(CreateDoctorRequest request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var checkEmailAlready = await _unitOfWork.GetRepository<Account>().GetByConditionAsync(x => x.Email.ToLower().Equals(request.Email.ToLower()));
                if (checkEmailAlready != null)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status409Conflict.ToString(),
                        message = "Email already exists",
                        data = null
                    };
                }

                var createAccount = new Account
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email,
                    Phone = request.Phone,
                    Password = PasswordUtil.HashPassword(request.Password),
                    Role = request.Role,
                    Active = true,
                    CreateAt = TimeUtil.GetCurrentSEATime(),
                    UpdateAt = null,
                    DeleteAt = null
                };
                await _unitOfWork.GetRepository<Account>().InsertAsync(createAccount);

             

                var createDoctor = new Doctor
                {
                    Id = Guid.NewGuid(),
                    FullName = request.DoctorRequest.FullName,
                    Avatar = request.DoctorRequest.Avatar!,
                    Dob = request.DoctorRequest.DOB,
                    Gender = request.DoctorRequest.Gender,
                    Content = request.DoctorRequest.Content,
                    Address = request.DoctorRequest.Address,
                    MajorId = request.MajorId,
                    AccountId = createAccount.Id,
                    Active = request.DoctorRequest?.Active,
                    CreateAt = request?.DoctorRequest?.CreateAt,
                    UpdateAt = null,
                    DeleteAt = null,
                    Experence = request.DoctorRequest.Experence,
                    CareerPath = request.DoctorRequest.CareerPath,
                    Focus  = request.DoctorRequest.Focus,
                    MedicalProfile = request.DoctorRequest.MedicalProfile,
                    OutStanding = request.DoctorRequest.OutStanding,
                    WorkSchedule = request.DoctorRequest.WorkSchedule
                };
                await _unitOfWork.GetRepository<Doctor>().InsertAsync(createDoctor);

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                _doctorCacheInvalidator.InvalidateEntityList();
                _doctorCacheInvalidator.InvalidateEntity(createDoctor.Id);
                _fetusCacheInvalidator.InvalidateEntityList();
                _majorCacheInvalidator.InvalidateEntityList();

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Create Doctor Success"
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception(ex.ToString());
            }
        }


        public async Task<BaseResponse<CreateMajorResponse>> CreateMajonr(CreateMajorRequest request)
        {
            try
            {
                var checkAlready = await _unitOfWork.GetRepository<Major>()
                    .GetByConditionAsync(x => x.Name.ToLower().Equals(request.Name.ToLower()));

                if (checkAlready != null)
                {
                    return new BaseResponse<CreateMajorResponse>
                    {
                        status = StatusCodes.Status409Conflict.ToString(),
                        message = "Major name already exists",
                        data = new CreateMajorResponse
                        {
                            Name = checkAlready.Name
                        }
                    };
                }

                var addNewMajor = new Major
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Active = true,
                    CreateAt = TimeUtil.GetCurrentSEATime(),
                    UpdateAt = null,
                    DeleteAt = null,
                };

                await _unitOfWork.GetRepository<Major>().InsertAsync(addNewMajor);
                await _unitOfWork.CommitAsync();

                _majorCacheInvalidator.InvalidateEntityList();
                _majorCacheInvalidator.InvalidateEntity(addNewMajor.Id);

                return new BaseResponse<CreateMajorResponse>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Create Major Success",
                    data = new CreateMajorResponse
                    {
                        Name = addNewMajor.Name
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<BaseResponse<Paginate<GetDoctorsResponse>>> GetDoctors(int pageSize, int pageNumber,string doctorName)
        {
            try
            {
                var listParameter = new ListParameters<Doctor>(pageNumber, pageSize);
                listParameter.AddFilter("DoctorName", doctorName);

                var cacheKey = _doctorCacheInvalidator.GetCacheKeyForList(listParameter);

                if (_cache.TryGetValue(cacheKey, out Paginate<GetDoctorsResponse> GetDoctors))
                {
                    return new BaseResponse<Paginate<GetDoctorsResponse>>
                    {
                        status = StatusCodes.Status200OK.ToString(),
                        message = "Get doctor success",
                        data = GetDoctors
                    };
                }

                Expression<Func<Doctor, bool>> predicate = null;

                if (!string.IsNullOrWhiteSpace(doctorName))
                {
                    predicate = d => d.FullName.Contains(doctorName);
                }

                var result = await _unitOfWork.GetRepository<Doctor>()
                                   .GetPagingListAsync(
                                       predicate: predicate,
                                       include: q => q.Include(d => d.Account)
                                                      .Include(d => d.Major),
                                       page: pageNumber,
                                       size: pageSize
                                   );

                var mappedItems = result.Items.Select(d => new GetDoctorsResponse
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    Avatar  = d.Avatar,
                    Major = d.Major.Name,
                    Address = d.Address
                }).ToList();

                var pagedResponse = new Paginate<GetDoctorsResponse>
                {
                    Items = mappedItems,
                    Page = result.Page,
                    Size = result.Size,
                    Total = result.Total,
                    //TotalPages = result.TotalPages
                };

                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };

                _cache.Set(cacheKey, pagedResponse,options);
                _doctorCacheInvalidator.AddToListCacheKeys(cacheKey);

                return new BaseResponse<Paginate<GetDoctorsResponse>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get doctors success",
                    data = pagedResponse
                };
            }
            catch (Exception ex) {

                throw new Exception(ex.ToString());
            }

        }

        public async Task<BaseResponse<GetDoctorResponse>> GetDoctorInfo(Guid doctorId)
        {
            var cacheKey = _doctorCacheInvalidator.GetEntityCache<GetDoctorResponse>(doctorId);
            if (cacheKey != null)
            {
                return new BaseResponse<GetDoctorResponse>
                {
                     status = StatusCodes.Status200OK.ToString(),
                     message = "Get doctor success",
                     data = cacheKey
                };
            }

            var getInfor = await _unitOfWork.GetRepository<Doctor>().SingleOrDefaultAsync(
                predicate: x => x.Id == doctorId,
                include: q => q.Include(m => m.Major)
                               .Include(a => a.Account)
                               .Include(f => f.Feedbacks));
             
            if (getInfor == null)
            {
                return new BaseResponse<GetDoctorResponse>
                {
                    status = StatusCodes.Status404NotFound.ToString(),
                    message = "Doctor not found",
                    data = null
                };
            }

         

            var medicalProfile = getInfor.MedicalProfile?
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim()).ToList() ?? new List<string>();

            var careerPath = getInfor.CareerPath?
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim()).ToList() ?? new List<string>();

            var outStanding = getInfor.OutStanding?
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim()).ToList() ?? new List<string>();

            var focus = getInfor.Focus.Split('|', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(s => s.Trim()).ToList() ?? new List<string>();

            var experence = getInfor.Experence.Split("|",StringSplitOptions.RemoveEmptyEntries)
                                       .Select(s => s.Trim()).ToList() ?? new List<string>();

            double rating = 0;
            int totalFeedbacks = 0;
            if (getInfor.Feedbacks.Count > 0)
            {
                rating = getInfor.Feedbacks.Average(s => s.Rating);
                totalFeedbacks = getInfor.Feedbacks.Count;
            }

            var response = new GetDoctorResponse
            {
                Id = getInfor.Id,
                FullName = getInfor.FullName,
                Avatar = getInfor.Avatar,
                Dob = getInfor.Dob,
                Gender = getInfor.Gender.ToString(),
                Content = getInfor.Content,
                Address = getInfor.Address,
                MajorId = getInfor.MajorId,
                Major = getInfor.Major.Name,
                Email = getInfor.Account.Email,
                Phone = getInfor.Account.Phone,
                Active = getInfor.Active,
                Experence = experence,
                Focus = focus,
                MedicalProfile = medicalProfile,
                CareerPath = careerPath,
                OutStanding = outStanding,
                Rating = rating,
                totalFeedbacks = totalFeedbacks,
                WorkSchedule = getInfor.WorkSchedule
            };

            _doctorCacheInvalidator.SetEntityCache(doctorId, response, TimeSpan.FromMinutes(30));
            return new BaseResponse<GetDoctorResponse>
            {

                status = StatusCodes.Status200OK.ToString(),
                message = "Get doctor infor success",
                data = response
            };

        }

        public async Task<BaseResponse<DoctorRequest>> EditDoctorInfor(Guid doctorId, DoctorRequest request)
        {
            try
            {
                var getDoctor = await _unitOfWork.GetRepository<Doctor>().SingleOrDefaultAsync(
                                                                       predicate: x => x.Id == doctorId,
                                                                       include: q => q.Include(m => m.Major)
                                                                                      .Include(a => a.Account));
                if (getDoctor == null)
                {
                    return new BaseResponse<DoctorRequest>
                    {
                        status = StatusCodes.Status404NotFound.ToString(),
                        message = "Docotor not found",
                        data = null
                    };
                }

                getDoctor.FullName = request.FullName ?? getDoctor.FullName;
                if (request.Avatar == null)
                {
                    getDoctor.Avatar = getDoctor.Avatar;
                }
                else
                {
                    getDoctor.Avatar = request.Avatar;
                }
                getDoctor.Dob = request.DOB != null ? request.DOB : getDoctor.Dob;
                getDoctor.Gender = request.Gender ?? getDoctor.Gender;
                getDoctor.Content = request.Content ?? getDoctor.Content;
                getDoctor.Address = request.Address ?? getDoctor.Address;
                getDoctor.Active = getDoctor.Active;
                getDoctor.CreateAt = getDoctor.CreateAt;
                getDoctor.UpdateAt = DateTime.Now;
                getDoctor.DeleteAt = null;
                getDoctor.MedicalProfile = request.MedicalProfile ?? getDoctor.MedicalProfile;
                getDoctor.CareerPath = request.CareerPath ?? getDoctor.CareerPath;
                getDoctor.Experence = request.Experence ?? getDoctor.Experence;
                getDoctor.OutStanding = request.OutStanding ?? getDoctor.OutStanding;
                getDoctor.Focus = request.Focus ?? getDoctor.Focus;
                getDoctor.WorkSchedule = request.WorkSchedule ?? getDoctor?.WorkSchedule;

                 _unitOfWork.GetRepository<Doctor>().UpdateAsync(getDoctor);
                await _unitOfWork.CommitAsync();

                _doctorCacheInvalidator.InvalidateEntityList();
                _doctorCacheInvalidator.InvalidateEntity(doctorId);
                _fetusCacheInvalidator.InvalidateEntityList();
                _majorCacheInvalidator.InvalidateEntityList();

                return new BaseResponse<DoctorRequest>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Edit doctor info success",
                    data = request
                };
            }
            catch (Exception ex) { 
            
                throw new Exception(ex.ToString());
            }
        }

        public async Task<BaseResponse<string>> EditMajor(Guid majorId, EditMajorRequest request)
        {
            try
            {
                var getMajor = await _unitOfWork.GetRepository<Major>().GetByConditionAsync(x => x.Id == majorId);
                if (getMajor == null)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status404NotFound.ToString(),
                        message = "Major not found"
                    };
                }

                getMajor.Name = request.MajorName;
                getMajor.CreateAt = getMajor.CreateAt;
                getMajor.DeleteAt = getMajor.DeleteAt;
                getMajor.UpdateAt = DateTime.Now;
                getMajor.Active = true;
                _unitOfWork.GetRepository<Major>().UpdateAsync(getMajor);
                await _unitOfWork.CommitAsync();
                
                _majorCacheInvalidator.InvalidateEntity(majorId);
                _majorCacheInvalidator.InvalidateEntityList();
                _fetusCacheInvalidator.InvalidateEntityList();
                _majorCacheInvalidator.InvalidateEntityList();

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Update major success",
                };
            }
            catch (Exception ex) { 
                
                throw new Exception(ex.ToString());
            }
        }

        public async Task<BaseResponse<string>> DeleteDoctor(Guid doctorId)
        {
            try
            {
                var getDoctor = await _unitOfWork.GetRepository<Doctor>().GetByConditionAsync(x => x.Id == doctorId);
                if (getDoctor != null && getDoctor.Active == true)
                {
                    getDoctor.Active = false;
                    getDoctor.DeleteAt = DateTime.Now;
                    _unitOfWork.GetRepository<Doctor>().UpdateAsync(getDoctor);
                    await _unitOfWork.CommitAsync();

                    _doctorCacheInvalidator.InvalidateEntity(doctorId);
                    _doctorCacheInvalidator.InvalidateEntityList();
                    _fetusCacheInvalidator.InvalidateEntityList();
                    _majorCacheInvalidator.InvalidateEntityList();

                    return new BaseResponse<string>
                    {

                        status = StatusCodes.Status200OK.ToString(),
                        message = "Delete doctor success"
                    };
                }

                return new BaseResponse<string>()
                {
                    status = StatusCodes.Status404NotFound.ToString(),
                    message = "Doctor not found"
                };
            }
            catch (Exception ex) {
                
                throw new Exception(ex.ToString());
            }

        }

        public async Task<BaseResponse<string>> DeleteMajor(Guid majorId)
        {
            try
            {
                var getMajor = await _unitOfWork.GetRepository<Major>().GetByConditionAsync(x => x.Id.Equals(majorId));
                if (getMajor != null && getMajor.Active == true)
                {
                    getMajor.Active = false;
                    getMajor.DeleteAt = DateTime.Now;
                    _unitOfWork.GetRepository<Major>().UpdateAsync(getMajor);
                    await _unitOfWork.CommitAsync();
                    _majorCacheInvalidator.InvalidateEntity(majorId);
                    _majorCacheInvalidator.InvalidateEntityList();
                    _doctorCacheInvalidator.InvalidateEntityList();
                    _fetusCacheInvalidator.InvalidateEntityList();

                    return new BaseResponse<string>
                    {

                        status = StatusCodes.Status200OK.ToString(),
                        message = "Delete major success"
                    };
                }

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status404NotFound.ToString(),
                    message = "Major not found"
                };
            }
            catch(Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<BaseResponse<string>> EditFetusInformation(Guid fetusId, EditFetusInformationRequest request)
        {
            try
            {

                var getFetus = await _unitOfWork.GetRepository<FetusDetail>().GetByConditionAsync(x => x.FetusId == fetusId);
                if (getFetus == null)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status404NotFound.ToString(),
                        message = "Fetus not found",
                        data = null
                    };
                }

                getFetus.Weekly = request.Weekly != 0 ? request.Weekly : getFetus.Weekly;
                getFetus.Gsd = request.Gsd != 0 ? request.Gsd : getFetus.Gsd;
                getFetus.Crl = request.Crl != 0 ? request.Crl : getFetus.Crl;
                getFetus.Crl = request.Crl != 0 ? request.Crl : getFetus.Crl;
                getFetus.Bpd = request.Bpd != 0 ? request.Bpd : getFetus.Bpd;
                getFetus.Fl = request.Fl != 0 ? request.Fl : getFetus.Fl;
                getFetus.Hc = request.Hc != 0 ? request.Hc : getFetus.Hc;
                getFetus.Ac = request.Ac != 0 ? request.Ac : getFetus.Ac;
                getFetus.CreateAt = getFetus.CreateAt;
                getFetus.UpdateAt = request.UpdateAt;
                getFetus.DeleteAt = getFetus.DeleteAt;

                await _unitOfWork.GetRepository<FetusDetail>().InsertAsync(getFetus);
                await _unitOfWork.CommitAsync();

                _fetusCacheInvalidator.InvalidateEntity(fetusId);
                _fetusCacheInvalidator.InvalidateEntityList();
                _majorCacheInvalidator.InvalidateEntityList();
                _doctorCacheInvalidator.InvalidateEntityList();

                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Edit fetus information success",
                    data = null
                };
            }
            catch (Exception ex) { 
            
                throw new Exception(ex.ToString());
            }
        }

        public async Task<BaseResponse<Paginate<GetMajors>>> GetMajors(int pageNumber, int pageSize)
        {
            var parameters = new ListParameters<GetMajors>(pageNumber, pageSize);
            var cache = _majorCacheInvalidator.GetCacheKeyForList(parameters);
            if (_cache.TryGetValue(cache, out Paginate<GetMajors> GetMajors))
            {
                return new BaseResponse<Paginate<GetMajors>>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = "Get majors succes(cache)",
                    data = GetMajors
                };
            }

            var getMajors = await _unitOfWork.GetRepository<Major>().GetPagingListAsync(page:pageNumber, size: pageSize);

            var mapItems = getMajors.Items.Select(c => new GetMajors
            {
                Id = c.Id,
                Name = c.Name,
                Active = c.Active,
                CreateAt = c.CreateAt,
                UpdateAt = c.UpdateAt,
                DeleteAt = c.DeleteAt

            }).ToList();

            var pagedResponse = new Paginate<GetMajors>
            {
                Items = mapItems,
                Page = pageNumber,
                Size = pageSize,
                Total = mapItems.Count
            };

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            _cache.Set(cache,pagedResponse, options);

            return new BaseResponse<Paginate<GetMajors>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Get majors success",
                data =pagedResponse
            };
        }

        public async Task<BaseResponse<List<GetFeedbackByDoctorId>>> GetFeedbackByDoctorId(Guid id)
        {
            var doctor = await _unitOfWork.GetRepository<Doctor>().GetListAsync(
                selector: d => _mapper.Map<GetFeedbackByDoctorId>(d),
                predicate: d => d.Id.Equals(id) && d.Active == true,
                include: d => d.Include(d => d.Feedbacks));

            return new BaseResponse<List<GetFeedbackByDoctorId>>
            {
                status = StatusCodes.Status200OK.ToString(),
                message = "Lấy danh sách feedback của doctor này thành công",
                data = doctor.ToList()
            };
        }
    }
}
