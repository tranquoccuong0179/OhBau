using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OhBau.Model.Entity;
using OhBau.Model.Payload.Request.Doctor;
using OhBau.Model.Payload.Response.Booking;

namespace OhBau.Model.Mapper
{
    public class DoctorMapper : Profile
    {
        public DoctorMapper()
        {
            CreateMap<Doctor, DoctorResponse>();
        }
    }
}
