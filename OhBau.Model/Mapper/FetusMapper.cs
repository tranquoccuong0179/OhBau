using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OhBau.Model.Entity;
using OhBau.Model.Payload.Response.Fetus;

namespace OhBau.Model.Mapper
{
    public class FetusMapper : Profile
    {
        public FetusMapper()
        {
            CreateMap<Fetus, GetFetusResponse>();
        }
    }
}
