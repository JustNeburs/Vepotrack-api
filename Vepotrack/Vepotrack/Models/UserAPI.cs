using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Vepotrack.API.Models
{
    /// <summary>
    /// Clase de usuario para usar externamente en la API
    /// </summary>
    public class UserAPI
    {
        /// <summary>
        /// Id del usuario de la API
        /// </summary>
        [JsonProperty(PropertyName ="Id")]
        public String Id { get; set; }
        /// <summary>
        /// Login
        /// </summary>
        [JsonProperty(PropertyName = "Username")]
        public String Username { get; set; }
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        [JsonProperty(PropertyName = "Name")]
        public String Name { get; set; }
        /// <summary>
        /// Password, solo para establecerlo
        /// </summary>
        [JsonProperty(PropertyName = "Password", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public String Password { get; set; }
        /// <summary>
        /// Ultimo login
        /// </summary>
        [JsonProperty(PropertyName = "LastLogin", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public DateTime? LastLogin { get; set; }
    }
}
