using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PCBookWebApp.DAL;
using PCBookWebApp.Models;
using PCBookWebApp.Models.BookViewModel;
using PCBookWebApp.Models.BankModule;

namespace PCBookWebApp.Controllers.BankModule.Api
{
    [Authorize]
    public class CheckBooksController : Controller
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: CheckBooks
        public ActionResult Index()
        {
            var checkBooks = db.CheckBooks.Include(c => c.BankAccount);
            return View(checkBooks.ToList());
        }

        // GET: CheckBooks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CheckBook checkBook = db.CheckBooks.Find(id);
            if (checkBook == null)
            {
                return HttpNotFound();
            }
            return View(checkBook);
        }

        // GET: CheckBooks/Create
        public ActionResult Create()
        {
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "BankAccountId", "BankAccountNumber");
            return View();
        }

        // POST: CheckBooks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CheckBookId,BankAccountId,CheckBookNo,Active,CreatedBy,DateCreated,UpdateCreated")] CheckBook checkBook)
        {
            if (ModelState.IsValid)
            {
                db.CheckBooks.Add(checkBook);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "BankAccountId", "BankAccountNumber", checkBook.BankAccountId);
            return View(checkBook);
        }

        // GET: CheckBooks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CheckBook checkBook = db.CheckBooks.Find(id);
            if (checkBook == null)
            {
                return HttpNotFound();
            }
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "BankAccountId", "BankAccountNumber", checkBook.BankAccountId);
            return View(checkBook);
        }

        // POST: CheckBooks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CheckBookId,BankAccountId,CheckBookNo,Active,CreatedBy,DateCreated,UpdateCreated")] CheckBook checkBook)
        {
            if (ModelState.IsValid)
            {
                db.Entry(checkBook).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "BankAccountId", "BankAccountNumber", checkBook.BankAccountId);
            return View(checkBook);
        }

        // GET: CheckBooks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CheckBook checkBook = db.CheckBooks.Find(id);
            if (checkBook == null)
            {
                return HttpNotFound();
            }
            return View(checkBook);
        }

        // POST: CheckBooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CheckBook checkBook = db.CheckBooks.Find(id);
            db.CheckBooks.Remove(checkBook);
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
