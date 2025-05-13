using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OhBau.Model.Entity;
using OhBau.Model.Payload.Response.MotherHealth;

namespace OhBau.Model.Mapper
{
    public class MotherHealthMapper : Profile
    {
        public MotherHealthMapper()
        {
            CreateMap<MotherHealthRecord, GetMotherHealthResponse>();

            CreateMap<MotherHealthRecord, UpdateMotherHealthResponse>();
        }
    }
}
