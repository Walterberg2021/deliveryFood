﻿using Pizza.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private AplicationDbContext _db;

        public UnitOfWork(AplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Product= new ProductRepository(_db);
            ApplicationUser= new ApplicationUserRepository(_db);
            ShoppingCart= new ShoppingCartRepository(_db);
            OrderDetails = new OrderDetailsRepository(_db);
            OrderHeader= new OrderHeaderRepository(_db);
        }
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IOrderDetailsRepository OrderDetails { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
