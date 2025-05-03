using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Exception
{
    public class CustomValidationException : System.Exception
    {
        public IEnumerable<string> Errors { get; }

        public CustomValidationException(string error) : base(error)
        {
            Errors = new List<string> { error };
        }

        public CustomValidationException(IEnumerable<string> errors) : base("Validation failed")
        {
            Errors = errors;
        }
    }
}
