using GSBTest.Models;
using GSBTest.Services;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;


namespace GSBTest.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly GsbtestContext _context;
        private readonly LogService _logService;
        private  int? _userId;
        public HomeController()
        {
            _context = new GsbtestContext();
            _logService = new LogService();
        }


        // Baþvuru formunu görüntülemek için (GET)
        [HttpGet]
        public IActionResult BasvuruKayit()
        {
            var data = _context.TblRefs.ToList();
            var returnData = new APPModel();
            returnData.RefData = data;

            return View(returnData); // Baþvuru formu burada gösterilecek
        }

        // Baþvuru formunu kaydetmek için (POST)
        [HttpPost]
        public IActionResult BasvuruKayit(TblBasvuru basvuru)
        {
            var data = _context.TblRefs.ToList();
            var returnData = new APPModel();
            returnData.RefData = data;


           
            return View();
        }

        public IActionResult BasvuruListele(string? ProjeAdi, string? BasvuranBirim, string? BasvuruYapilanProje, string? BasvuruYapilanTur, string? KatilimciTuru, string? BasvuruDonemi, DateTime? BasvuruTarih, string? BasvuruDurumu, DateTime? AciklamaTarihi, int? HibeTutari)
        {



            var query = _context.TblBasvurus.AsQueryable();

            if (!string.IsNullOrEmpty(ProjeAdi))
            {
                query = query.Where(b => b.ProjeAdi.Contains(ProjeAdi));
            }

            if (!string.IsNullOrEmpty(BasvuranBirim))
            {
                query = query.Where(b => b.BasvuranBirim.Contains(BasvuranBirim));
            }

            if (!string.IsNullOrEmpty(BasvuruYapilanProje))
            {
                query = query.Where(b => b.BasvuruYapilanProje.Contains(BasvuruYapilanProje));
            }

            if (!string.IsNullOrEmpty(BasvuruYapilanTur))
            {
                query = query.Where(b => b.BasvuruYapilanTur.Contains(BasvuruYapilanTur));
            }

            if (!string.IsNullOrEmpty(KatilimciTuru))
            {
                query = query.Where(b => b.KatilimciTuru.Contains(KatilimciTuru));
            }

            if (!string.IsNullOrEmpty(BasvuruDonemi))
            {
                query = query.Where(b => b.BasvuruDonemi.Contains(BasvuruDonemi));
            }

            // DateTime türü filtreleme
            if (BasvuruTarih.HasValue) // DateTime türü için HasValue kullanýlýr
            {
                query = query.Where(b => b.BasvuruTarih == BasvuruTarih.Value); // DateTime için Contains kullanýlamaz
            }

            if (!string.IsNullOrEmpty(BasvuruDurumu))
            {
                query = query.Where(b => b.BasvuruDurumu.Contains(BasvuruDurumu));
            }

            if (AciklamaTarihi.HasValue)
            {
                query = query.Where(b => b.AciklamaTarihi == AciklamaTarihi.Value);
            }

            // int olduðu için direkt karþýlaþtýrma yaptýrdýk
            if (HibeTutari.HasValue)
            {
                query = query.Where(b => b.HibeTutari == HibeTutari.Value);
            }


            var filteredBasvurular = query.ToList();

            List<TblBasvuru> lst = new List<TblBasvuru>();
            foreach (var item in filteredBasvurular)
            {
                TblBasvuru tbl = new TblBasvuru();
                tbl.BasvuruDurumu = RefDataConvert(Convert.ToInt32(item.BasvuruDurumu));
                tbl.KatilimciTuru = RefDataConvert(Convert.ToInt32(item.KatilimciTuru));
                tbl.BasvuranBirim = RefDataConvert(Convert.ToInt32(item.BasvuranBirim));
                tbl.BasvuruDonemi = RefDataConvert(Convert.ToInt32(item.BasvuruDonemi));
                tbl.BasvuruYapilanProje = RefDataConvert(Convert.ToInt32(item.BasvuruYapilanProje));
                tbl.BasvuruYapilanTur = RefDataConvert(Convert.ToInt32(item.BasvuruYapilanTur));
                tbl.BasvuruTarih = item.BasvuruTarih;
                tbl.Id = item.Id;
                tbl.ProjeAdi = item.ProjeAdi;
                tbl.HibeTutari = item.HibeTutari;
                tbl.AciklamaTarihi = item.AciklamaTarihi;

                lst.Add(tbl);

            }
            var data = _context.TblRefs.ToList();

            FilteredBasvuru model = new FilteredBasvuru();
            model.BasvuruListesi = lst;
            model.RefData = data;



            return View(model);




        }

        //ref tablosundan aldýðým alttiplerin idleri geliyodu baþvuru listelemeye onu öne text gönderme
        public string RefDataConvert(int Ids)
        {
            string deger = string.Empty;
            if (Ids == 0 || Ids == null)
            {
                return "";
            }
            var result = _context.TblRefs.Where(x => x.Id == Ids).FirstOrDefault().Alttip;
            if (result != null)
            {
                deger = result.ToString();
            }
            return deger;
        }

        [HttpPost]
        public JsonResult SaveData(TblBasvuru tbl)
        {
            _userId = HttpContext.Session.GetInt32("UserId");
            if (tbl != null)
            {
                try
                {
                    _context.TblBasvurus.Add(tbl);
                    _context.SaveChanges();
                    _logService.LogAction(_userId, "SaveData", "Kayýt baþarýlý þekilde eklendi.");
                }
                catch (Exception ex)
                {
                    _logService.LogError(_userId, "SaveData",  "Genel Hata", ex.Message);
                }


            }
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult ExportToExcelBasvuru(string? ProjeAdi, string? BasvuranBirim, string? BasvuruYapilanProje, string? BasvuruYapilanTur, string? KatilimciTuru, string? BasvuruDonemi, DateTime? BasvuruTarih, string? BasvuruDurumu, DateTime? AciklamaTarihi, int? HibeTutari)
        {
            // Filtreleme iþlemi
            var query = _context.TblBasvurus.AsQueryable();

            if (!string.IsNullOrEmpty(ProjeAdi))
            {
                query = query.Where(b => b.ProjeAdi.Contains(ProjeAdi));
            }

            if (!string.IsNullOrEmpty(BasvuranBirim))
            {
                query = query.Where(b => b.BasvuranBirim.Contains(BasvuranBirim));
            }

            if (!string.IsNullOrEmpty(BasvuruYapilanProje))
            {
                query = query.Where(b => b.BasvuruYapilanProje.Contains(BasvuruYapilanProje));
            }

            if (!string.IsNullOrEmpty(BasvuruYapilanTur))
            {
                query = query.Where(b => b.BasvuruYapilanTur.Contains(BasvuruYapilanTur));
            }

            if (!string.IsNullOrEmpty(KatilimciTuru))
            {
                query = query.Where(b => b.KatilimciTuru.Contains(KatilimciTuru));
            }

            if (!string.IsNullOrEmpty(BasvuruDonemi))
            {
                query = query.Where(b => b.BasvuruDonemi.Contains(BasvuruDonemi));
            }

            if (BasvuruTarih.HasValue)
            {
                query = query.Where(b => b.BasvuruTarih == BasvuruTarih.Value);
            }

            if (!string.IsNullOrEmpty(BasvuruDurumu))
            {
                query = query.Where(b => b.BasvuruDurumu.Contains(BasvuruDurumu));
            }

            if (AciklamaTarihi.HasValue)
            {
                query = query.Where(b => b.AciklamaTarihi == AciklamaTarihi.Value);
            }

            if (HibeTutari.HasValue)
            {
                query = query.Where(b => b.HibeTutari == HibeTutari.Value);
            }

            var filteredBasvurular = query.ToList();
            

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Baþvuru Listesi");

                // Excel baþlýklarý
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Proje Adý";
                worksheet.Cells[1, 3].Value = "Baþvuran Birim";
                worksheet.Cells[1, 4].Value = "Baþvuru Yapýlan Proje";
                worksheet.Cells[1, 5].Value = "Baþvuru Yapýlan Tür";
                worksheet.Cells[1, 6].Value = "Katilimci Türü";
                worksheet.Cells[1, 7].Value = "Baþvuru Dönemi";
                worksheet.Cells[1, 8].Value = "Baþvuru Tarihi";
                worksheet.Cells[1, 9].Value = "Baþvuru Durumu";
                worksheet.Cells[1, 10].Value = "Hibe Tutarý";
                worksheet.Cells[1, 11].Value = "Açýklama Tarihi";

                // Filtrelenmiþ verileri ekleme ve RefDataConvert ile isimlerine çevirme
                for (int i = 0; i < filteredBasvurular.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = filteredBasvurular[i].Id;
                    worksheet.Cells[i + 2, 2].Value = filteredBasvurular[i].ProjeAdi;
                    worksheet.Cells[i + 2, 3].Value = RefDataConvert(Convert.ToInt32(filteredBasvurular[i].BasvuranBirim));
                    worksheet.Cells[i + 2, 4].Value = RefDataConvert(Convert.ToInt32(filteredBasvurular[i].BasvuruYapilanProje));
                    worksheet.Cells[i + 2, 5].Value = RefDataConvert(Convert.ToInt32(filteredBasvurular[i].BasvuruYapilanTur));
                    worksheet.Cells[i + 2, 6].Value = RefDataConvert(Convert.ToInt32(filteredBasvurular[i].KatilimciTuru));
                    worksheet.Cells[i + 2, 7].Value = RefDataConvert(Convert.ToInt32(filteredBasvurular[i].BasvuruDonemi));
                    worksheet.Cells[i + 2, 8].Value = filteredBasvurular[i].BasvuruTarih;
                    worksheet.Cells[i + 2, 9].Value = RefDataConvert(Convert.ToInt32(filteredBasvurular[i].BasvuruDurumu));
                    worksheet.Cells[i + 2, 10].Value = filteredBasvurular[i].HibeTutari;
                    worksheet.Cells[i + 2, 11].Value = filteredBasvurular[i].AciklamaTarihi;
                }

                var excelBytes = package.GetAsByteArray();
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BasvuruListesi.xlsx");
            }
        }






        // Referans ekleme formunu görüntülemek için (GET)
        [HttpGet]
        public IActionResult ReferansEkle()
        {
            return View(); // Referans ekleme formu burada gösterilecek
        }

        // Referans ekleme iþlemi (POST)
        [HttpPost]
        public IActionResult ReferansEkle(TblRef referans)
        {
            if (ModelState.IsValid) // Formdan gelen verinin doðruluðunu kontrol et
            {
                referans.SilinmeDurumu = false;  // Varsayýlan olarak silinmiþ deðil
                _context.TblRefs.Add(referans);  // Veritabanýna ekleniyor
                _context.SaveChanges();
            }

                
            return View(referans);
        }


        [HttpPost]
        public JsonResult SaveRef(TblRef tbl)
        {
            _userId = HttpContext.Session.GetInt32("UserId");
            if (tbl != null)
            {
                try
                {
                    tbl.SilinmeDurumu = true;
                    _context.TblRefs.Add(tbl);
                    _context.SaveChanges();
                    _logService.LogAction(_userId, "SaveRef", "Kayýt baþarýlý þekilde eklendi.");
                }
                catch (Exception ex)
                {
                    _logService.LogError(_userId, "SaveRef", "Genel Hata", ex.Message);
                }


            }
            return Json(new { success = true });
        }

        



        public IActionResult ReferansListele()
        {
            // SilinmeDurumu true olan referanslarý getir (eðer null ise false kabul edilir)
            ViewBag.Referanslar = _context.TblRefs.Where(r => r.SilinmeDurumu == true).ToList();



            return View();
        }


        public JsonResult DeleteReference(int id)
        {

            var item = _context.TblRefs.Find(id);

            if (item != null)
            {
                item.SilinmeDurumu = false; // SilinmeDurumu'nu false olarak güncelle
                _context.SaveChanges();
                return Json(new { success = true, message = "Referans baþarýyla silindi." });
            }

            return Json(new { success = false, message = "Referans bulunamadý." });
        }


        [HttpPost]
        public IActionResult ExportToExcelReferans()
        {
            // Tüm referans verilerini alýyoruz
            var referanslar = _context.TblRefs.ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Referans Listesi");

                // Excel baþlýklarý
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Tip";
                worksheet.Cells[1, 3].Value = "Alttip";
                

                // Verileri Excel'e ekleme
                for (int i = 0; i < referanslar.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = referanslar[i].Id;
                    worksheet.Cells[i + 2, 2].Value = referanslar[i].Tip;
                    worksheet.Cells[i + 2, 3].Value = referanslar[i].Alttip;
                    
                }

                // Excel dosyasýný oluþtur ve indir
                var excelBytes = package.GetAsByteArray();
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReferansListesi.xlsx");
            }
        }




        // Kullanýcý ekleme formunu görüntülemek için (GET)
        [HttpGet]
        public IActionResult KullaniciEkle()
        {
            return View();  // Kullanýcý ekleme formunu göster
        }

        // Kullanýcý ekleme iþlemi (POST)
        [HttpPost]
        public IActionResult KullaniciEkle(TblAccount kullanici)
        {
            if (ModelState.IsValid)  // Formdan gelen verinin doðruluðunu kontrol et
            {
                kullanici.KayitDurumu = true;    // Varsayýlan olarak aktif
                kullanici.SillinmeDurumu = false; // Silinme durumu false olarak ayarlanýyor
                _context.TblAccounts.Add(kullanici);  // Veritabanýna ekleniyor
                _context.SaveChanges();              // Veritabaný kaydediliyor

                
            }

            // Hata durumunda form tekrar gösterilecek
            return View(kullanici);
        }



        [HttpPost]
        public JsonResult SaveUserIslem(TblAccount tbl)
        {
            _userId = HttpContext.Session.GetInt32("UserId");
            if (tbl != null)
            {
                try
                {
                    tbl.SillinmeDurumu = false;
                    tbl.KayitDurumu = true;
                    _context.TblAccounts.Add(tbl);
                    _context.SaveChanges();
                    _logService.LogAction(_userId, "SaveUserIslem", "Kayýt baþarýlý þekilde eklendi.");
                }
                catch (Exception ex)
                {
                    _logService.LogError(_userId, "SaveUserIslem", "Genel Hata", ex.Message);
                }


            }
            return Json(new { success = true });
        }






      
        public IActionResult KullaniciListele(string KullaniciAdi, bool? KayitDurumu)
        {
            var query = _context.TblAccounts.AsQueryable();

            if (!string.IsNullOrEmpty(KullaniciAdi))
            {
                query = query.Where(b => b.KullaniciAdi.Contains(KullaniciAdi));
            }

            if (KayitDurumu.HasValue)
            {
                query = query.Where(x => x.KayitDurumu == KayitDurumu.Value);
            }

            var data = query.Where(r => r.SillinmeDurumu == false).ToList();

            var model = new FilteredKullanici
            {
                KullaniciAdi = KullaniciAdi,
                KayitDurumu = KayitDurumu,
                UserData = data
            };

            ViewBag.register = _context.TblAccounts.Where(r => r.SillinmeDurumu == false).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult ExportToExcelKullanici(string KullaniciAdi, bool? KayitDurumu)
        {
            var query = _context.TblAccounts.AsQueryable();

            if (!string.IsNullOrEmpty(KullaniciAdi))
            {
                query = query.Where(b => b.KullaniciAdi.Contains(KullaniciAdi));
            }

            if (KayitDurumu.HasValue)
            {
                query = query.Where(x => x.KayitDurumu == KayitDurumu.Value);
            }

            var data = query.Where(r => r.SillinmeDurumu == false).ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Kullanici");
                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Kullanýcý Adý";
                worksheet.Cells[1, 3].Value = "Email";
                worksheet.Cells[1, 4].Value = "Þifre";
                worksheet.Cells[1, 5].Value = "Silinme Durumu";
                worksheet.Cells[1, 6].Value = "Kayýt Durumu";

                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].Id;
                    worksheet.Cells[i + 2, 2].Value = data[i].KullaniciAdi;
                    worksheet.Cells[i + 2, 3].Value = data[i].KullaniciEmail;
                    worksheet.Cells[i + 2, 4].Value = data[i].KullaniciSifre;
                    worksheet.Cells[i + 2, 5].Value = data[i].SillinmeDurumu ? "Silindi" : "Aktif";
                    worksheet.Cells[i + 2, 6].Value = data[i].KayitDurumu ? "Aktif" : "Pasif";
                }

                var excelBytes = package.GetAsByteArray();
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Kullanici.xlsx");

                



            }
        }




        [HttpPost]
        public JsonResult FuncKayitDurumu(int id, bool status)
        {
            try
            {
                var user = _context.TblAccounts.FirstOrDefault(u => u.Id == id);
                if (user != null)
                {
                    user.KayitDurumu = status;
                    _context.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Kullanýcý bulunamadý." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public JsonResult DeleteRegister(int id)
        {

            var item = _context.TblAccounts.Find(id);

            if (item != null)
            {
                item.SillinmeDurumu = true; // SilinmeDurumu'nu false olarak güncelle
                _context.SaveChanges();
                return Json(new { success = true, message = "Kayýt baþarýyla silindi." });
            }

            return Json(new { success = false, message = "Kayýt bulunamadý." });
        }


        public IActionResult HomePage()
        {


            return View();
        }


    }
}








//public void SaveAllRefDataToBasvuru()
//{
// Tüm TblRef kayýtlarýný alýyoruz
//var refKayitlar = _context.TblRef.Where(x => !x.SilinmeDurumu).ToList();

// Her bir TblRef kaydýný TblBasvuru'ya ekle
//foreach (var refKayit in refKayitlar)
//{
//	TblBasvuru yeniBasvuru = new TblBasvuru
//	{
//		ProjeAdi = "Örnek Proje", // Bu alaný ihtiyaçlarýnýza göre ayarlayabilirsiniz
//		BasvuranBirim = refKayit.Tip == "BasvuranBirim" ? refKayit.Alttip : null,
//		BasvuruYapilanProje = refKayit.Tip == "BasvuruYapilanProje" ? refKayit.Alttip : null,
//		BasvuruYapilanTur = refKayit.Tip == "BasvuruYapilanTur" ? refKayit.Alttip : null,
//		KatilimciTuru = refKayit.Tip == "KatilimciTuru" ? refKayit.Alttip : null,
//		BasvuruDonemi = refKayit.Tip == "BasvuruDonemi" ? refKayit.Alttip : null,
//		BasvuruTarih = DateTime.Now,  // Basvuru tarihini þimdi olarak ayarlýyoruz
//		BasvuruDurumu = refKayit.Tip == "BasvuruDurumu" ? refKayit.Alttip : null,
//		AciklamaTarihi = DateTime.Now.AddMonths(1),  // Açýklama tarihini 1 ay sonrasý olarak ayarlýyoruz
//		HibeTutari = 10000  // Örnek bir hibe tutarý
//	};

//	_context.TblBasvurus.Add(yeniBasvuru);
//}

// Deðiþiklikleri veritabanýna kaydet
//            _context.SaveChanges();
//        }
//    }
//}






