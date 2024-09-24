using GSBTest.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
//using System.Web.Security;



namespace GSBTest.Controllers
{
    public class AccountController : Controller
    {
        private readonly GsbtestContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {

            _logger = logger;
            _context = new GsbtestContext();
        }


        public async Task<IActionResult> Login(string KullaniciAdi, string KullaniciSifre)
        {
            var users = _context.TblAccounts
                .FirstOrDefault(u => (u.KullaniciAdi == KullaniciAdi) && u.KullaniciSifre == KullaniciSifre);

            if (users != null)
            {
                // Eğer kullanıcı pasifse
                if (users.KayitDurumu == false)
                {
                    // Pasif sayfasına yönlendir
                    return RedirectToAction("Pasif", "Account");
                }

                // Kullanıcı aktifse, kimlik doğrulama işlemini başlat
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, users.KullaniciAdi),
            new Claim("UserId", users.Id.ToString()) // Kullanıcı ID'sini Claim olarak ekle
        };

                // Yetkiler için de claimler eklenebilir
                if (users.YetkiBasvuruIslemleri.HasValue && users.YetkiBasvuruIslemleri.Value)
                    claims.Add(new Claim("BasvuruYetkisi", "true"));
                if (users.YetkiReferansIslemleri.HasValue && users.YetkiReferansIslemleri.Value)
                    claims.Add(new Claim("ReferansYetkisi", "true"));
                if (users.YetkiKullaniciIslemleri.HasValue && users.YetkiKullaniciIslemleri.Value)
                    claims.Add(new Claim("KullaniciYetkisi", "true"));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Kimlik doğrulama işlemini başlat
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // Session bilgilerini ekleme (isteğe bağlı)
                HttpContext.Session.SetInt32("UserId", users.Id);
                HttpContext.Session.SetInt32("HasBasvuruYetkisi", users.YetkiBasvuruIslemleri.HasValue && users.YetkiBasvuruIslemleri.Value ? 1 : 0);
                HttpContext.Session.SetInt32("HasReferansYetkisi", users.YetkiReferansIslemleri.HasValue && users.YetkiReferansIslemleri.Value ? 1 : 0);
                HttpContext.Session.SetInt32("HasKullaniciYetkisi", users.YetkiKullaniciIslemleri.HasValue && users.YetkiKullaniciIslemleri.Value ? 1 : 0);

                return RedirectToAction("HomePage", "Home");
            }

            // Geçersiz kullanıcı adı veya şifre
            ViewBag.ErrorMessage = "Geçersiz kullanıcı adı veya şifre.";
            return View();
        }

        public IActionResult Pasif()
        {

            return View();

        }



        public IActionResult Register()
        {

            return View();

        }

        [HttpPost]
        public JsonResult SaveUser(TblAccount tbl)
        {
            if (tbl != null)
            {
                tbl.SillinmeDurumu = false;
                tbl.KayitDurumu = false;
                _context.TblAccounts.Add(tbl);
                _context.SaveChanges();
            }
            return Json(new { success = true });
        }


        public async Task<IActionResult> Logout()
        {
            // Kimlik doğrulama oturumunu sonlandır
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Session'ı temizle
            HttpContext.Session.Clear();

            // Kullanıcı çıkış yaptıktan sonra login sayfasına yönlendirme
            return RedirectToAction("Login", "Account");
        }




        public IActionResult AccessDenied()
        {

            return View();

        }







    }
}
