using InventorySalesManagement.Core.Entity.ApplicationData;
using InventorySalesManagement.Core.Entity.OrderData;
using InventorySalesManagement.Core.Entity.SectionsData;
using InventorySalesManagement.Core;
using InventorySalesManagement.RepositoryLayer.Interfaces;
using static Azure.Core.HttpHeader;
using System.Net;

namespace InventorySalesManagement.RepositoryLayer.Repositories;


public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext _context;

    public IBaseRepository<ApplicationUser> Users { get; private set; }
    public IBaseRepository<MainSection> MainSections { get; private set; }
    public IBaseRepository<Service> Services { get; private set; }
    public IBaseRepository<Order> Orders { get; private set; }
   
    public UnitOfWork(ApplicationContext context)
    {
        _context = context;
       
        Users = new BaseRepository<ApplicationUser>(_context);
        MainSections = new BaseRepository<MainSection>(_context);
        Services = new BaseRepository<Service>(_context);
        Orders = new BaseRepository<Order>(_context);
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
