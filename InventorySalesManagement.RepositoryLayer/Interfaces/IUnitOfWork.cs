using InventorySalesManagement.Core.Entity.ApplicationData;
using InventorySalesManagement.Core.Entity.OrderData;
using InventorySalesManagement.Core.Entity.SectionsData;
using System.Drawing;
using System.Net;

namespace InventorySalesManagement.RepositoryLayer.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IBaseRepository<ApplicationUser> Users { get; }

    //-----------------------------------------------------------------------------------

    IBaseRepository<MainSection> MainSections { get; }
    IBaseRepository<Service> Services { get; }
    IBaseRepository<Order> Orders { get; }
   
  
    //-----------------------------------------------------------------------------------
    int SaveChanges();

    Task<int> SaveChangesAsync();
}
