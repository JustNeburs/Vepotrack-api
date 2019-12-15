using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.Services.Interfaces;

namespace Vepotrack.API.Models
{
    public class NotifyElement
    {
        public NotifyType NotifyType { get; set; }
        public Guid UserId { get; set; }
        public String UserName { get; set; }
        public String VehicleReference { get; set; }
        public DateTime DtExpire { get; set; }
        public String Data { get; set; }        
    }
}
