using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OhBau.Model.Entity;
using OhBau.Model.Payload.Response.FetusResponse;

namespace OhBau.Model.Mapper
{
    public class FetusDetailMapper : Profile
    {
        public FetusDetailMapper()
        {
            CreateMap<FetusDetail, GetFetusDetailResponse>();
        }
    }
}
