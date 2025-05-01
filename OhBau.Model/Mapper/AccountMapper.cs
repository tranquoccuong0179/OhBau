using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OhBau.Model.Entity;
using OhBau.Model.Enum;
using OhBau.Model.Payload.Request.Account;
using OhBau.Model.Payload.Response.Account;
using OhBau.Model.Utils;

namespace OhBau.Model.Mapper
{
    public class AccountMapper : Profile
    {
        public AccountMapper()
        {
            CreateMap<RegisterRequest, Account>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => PasswordUtil.HashPassword(src.Password)))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.GetDescriptionFromEnum()))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()));

            CreateMap<Account, RegisterResponse>()
                .ForMember(dest => dest.RegisterParentResponse, opt => opt.MapFrom(src => src.Parents.FirstOrDefault()));

            CreateMap<Account, GetAccountResponse>();
        }
    }
}
