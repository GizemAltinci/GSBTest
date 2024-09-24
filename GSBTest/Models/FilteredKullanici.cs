namespace GSBTest.Models
{
    public class FilteredKullanici
    {
        public string KullaniciAdi { get; set; }
        public bool? KayitDurumu { get; set; }
        public List<TblAccount> UserData { get; set; }
    }
}
