using InventorySalesManagement.Core.Entity.ApplicationData;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using InventorySalesManagement.Core.Entity.OrderData;
using InventorySalesManagement.Core.Helpers;
using InventorySalesManagement.Core.Entity.OrderServiceData;

namespace InventorySalesManagement.Core.Entity.SectionsData;

public class Service : BaseEntity
{
    [Required(ErrorMessage = "اسم المنتج بالعربي مطلوب")]
    [Display(Name = "اسم المنتج بالعربي")]
    public string TitleAr { get; set; }

    [Required(ErrorMessage = "اسم المنتج بالانجليزي مطلوب")]
    [Display(Name = "اسم المنتج بالانجليزي")]
    public string TitleEn { get; set; }

    [Required(ErrorMessage = "وصف المنتج مطلوب")]
    [Display(Name = "وصف المنتج ")]
    public string Description { get; set; }

    [Required(ErrorMessage = "الرصيد الحالي مطلوب")]
    [Display(Name = "رصيد المنتج ")]
    [Range(1, 1000000, ErrorMessage = "الرصيد يجب ان يكون اكبر من 0")]
    public int Qty { get; set; }

    [Required(ErrorMessage = "نوع المنتج مطلوب")]
    [Display(Name = "نوع المنتج ")]
    public ProductType ProductType { get; set; }


    //-----------------------------------------------------------------------

    [Display(Name = " السعر  ")]
    [Required(ErrorMessage = "السعر مطلوب")]
    [Range(1, 1000000, ErrorMessage = "السعر يجب ان يكون اكبر من 0")]
    public float Price { get; set; }

    //-------------------------------------------------------------------
    [ForeignKey("MainSection")]
    [Display(Name = "الفئة الرئيسية")]
    [Required(ErrorMessage = "الفئة الرئيسية مطلوبة")]
    public int MainSectionId { get; set; }
    [Display(Name = "الفئة الرئيسية")]
    public MainSection MainSection { get; set; }

    //--------------------------------------------------------------------
    public IEnumerable<Order> Orders { get; set; } = new List<Order>();

    public List<OrderService> OrderServices { get; set; } = new List<OrderService>();


}
