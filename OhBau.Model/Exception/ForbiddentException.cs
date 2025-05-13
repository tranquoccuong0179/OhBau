using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Exception
{
    public class ForbiddentException : System.Exception
    {
        public ForbiddentException(string message) : base(message)
        {
        }

        public ForbiddentException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
