using GSBTest.Models;
using Microsoft.Extensions.Logging;

namespace GSBTest.Services
{
    public class LogService
    {
        private readonly GsbtestContext _context;

        public LogService()
        {
            _context = new GsbtestContext();

        }

        // Hareket logu kaydetme
        public void LogAction(int? userId, string methodName, string logType)
        {

            var log = new TblLog
            {
                UserId = userId,
                MethodName = methodName,
                LogType = logType, // "Kayıt Alındı" gibi başarılı işlem tipleri
                Hata = "IslemLog",
                ExceptionMessage = null,
                DateTime = System.DateTime.Now
            };

            try
            {
                _context.TblLogs.Add(log);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // LogError($"Log action kaydedilemedi: {ex.Message}");
            }
        }

        // Hata logu kaydetme
        public void LogError(int? userId, string methodName, string logType, string exceptionMessage)
        {

            var log = new TblLog
            {
                UserId = userId,
                MethodName = methodName,
                LogType = logType,
                Hata = "HataLog", // SQL Hatası, Null Referans Hatası gibi hata türleri
                ExceptionMessage = exceptionMessage.Substring(0, 1000),
                DateTime = System.DateTime.Now
            };

            try
            {
                _context.TblLogs.Add(log);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
