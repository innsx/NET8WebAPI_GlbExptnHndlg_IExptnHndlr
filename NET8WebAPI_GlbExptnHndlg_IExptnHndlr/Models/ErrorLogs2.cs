using System;
using System.Collections.Generic;

namespace NET8WebAPI_GlbExptnHndlg_IExptnHndlr.Models;

public partial class ErrorLogs2
{
    public int? ErrorNumber { get; set; }

    public int? ErrorState { get; set; }

    public int? ErrorSeverity { get; set; }

    public int? ErrorLine { get; set; }

    public string? ErrorProcedure { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime? ErrorDateTime { get; set; }
}
