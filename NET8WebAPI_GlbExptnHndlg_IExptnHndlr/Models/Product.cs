using System;
using System.Collections.Generic;

namespace NET8WebAPI_GlbExptnHndlg_IExptnHndlr.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Sku { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public bool IsAvailable { get; set; }

    public string Category { get; set; } = null!;

    public string? Brand { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public decimal SupplierCost { get; set; }

    public string SupplierInfo { get; set; } = null!;

    public int StockQuantity { get; set; }
}
