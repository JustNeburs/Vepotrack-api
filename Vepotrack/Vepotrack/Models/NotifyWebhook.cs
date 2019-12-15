using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vepotrack.API.Models
{
    public class NotifyWebhook
    {
        public String UserName { get; set; }
        public String VehicleReference { get; set; }
        public PositionAPI Position { get; set; }
    }
}
