using System;
using System.Collections.Generic;

namespace GSBTest.Models;

public partial class TblLog
{
    public int LogId { get; set; }

    public int? UserId { get; set; }

    public string? LogType { get; set; }

    public string? MethodName { get; set; }

    public string? Hata { get; set; }

    public string? ExceptionMessage { get; set; }

    public DateTime? DateTime { get; set; }

    public virtual TblAccount? User { get; set; }
}
