using System.ComponentModel.DataAnnotations;

namespace InventorySalesManagement.Core.DTO.EntityDto;

public class MainSectionDto
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
