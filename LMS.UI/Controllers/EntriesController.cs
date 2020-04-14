using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.DATA;
using Microsoft.AspNet.Identity;

namespace LMS.UI.Controllers
{
    public class EntriesController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: Entries
        public ActionResult Index()
        {
            var currentUser = User.Identity.GetUserId();

            var userAssets = db.Entries.Where(x => x.UserEntryId == currentUser);
            var Gold = userAssets.Where(x => x.Metal.Metal_Name == "Gold");
            var Silver = userAssets.Where(x => x.Metal.Metal_Name == "Silver");
            var totalSilver2 = Silver.Any() ? Silver.Sum(x => x.AmountOfPurchase) : 0;
            var totalGold2 = Gold.Any() ? Gold.Sum(x => x.AmountOfPurchase) : 0;
            ViewBag.totalSilver = totalSilver2;
            ViewBag.totalGold = totalGold2;
            return View(db.Entries.Where(x => x.UserEntryId == currentUser).ToList());

            //var entries = db.Entries.Include(e => e.Metal).Include(e => e.UserDetail);
            //return View(entries.ToList());
        }

        // GET: Entries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entry entry = db.Entries.Find(id);
            if (entry == null)
            {
                return HttpNotFound();
            }
            return View(entry);
        }

        // GET: Entries/Create
        public ActionResult Create()
        {
            //ViewBag.MetalType = new SelectList(db.Metals, "MetalId", "Metal_Name");
            //ViewBag.UserEntryId = new SelectList(db.UserDetails, "UserId", "FirstName");
            var currentUser = User.Identity.GetUserId();
            ViewBag.UserEntryID = new SelectList(db.AspNetUsers.Where(x => x.Id == currentUser), "Id", "Email");
            ViewBag.MetalType = new SelectList(db.Metals, "MetalId", "Metal_Name");
            return View();
        }

        // POST: Entries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EntryId,UserEntryId,MetalType,PlaceOfPurchase,DateOfPurchase,AmountOfPurchase")] Entry entry)
        {
            if (ModelState.IsValid)
            {
                db.Entries.Add(entry);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MetalType = new SelectList(db.Metals, "MetalId", "Metal_Name", entry.MetalType);
            ViewBag.UserEntryId = new SelectList(db.UserDetails, "UserId", "FirstName", entry.UserEntryId);
            return View(entry);
        }

        // GET: Entries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entry entry = db.Entries.Find(id);
            if (entry == null)
            {
                return HttpNotFound();
            }
            ViewBag.MetalType = new SelectList(db.Metals, "MetalId", "Metal_Name", entry.MetalType);
            ViewBag.UserEntryId = new SelectList(db.UserDetails, "UserId", "FirstName", entry.UserEntryId);
            return View(entry);
        }

        // POST: Entries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EntryId,UserEntryId,MetalType,PlaceOfPurchase,DateOfPurchase,AmountOfPurchase")] Entry entry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(entry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MetalType = new SelectList(db.Metals, "MetalId", "Metal_Name", entry.MetalType);
            ViewBag.UserEntryId = new SelectList(db.UserDetails, "UserId", "FirstName", entry.UserEntryId);
            return View(entry);
        }

        // GET: Entries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entry entry = db.Entries.Find(id);
            if (entry == null)
            {
                return HttpNotFound();
            }
            return View(entry);
        }

        // POST: Entries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Entry entry = db.Entries.Find(id);
            db.Entries.Remove(entry);
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
