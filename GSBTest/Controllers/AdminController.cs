using GSBTest.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace GSBTest.Controllers
{



    public class AdminController : Controller
    {

        private readonly GsbtestContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {

            _logger = logger;
            _context = new GsbtestContext();
        }


        //[HttpPost]
        public ActionResult Admin(string AdminAdi, string AdminSoyadi, string AdminEmail, string AdminSifre)
        {
            var admin = _context.TblAdmins
                .FirstOrDefault(u => (u.AdminAdi == AdminAdi) && (u.AdminSoyadi == AdminSoyadi) && (u.AdminEmail == AdminEmail) && (u.AdminSifre == AdminSifre));

            if (admin != null)
            {
                bool isAdmin = admin.IsAdmin ?? false; // Nullable boolean'ı normal boolean'a çevirdik,yoksa false dedik.
                HttpContext.Session.SetInt32("IsAdmin", isAdmin ? 1 : 0);
                return RedirectToAction("HomePage", "Home");
            }

            ViewBag.ErrorMessage = "Geçersiz admin bilgileri, kontrol ediniz.";
            return View();
        }

        public IActionResult Yetkilendirme()
        {
            // Kayıt durumu aktif olan kullanıcıları al
            var aktifKullanicilar = _context.TblAccounts
                .Where(x => x.KayitDurumu == true && x.SillinmeDurumu == false)
                .ToList();

            return View(aktifKullanicilar);
        }

        [HttpPost]
        public IActionResult ChangePermission(int userId, string permissionType, bool isChecked)
        {
            var user = _context.TblAccounts.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                switch (permissionType)
                {
                    case "Basvuru":
                        user.YetkiBasvuruIslemleri = isChecked;
                        break;
                    case "Referans":
                        user.YetkiReferansIslemleri = isChecked;
                        break;
                    case "Kullanici":
                        user.YetkiKullaniciIslemleri = isChecked;
                        break;
                }

                _context.SaveChanges();
            }

            return Ok();
        }

        public IActionResult Logout()
        {
            // Session'ı temizle
            HttpContext.Session.Clear();

            // Admin logout olduktan sonra admin login sayfasına yönlendirme
            return RedirectToAction("Admin", "Admin");
        }








    }
}



