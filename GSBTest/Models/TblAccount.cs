using System;
using System.Collections.Generic;

namespace GSBTest.Models;

public partial class TblAccount
{
    public int Id { get; set; }

    public string KullaniciAdi { get; set; } = null!;

    public string KullaniciEmail { get; set; } = null!;

    public string KullaniciSifre { get; set; } = null!;

    public bool SillinmeDurumu { get; set; }

    public bool KayitDurumu { get; set; }

    public bool? YetkiBasvuruIslemleri { get; set; }

    public bool? YetkiReferansIslemleri { get; set; }

    public bool? YetkiKullaniciIslemleri { get; set; }

    public virtual ICollection<TblLog> TblLogs { get; set; } = new List<TblLog>();
}
