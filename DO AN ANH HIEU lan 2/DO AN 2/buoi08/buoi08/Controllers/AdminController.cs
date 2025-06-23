using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using buoi08.ADMIN;

namespace buoi08.Controllers
{
    public class AdminController : Controller
    {
        private Doanltweb3Entities2 db = new Doanltweb3Entities2();

        // GET: Admin
        public ActionResult Index()
        {
            var hoas = db.Hoas.Include(h => h.LoaiHoa).Include(h => h.NhaCungCap);

            return View(hoas.ToList());
        }

        // GET: Admin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hoa hoa = db.Hoas.Find(id);
            if (hoa == null)
            {
                return HttpNotFound();
            }
            return View(hoa);
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            ViewBag.MaLoai = new SelectList(db.LoaiHoas, "MaLoai", "TenLoai");
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps, "MaNCC", "TenNCC");
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaHoa,TenHoa,GiaBan,MoTa,Anh,SoLuongTon,MaNCC,MaLoai")] Hoa hoa)
        {
            if (ModelState.IsValid)
            {
                db.Hoas.Add(hoa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaLoai = new SelectList(db.LoaiHoas, "MaLoai", "TenLoai", hoa.MaLoai);
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps, "MaNCC", "TenNCC", hoa.MaNCC);
            return View(hoa);
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hoa hoa = db.Hoas.Find(id);
            if (hoa == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaLoai = new SelectList(db.LoaiHoas, "MaLoai", "TenLoai", hoa.MaLoai);
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps, "MaNCC", "TenNCC", hoa.MaNCC);
            return View(hoa);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaHoa,TenHoa,GiaBan,MoTa,Anh,SoLuongTon,MaNCC,MaLoai")] Hoa hoa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hoa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaLoai = new SelectList(db.LoaiHoas, "MaLoai", "TenLoai", hoa.MaLoai);
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps, "MaNCC", "TenNCC", hoa.MaNCC);
            return View(hoa);
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hoa hoa = db.Hoas.Find(id);
            if (hoa == null)
            {
                return HttpNotFound();
            }
            return View(hoa);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Hoa hoa = db.Hoas.Find(id);
            db.Hoas.Remove(hoa);
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
        public ActionResult Login()
        {
            return View(); 
        }

    }
}
