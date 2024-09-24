using System;
using System.Collections.Generic;

namespace GSBTest.Models;

public partial class TblRef
{
    public int Id { get; set; }

    public string? Tip { get; set; }

    public string? Alttip { get; set; }

    public bool? SilinmeDurumu { get; set; }
}
