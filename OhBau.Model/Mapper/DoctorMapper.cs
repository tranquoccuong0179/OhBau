using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OhBau.Model.Entity;
using OhBau.Model.Payload.Request.Doctor;
using OhBau.Model.Payload.Response.Booking;
using OhBau.Model.Payload.Response.Feedback;

namespace OhBau.Model.Mapper
{
    public class DoctorMapper : Profile
    {
        public DoctorMapper()
        {
            CreateMap<Doctor, DoctorResponse>();

            CreateMap<Doctor, GetFeedbackByDoctorId>()
                .ForMember(dest => dest.Feedbacks, opt => opt.MapFrom(src => src.Feedbacks));
        }
    }
}
