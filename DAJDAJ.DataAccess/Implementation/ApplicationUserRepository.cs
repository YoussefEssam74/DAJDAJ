using DAJDAJ.Entities;
using DAJDAJ.Entities.Models;
using DAJDAJ.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAJDAJ.DataAccess.Implementation
{
    public class ApplicationUserRepository:GenericRepository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;
        public ApplicationUserRepository(ApplicationDbContext context) :base(context) 
        {
            _context = context;
        }

   
    }
}
