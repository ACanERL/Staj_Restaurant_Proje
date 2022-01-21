using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Configuration;
using Staj_Restoran_Projesi.Models;
using System.Web.Helpers;

using System.Drawing;
using QRCoder;
using System.Drawing.Imaging;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace Staj_Restoran_Projesi.Controllers
{   
    public class MENULERController : controllerBase
    {
        public string url2;
        public string url1;
        int rID;
        string rid;
        private restaurantEntities db = new restaurantEntities();
        // GET: MENULER       
        public ActionResult Index()
        {
            url1 = HttpContext.Request.Url.AbsoluteUri;
            Session["url2"] = url1;
            rid= Session["UyeID"].ToString();
            rID = Int32.Parse(rid);
            //int rID = (int)Session["UyeID"];
            List<MENULER> plist = db.MENULER.Where(p => p.UYEID == rID).ToList();
            return View(plist);
        }
        // GET: MENULER/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MENULER mENULER = db.MENULER.Find(id);
            if (mENULER == null)
            {
                return HttpNotFound();
                
            }
            return View(mENULER);
        }
        // GET: MENULER/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.UYEID = new SelectList(db.UYELER, "ID", "RESTAURANTADI");
            return View();
        }
        // POST: MENULER/Create
        // Aşırı gönderim saldırılarından korunmak için, bağlamak istediğiniz belirli özellikleri etkinleştirin, 
        // daha fazla bilgi için bkz. https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UYEID,FIYAT,ACIKLAMA,RESIM,SIRA,MENUGRUBU,STATUS,KAYITTARIHI,GUNCELLEMETARIHI")] MENULER mENULER,HttpPostedFileBase file)
        {
            if (file != null)
            {
                string ds = file.FileName.Substring(file.FileName.Length - 3);
                string p = string.Empty;
                p = Server.MapPath("~/Resimler/");
                file.SaveAs(p + file.FileName);

            }
            using (db)
            {
                mENULER.RESIM = file.FileName;
                db.MENULER.Add(mENULER);
                db.SaveChanges();
            }            
                if (ModelState.IsValid)
                {          
                   // db.MENULER.Add(mENULER);
                  //  db.SaveChanges();
                    return RedirectToAction("Index",new{rID=mENULER.UYEID});
                }
            
            ViewBag.UYEID = new SelectList(db.UYELER, "ID", "RESTAURANTADI");
            return View(mENULER);
        }

        // GET: MENULER/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MENULER mENULER = db.MENULER.Find(id);
            if (mENULER == null)
            {
                return HttpNotFound();
            }
            ViewBag.UYEID = new SelectList(db.UYELER, "ID", "RESTAURANTADI", mENULER.UYEID);
            return View(mENULER);
        }

        // POST: MENULER/Edit/5
        // Aşırı gönderim saldırılarından korunmak için, bağlamak istediğiniz belirli özellikleri etkinleştirin, 
        // daha fazla bilgi için bkz. https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UYEID,FIYAT,ACIKLAMA,RESIM,SIRA,MENUGRUBU,STATUS,KAYITTARIHI,GUNCELLEMETARIHI")] MENULER mENULER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mENULER).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MenuAnasayfa");
            }
            ViewBag.UYEID = new SelectList(db.UYELER, "ID", "RESTAURANTADI", mENULER.UYEID);
            return View(mENULER);
        }

        // GET: MENULER/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MENULER mENULER = db.MENULER.Find(id);
            if (mENULER == null)
            {
                return HttpNotFound();
            }
            return View(mENULER);
        }

        // POST: MENULER/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            MENULER mENULER = db.MENULER.Find(id);
            db.MENULER.Remove(mENULER);
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
        public ActionResult MenuAnasayfa(string id)
        {
            
            return View();
        }
       public ActionResult indir()
        {
            string yeniurl = Session["url2"].ToString();
            MENULERController n = new MENULERController();           
            using (MemoryStream ms = new MemoryStream())
            {               
                QRCodeGenerator oQRCodeGenerator = new QRCodeGenerator();             
                QRCodeData oQRCodeData = oQRCodeGenerator.CreateQrCode(yeniurl, QRCodeGenerator.ECCLevel.Q);
                QRCode oQRCode = new QRCode(oQRCodeData);
                System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
                imgBarCode.Height = 150;
                imgBarCode.Width = 150;
                using (Bitmap oBitmap = oQRCode.GetGraphic(20))
                {
                    oBitmap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
                string base64 = Convert.ToBase64String(ms.ToArray());
                byte[] imageBytes = Convert.FromBase64String(base64);
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageBytes);
                using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                {
                    Document document = new Document(PageSize.A1, 0F, 0F, 0F, 0F);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();
                    document.Add(image);
                    document.Close();
                    byte[] bytes = memoryStream.ToArray();
                    memoryStream.Close();
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", "attachment; filename=Image.pdf");
                    Response.ContentType = "application/pdf";
                    Response.Buffer = true;
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.BinaryWrite(bytes);
                    Response.End();
                }
            }
            return View();
        }
       
    }
}
