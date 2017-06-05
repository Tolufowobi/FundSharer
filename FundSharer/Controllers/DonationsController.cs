using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FundSharer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using FundSharer.Models;
    using DataServices;
    using Microsoft.AspNet.Identity;
    using Microsoft.Ajax.Utilities;

    namespace FundShare.Controllers
    {
        public class DonationsController : Controller
        {
            private ApplicationDbContext db = DbAccessHandler.DbContext;

            // GET: Donations
            public async Task<ActionResult> Index()
            {
                return View(await db.Donations.ToListAsync());
            }

            // GET: Donations/Details/5
            public async Task<ActionResult> Details(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Donation donation = await db.Donations.FindAsync(id);
                if (donation == null)
                {
                    return HttpNotFound();
                }
                return View(donation);
            }

            // GET: Donations/Create
            public ActionResult Create()
            {
                return View();
            }

            // POST: Donations/Create
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> Create([Bind(Include = "Id,CreationDate,IsOpen")] Donation donation)
            {
                if (ModelState.IsValid)
                {
                    db.Donations.Add(donation);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

                return View(donation);
            }

            // GET: Donations/Edit/5
            public async Task<ActionResult> Edit(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Donation donation = await db.Donations.FindAsync(id);
                if (donation == null)
                {
                    return HttpNotFound();
                }
                return View(donation);
            }

            // POST: Donations/Edit/5
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> Edit([Bind(Include = "Id,CreationDate,IsOpen")] Donation donation)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(donation).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return View(donation);
            }

            // GET: Donations/Delete/5
            public async Task<ActionResult> Delete(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Donation donation = await db.Donations.FindAsync(id);
                if (donation == null)
                {
                    return HttpNotFound();
                }
                return View(donation);
            }

            // POST: Donations/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> DeleteConfirmed(int id)
            {
                Donation donation = await db.Donations.FindAsync(id);
                db.Donations.Remove(donation);
                await db.SaveChangesAsync();
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

}