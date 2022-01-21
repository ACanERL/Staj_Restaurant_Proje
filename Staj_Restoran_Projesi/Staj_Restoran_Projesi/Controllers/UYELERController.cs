using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Staj_Restoran_Projesi.Models;



namespace Staj_Restoran_Projesi.Controllers
{
    public class UYELERController : controllerBase
    {
        private restaurantEntities db = new restaurantEntities();
        public ActionResult CIKIS()
        {
            Session["KULLANICIEPOSTA"] = null;
            RedirectToAction("Index", "Home");
            return View();
        }
        public ActionResult GIRIS()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GIRIS(UYELER Model)
        {
            var UYE = db.UYELER.FirstOrDefault(x => x.KULLANICIEPOSTA == Model.KULLANICIEPOSTA && x.KULLANICISIFRE == Model.KULLANICISIFRE);
            if (UYE != null)
            {
                Session["UyeID"] = UYE.ID;
                Session["KULLANICIEPOSTA"] = UYE;
                return RedirectToAction("Index", "MENULER",new {rID=UYE.ID });
                //return RedirectToAction("MenuAnasayfa", "MENULER");
            }
            ViewBag.HATA = "Kullanıcı Adı veya Şifre Yanlış !!";
            return View();
        }
        private void IsLogin()
        {
            throw new NotImplementedException();
        }
        public ActionResult SIFIRLA()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SIFIRLA(UYELER Model)
        {
            var UYE = db.UYELER.Where(x => x.KULLANICIEPOSTA == Model.KULLANICIEPOSTA).FirstOrDefault();
            if (UYE != null)
            {
                Guid rastgele = Guid.NewGuid();
                UYE.KULLANICISIFRE = rastgele.ToString().Substring(0, 8);
                db.SaveChanges();
                SmtpClient client = new SmtpClient("smtp.yandex.ru",587);
                client.EnableSsl = true;
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("frkngnc12345@yandex.ru", "Şifre Sıfırlama");
                mail.To.Add(Model.KULLANICIEPOSTA);
                mail.IsBodyHtml = true;
                mail.Subject = "Şifre Sıfırlama İsteği";
                mail.Body += "Merhaba " + UYE.RESTAURANTADI + "<br/> Kullanıcı Adınız " + UYE.KULLANICIEPOSTA + "<br/> Şifreniz=" + UYE.KULLANICISIFRE;
                NetworkCredential net = new NetworkCredential("ahmetcan111@yandex.ru", "Aa123456");
                client.Credentials = net;
                client.Send(mail);
                return RedirectToAction("GIRIS");
            }
            ViewBag.Hata = "Böyle Bir Mail Adresi Bulunamadı.";
            return View();
        }

        // GET: UYELER
        public ActionResult Index()
        {
            return View(db.UYELER.ToList());
        }
        // GET: UYELER/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UYELER uYELER = db.UYELER.Find(id);
            if (uYELER == null)
            {
                return HttpNotFound();
            }
            return View(uYELER);
        }

        // GET: UYELER/Create
        public ActionResult KAYIT()
        {
            return View();
        }
        // POST: UYELER/Create
        // Aşırı gönderim saldırılarından korunmak için, bağlamak istediğiniz belirli özellikleri etkinleştirin, 
        // daha fazla bilgi için bkz. https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KAYIT([Bind(Include = "ID,RESTAURANTADI,KULLANICIEPOSTA,KULLANICISIFRE,KAYITTARIHI,ONAYYARIHI,STATUS,GUID")] UYELER uYELER)
        {
            if (ModelState.IsValid)
            {
                db.UYELER.Add(uYELER);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(uYELER);
        }
        // GET: UYELER/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UYELER uYELER = db.UYELER.Find(id);
            if (uYELER == null)
            {
                return HttpNotFound();
            }
            return View(uYELER);
        }
        // POST: UYELER/Edit/5
        // Aşırı gönderim saldırılarından korunmak için, bağlamak istediğiniz belirli özellikleri etkinleştirin, 
        // daha fazla bilgi için bkz. https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,RESTAURANTADI,KULLANICIEPOSTA,KULLANICISIFRE,KAYITTARIHI,ONAYYARIHI,STATUS,GUID")] UYELER uYELER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(uYELER).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(uYELER);
        }
        // GET: UYELER/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UYELER uYELER = db.UYELER.Find(id);
            if (uYELER == null)
            {
                return HttpNotFound();
            }
            return View(uYELER);
        }

        // POST: UYELER/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            UYELER uYELER = db.UYELER.Find(id);
            db.UYELER.Remove(uYELER);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
