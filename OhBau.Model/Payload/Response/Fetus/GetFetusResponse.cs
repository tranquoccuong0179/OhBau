using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Payload.Response.FetusResponse;

namespace OhBau.Model.Payload.Response.Fetus
{
    public class GetFetusResponse
    {
        public Guid? Id { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? Name { get; set; } = null!;

        public string? Code { get; set; } = null!;

        public List<GetFetusDetailResponse>? FetusDetails { get; set; }

    }
}
