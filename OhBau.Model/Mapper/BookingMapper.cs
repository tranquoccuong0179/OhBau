using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OhBau.Model.Entity;
using OhBau.Model.Enum;
using OhBau.Model.Payload.Request.Booking;
using OhBau.Model.Payload.Response.Booking;
using OhBau.Model.Utils;

namespace OhBau.Model.Mapper
{
    public class BookingMapper : Profile
    {
        public BookingMapper()
        {
            CreateMap<CreateBookingRequest, Booking>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => TypeBookingEnum.Booked.GetDescriptionFromEnum()))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => TimeUtil.GetCurrentSEATime()));

            CreateMap<Booking, CreateBookingResponse>();

            CreateMap<Booking, GetBookingResponse>()
                .ForMember(dest => dest.Parent, opt => opt.MapFrom(src => src.Parent))
                .ForMember(dest => dest.Doctor, opt => opt.MapFrom(src => src.DotorSlot.Doctor))
                .ForMember(dest => dest.Slot, opt => opt.MapFrom(src => src.DotorSlot.Slot));
        }
    }
}
