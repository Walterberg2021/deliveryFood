using Pizza.DataAccess.Repository.IRepository;
using Pizza.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.DataAccess.Repository
{
    public class OrderDetailsRepository : Repository<OrderDetail>, IOrderDetailsRepository
	{
        private AplicationDbContext _db;

        public OrderDetailsRepository(AplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderDetail obj)
        {
            _db.OrderDetail.Update(obj);
        }
    }
}
