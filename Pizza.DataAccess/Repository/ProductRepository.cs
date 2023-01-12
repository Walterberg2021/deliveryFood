using Pizza.DataAccess.Repository.IRepository;
using Pizza.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private AplicationDbContext _db;

        public ProductRepository(AplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product obj)
        {
            //_db.Products.Update(obj);

            var objFromDb = _db.Products.FirstOrDefault(u=>u.Id == obj.Id);
            if (objFromDb != null) 
            {
                objFromDb.Name = obj.Name;
                objFromDb.Description = obj.Description;
                objFromDb.Additional_ingredients = obj.Additional_ingredients;
                objFromDb.Prise = obj.Prise;
                objFromDb.Categoryid = obj.Categoryid;
                if(obj.imgurl != null)
                {
                    objFromDb.imgurl = obj.imgurl;
                }

            }
        }
    }
}
