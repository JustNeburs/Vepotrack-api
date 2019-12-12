using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vepotrack.API.DataModels;
using Vepotrack.API.Persistence.Contexts;

namespace Vepotrack.API.Repositories
{
    /// <summary>
    /// Clase base de los repositorios
    /// </summary>
    public abstract class BaseRepository
    {
        /// <summary>
        /// Contexto para los repositorios
        /// </summary>
        protected readonly ApiDbContext _context;

        public BaseRepository(ApiDbContext context)
        {
            _context = context;
        }

        
    }
}
