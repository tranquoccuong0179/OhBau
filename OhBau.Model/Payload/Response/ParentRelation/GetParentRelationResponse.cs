using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhBau.Model.Payload.Response.Fetus;
using OhBau.Model.Payload.Response.Parent;

namespace OhBau.Model.Payload.Response.ParentRelation
{
    public class GetParentRelationResponse
    {
        public GetParentResponse? Father { get; set; }
        public GetParentResponse? Mother { get; set; }
        public List<GetFetusResponse>? Fetuses { get; set; }
    }
}
