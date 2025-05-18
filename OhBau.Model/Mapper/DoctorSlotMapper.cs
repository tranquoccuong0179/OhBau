using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OhBau.Model.Entity;
using OhBau.Model.Payload.Request.DoctorSlot;
using OhBau.Model.Payload.Response.DoctorSlot;
using OhBau.Model.Utils;

namespace OhBau.Model.Mapper
{
    public class DoctorSlotMapper : Profile
    {
        public DoctorSlotMapper()
        {
            CreateMap<CreateDoctorSlotRequest, DoctorSlot>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()));

            CreateMap<DoctorSlot, CreateDoctorSlotReponse>();
        }
    }
}
