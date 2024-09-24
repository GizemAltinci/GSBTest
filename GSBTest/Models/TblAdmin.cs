using System;
using System.Collections.Generic;

namespace GSBTest.Models;

public partial class TblAdmin
{
    public int Id { get; set; }

    public string? AdminAdi { get; set; }

    public string? AdminSoyadi { get; set; }

    public string? AdminEmail { get; set; }

    public string? AdminSifre { get; set; }

    public bool? IsAdmin { get; set; }
}
