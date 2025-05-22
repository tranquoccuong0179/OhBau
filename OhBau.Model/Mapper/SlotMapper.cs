using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OhBau.Model.Entity;
using OhBau.Model.Payload.Request.Slot;
using OhBau.Model.Payload.Response.DoctorSlot;
using OhBau.Model.Payload.Response.Slot;
using OhBau.Model.Utils;

namespace OhBau.Model.Mapper
{
    public class SlotMapper : Profile
    {
        public SlotMapper()
        {
            CreateMap<CreateSlotRequest, Slot>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()));

            CreateMap<Slot, CreateSlotResponse>();

            CreateMap<Slot, GetSlotResponse>();

            CreateMap<Slot, SlotResponse>();
        }
    }
}
