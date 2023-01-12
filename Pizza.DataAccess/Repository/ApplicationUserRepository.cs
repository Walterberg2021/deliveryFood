using Pizza.DataAccess.Repository.IRepository;
using Pizza.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private AplicationDbContext _db;

        public ApplicationUserRepository(AplicationDbContext db) : base(db)
        {
            _db = db;
        }

    }
}
