namespace GSBTest.Models
{
    public class FilteredBasvuru
    {
        public List<TblRef> RefData { get; set; } // RefData isimli bir özellik tanımlar. TblRef tipinde nesneleri içeren bir listeyi temsil eder.

        public List<TblBasvuru> BasvuruListesi { get; set; } // Filtrelenmiş başvuru verileri

        public List<TblBasvuru> BasvuruListesiText { get; set; }

        public int Id { get; set; }

        public string ProjeAdi { get; set; }

        public string BasvuranBirim { get; set; }

        public string BasvuruYapilanProje { get; set; }

        public string BasvuruYapilanTur { get; set; }

        public string KatilimciTuru { get; set; }

        public string BasvuruDonemi { get; set; }

        public DateTime BasvuruTarih { get; set; }

        public string BasvuruDurumu { get; set; }

        public DateTime AciklamaTarihi { get; set; }

        public int HibeTutari { get; set; }
    }
}
