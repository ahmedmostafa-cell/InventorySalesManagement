using System.ComponentModel.DataAnnotations;

namespace InventorySalesManagement.Core.DTO.EntityDto;

public class ServiceDto
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


    //----------------------------------------
    [Display(Name = " السعر  ")]
    [Required(ErrorMessage = "السعر مطلوب")]
    [Range(1, 1000000, ErrorMessage = "السعر يجب ان يكون اكبر من 0")]
    public float Price { get; set; }

    //------------------------------------
    [Display(Name = "القسم الرئيسي")]
    [Required(ErrorMessage = "القسم الرئيسي مطلوب")]
    public int MainSectionId { get; set; }

}
