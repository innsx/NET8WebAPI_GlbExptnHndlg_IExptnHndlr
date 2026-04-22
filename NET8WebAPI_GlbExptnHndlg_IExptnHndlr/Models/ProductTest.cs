using System;
using System.Collections.Generic;

namespace NET8WebAPI_GlbExptnHndlg_IExptnHndlr.Models;

public partial class ProductTest
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}
