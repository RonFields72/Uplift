using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Uplift.DataAccess.Data.Repository.IRepository;
using Uplift.Models;

namespace Uplift.DataAccess.Data.Repository
{
    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        private readonly ApplicationDbContext _db;

        public ServiceRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Service service)
        {
            var serviceFromDb = _db.Service.FirstOrDefault(s => s.Id == service.Id);

            serviceFromDb.Name = service.Name;
            serviceFromDb.LongDesc = service.LongDesc;
            serviceFromDb.Price = service.Price;
            serviceFromDb.ImageUrl = service.ImageUrl;
            serviceFromDb.FrequencyId = service.FrequencyId;
            serviceFromDb.CategoryId = service.CategoryId;

            _db.SaveChanges();
        }
    }
}
