using System;
using System.Collections.Generic;

namespace NET8WebAPI_GlbExptnHndlg_IExptnHndlr.Models;

public partial class Log1
{
    public int Id { get; set; }

    public string? Message { get; set; }

    public string? MessageTemplate { get; set; }

    public string? Level { get; set; }

    public DateTime? TimeStamp { get; set; }

    public string? Exception { get; set; }

    public string? Properties { get; set; }
}
