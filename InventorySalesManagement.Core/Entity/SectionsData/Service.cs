using InventorySalesManagement.Core.Entity.ApplicationData;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using InventorySalesManagement.Core.Entity.OrderData;

namespace InventorySalesManagement.Core.Entity.SectionsData;

public class Service : BaseEntity
{
    [Required(ErrorMessage = "اسم الخدمة بالعربي مطلوب")]
    [Display(Name = "اسم الخدمة بالعربي")]
    public string TitleAr { get; set; }

    [Required(ErrorMessage = "اسم الخدمة بالانجليزي مطلوب")]
    [Display(Name = "اسم الخدمة بالانجليزي")]
    public string TitleEn { get; set; }

    [Required(ErrorMessage = "وصف الخدمة مطلوب")]
    [Display(Name = "وصف الخدمة ")]
    public string Description { get; set; }

    [Display(Name = " الصورة  ")]
    public string ImgUrl { get; set; }


    //-----------------------------------------------------------------------

    [Display(Name = " السعر  ")]
    [Required(ErrorMessage = "السعر مطلوب")]
    [Range(1, 1000000, ErrorMessage = "السعر يجب ان يكون اكبر من 0")]
    public float Price { get; set; }

    //-------------------------------------------------------------------
    [ForeignKey("MainSection")]
    [Display(Name = "القسم الرئيسي")]
    [Required(ErrorMessage = "القسم الرئيسي مطلوب")]
    public int MainSectionId { get; set; }
    [Display(Name = "القسم الرئيسي")]
    public MainSection MainSection { get; set; }

    //--------------------------------------------------------------------
    public IEnumerable<Order> Orders { get; set; } = new List<Order>();



}
