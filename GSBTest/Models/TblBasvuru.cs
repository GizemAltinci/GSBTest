using System;
using System.Collections.Generic;

namespace GSBTest.Models;

public partial class TblBasvuru
{
    public int Id { get; set; }

    public string? ProjeAdi { get; set; }

    public string? BasvuranBirim { get; set; }

    public string? BasvuruYapilanProje { get; set; }

    public string? BasvuruYapilanTur { get; set; }

    public string? KatilimciTuru { get; set; }

    public string? BasvuruDonemi { get; set; }

    public DateTime? BasvuruTarih { get; set; }

    public string? BasvuruDurumu { get; set; }

    public DateTime? AciklamaTarihi { get; set; }

    public int? HibeTutari { get; set; }
}
