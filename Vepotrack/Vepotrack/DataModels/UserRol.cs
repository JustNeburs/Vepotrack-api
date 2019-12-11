using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vepotrack.API.DataModels
{
    /// <summary>
    /// Clase para definir si un usuario tiene permisos para una categoria
    /// </summary>
    public class UserRol : IdentityRole<Guid>
    {      
    }
}
