using System;
using System.Collections.Generic;

namespace NET8WebAPI_GlbExptnHndlg_IExptnHndlr.Models;

public partial class Products1
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Rate { get; set; }

    public DateTime CreatedOn { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifieddOn { get; set; }

    public int? ModifieddBy { get; set; }
}
