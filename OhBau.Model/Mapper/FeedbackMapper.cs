using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OhBau.Model.Entity;
using OhBau.Model.Payload.Request.Feedback;
using OhBau.Model.Payload.Response.Feedback;

namespace OhBau.Model.Mapper
{
    public class FeedbackMapper : Profile
    {
        public FeedbackMapper()
        {
            CreateMap<CreateFeedbackRequest, Feedback>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

            CreateMap<Feedback, CreateFeedbackResponse>();

            CreateMap<Feedback, GetFeedback>();

        }
    }
}
