﻿using InventorySalesManagement.Core.Entity.ApplicationData;
using InventorySalesManagement.Core.Entity.SectionsData;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using InventorySalesManagement.Core.Entity.OrderServiceData;

namespace InventorySalesManagement.Core.Entity.OrderData;

public class Order : BaseEntity
{

    [Display(Name = "رقم الطلب")]
    public string OrderNumber { get; set; }

    [Display(Name = "الاجمالى")]
    public float Total { get; set; }

    [Display(Name = "تاريخ الطلب")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime? CreatedOn { get; set; }
    public List<OrderService> OrderServices { get; set; } = new List<OrderService>();

}
