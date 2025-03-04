using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace InventorySalesManagement.Core.Entity.SectionsData;

public class MainSection : BaseEntity
{
    [Required(ErrorMessage = "اسم الفئة بالعربي مطلوب")]
    [Display(Name = "اسم الفئة بالعربي")]
    public string TitleAr { get; set; }

    [Required(ErrorMessage = "اسم الفئة  بالانجليزي طلوب")]
    [Display(Name = "اسم الفئة بالانجليزي")]
    public string TitleEn { get; set; }

    [Required(ErrorMessage = "وصف الفئة مطلوب")]
    [Display(Name = "وصف الفئة ")]
    public string Description { get; set; }

}
