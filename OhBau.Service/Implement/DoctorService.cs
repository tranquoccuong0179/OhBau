using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OhBau.Model.Entity;
using OhBau.Model.Paginate;
using OhBau.Model.Payload.Request.Doctor;
using OhBau.Model.Payload.Request.Major;
using OhBau.Model.Payload.Response;
using OhBau.Model.Payload.Response.Major;
using OhBau.Model.Utils;
using OhBau.Repository.Interface;
using OhBau.Service.Interface;

namespace OhBau.Service.Implement
{
    public class DoctorService : BaseService<DoctorService>, IDoctorService
    {
        public DoctorService(IUnitOfWork<OhBauContext> unitOfWork, ILogger<DoctorService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
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
                    Active = request.Active,
                    CreateAt = request.CreateAt,
                    UpdateAt = null,
                    DeleteAt = null
                };
                await _unitOfWork.GetRepository<Account>().InsertAsync(createAccount);

                var createMajor = new Major
                {
                    Id = Guid.NewGuid(),
                    Name = request.CreateMajorRequest.Name,
                    Active = request.CreateMajorRequest.Active,
                    CreateAt = request.CreateMajorRequest.CreateAt,
                    UpdateAt = null,
                    DeleteAt = null
                };
                await _unitOfWork.GetRepository<Major>().InsertAsync(createMajor);

                var createDoctor = new Doctor
                {
                    Id = Guid.NewGuid(),
                    FullName = request.DoctorRequest.FullName,
                    Dob = request.DoctorRequest.DOB,
                    Gender = request.DoctorRequest.Gender,
                    Content = request.DoctorRequest.Content,
                    Address = request.DoctorRequest.Address,
                    MajorId = createMajor.Id,
                    AccountId = createAccount.Id,
                    Active = request.DoctorRequest?.Active,
                    CreateAt = request?.DoctorRequest?.CreateAt,
                    UpdateAt = null,
                    DeleteAt = null
                };
                await _unitOfWork.GetRepository<Doctor>().InsertAsync(createDoctor);
                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

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
                    Active = request.Active,
                    CreateAt = request.CreateAt,
                    UpdateAt = null,
                    DeleteAt = null,
                };

                await _unitOfWork.GetRepository<Major>().InsertAsync(addNewMajor);
                await _unitOfWork.CommitAsync();

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

        public async Task<BaseResponse<Paginate<GetDoctorsResponse>>> GetDoctors(int pageSize, int pageNumber)
        {
            try
            {
                var result = await _unitOfWork.GetRepository<Doctor>()
                                   .GetPagingListAsync(include: a => a.Include(a => a.Account)
                                                                      .Include(m => m.Major),
                                                                      page: pageNumber,
                                                                      size: pageSize);
                var mappedItems = result.Items.Select(d => new GetDoctorsResponse
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    Major = d.Major.Name,
                    Address = d.Address
                }).ToList();

                var pagedResponse = new Paginate<GetDoctorsResponse>
                {
                    Items = mappedItems,
                    Page = result.Page,
                    Size = result.Size,
                    Total = result.Total,
                    TotalPages = result.TotalPages
                };

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
            var getInfor = await _unitOfWork.GetRepository<Doctor>().SingleOrDefaultAsync(
                predicate: x => x.Id == doctorId,
                include: q => q.Include(m => m.Major)
                               .Include(a => a.Account));
            if (getInfor == null)
            {
                return new BaseResponse<GetDoctorResponse>
                {
                    status = StatusCodes.Status404NotFound.ToString(),
                    message = "Doctor not found",
                    data = null
                };
            }

            var response = new GetDoctorResponse
            {
                Id = getInfor.Id,
                FullName = getInfor.FullName,
                Dob = getInfor.Dob,
                Gender = getInfor.Gender.ToString(), 
                Content = getInfor.Content,
                Address = getInfor.Address,
                Major = getInfor.Major.Name,
                Email = getInfor.Account.Email, 
                Phone = getInfor.Account.Phone,
                Active = getInfor.Active
            };

            return new BaseResponse<GetDoctorResponse>
            {

                status = StatusCodes.Status200OK.ToString(),
                message = "Get doctor infor success",
                data = response
            };

            throw new NotImplementedException();
        }
    }
}
