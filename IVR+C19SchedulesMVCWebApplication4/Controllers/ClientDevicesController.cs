using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IVR_C19SchedulesMVCWebApplication4.Models;

namespace IVR_C19SchedulesMVCWebApplication4.Controllers
{
    public class ClientDevicesController : Controller
    {
        private ClientDevicesContext db = new ClientDevicesContext();

        // GET: ClientDevices
        public ActionResult Index()
        {
            return View(db.ClientDevices.ToList());
        }

        // GET: ClientDevices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientDevice clientDevice = db.ClientDevices.Find(id);
            if (clientDevice == null)
            {
                return HttpNotFound();
            }
            return View(clientDevice);
        }

        // GET: ClientDevices/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClientDevices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClientDeviceId,ClientId,DeviceList")] ClientDevice clientDevice)
        {
            if (ModelState.IsValid)
            {
                db.ClientDevices.Add(clientDevice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(clientDevice);
        }

        // GET: ClientDevices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientDevice clientDevice = db.ClientDevices.Find(id);
            if (clientDevice == null)
            {
                return HttpNotFound();
            }
            return View(clientDevice);
        }

        // POST: ClientDevices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClientDeviceId,ClientId,DeviceList")] ClientDevice clientDevice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientDevice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clientDevice);
        }

        // GET: ClientDevices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientDevice clientDevice = db.ClientDevices.Find(id);
            if (clientDevice == null)
            {
                return HttpNotFound();
            }
            return View(clientDevice);
        }

        // POST: ClientDevices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClientDevice clientDevice = db.ClientDevices.Find(id);
            db.ClientDevices.Remove(clientDevice);
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
