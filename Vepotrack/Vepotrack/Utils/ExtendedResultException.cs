using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vepotrack.API.Utils
{
    public class ExtendedResultException : Exception
    {
        public Microsoft.AspNetCore.Mvc.IActionResult ExtendedResult { get; set; }
        public ExtendedResultException(Microsoft.AspNetCore.Mvc.IActionResult result, String msg = null): base(msg)
        {
            ExtendedResult = result;
        }

    }
}
