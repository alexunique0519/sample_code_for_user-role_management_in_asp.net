using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using YYPatient.Models;

namespace YYPatient.Controllers
{
    public class YYPatientController : Controller
    {
        private patientSQLContext db = new patientSQLContext();

        //order the patients first by last name and then by first name and pass all the patients to the index view.
        public ActionResult Index()
        {
            var patients = db.patients.OrderBy(p => p.lastName).ThenBy(p => p.firstName);
            return View(patients.ToList());
        }

        // if the passed id is null , then display a message to user, and navigate to index view, otherwise, 
        //pass the object to detail view if the patient object can be found by the id.
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "please select a valid patient from the list";
                return RedirectToAction("index");
            }
            patient patient = db.patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // order the provice by name, and assign all the province objects to a collection and pass it to the viewbag, then render the create view.
        public ActionResult Create()
        {
            var provinces = db.provinces.OrderBy(p => p.name);
            ViewBag.provinceCode = new SelectList(provinces, "provinceCode", "name");
            return View();
        }

        // POST: /Patient/Create
        // get the passed patient object, if the modelstate is validl, then persist the patient into database, or go back the create view. if there is any exception thrown, 
        // place the message into Modelstate, and go back to the create view with error message
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "patientId,firstName,lastName,address,city,provinceCode,postalCode,OHIP,dateOfBirth,deceased,dateOfDeath,homePhone,gender")] patient patient)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.patients.Add(patient);
                    db.SaveChanges();

                    TempData["messageType"] = "success";
                    TempData["message"] = "One new patient record has been added successfully!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.GetBaseException().Message);
            }


            return Create();
        }

        //if the passed id is null, then navigate to the index view and display a message, if the id is valid and the patient exists in the database, 
        //then retrieve the patient object and pass the patient to the edit view, meanwhile get the province info collection and pass it into the viewbag.
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "please select a valid patient from the list";
                return RedirectToAction("index");
            }
            patient patient = db.patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }

            var provinces = db.provinces.OrderBy(p => p.name);
            ViewBag.provinceCode = new SelectList(provinces, "provinceCode", "name", patient.provinceCode);

            return View(patient);
        }

        // POST: /Patient/Edit/5
        // get the data from the edit submission, if the modelstate is valid, then persist the change into the database, or stay at the edit view and display the error message.
        // if there is any exception has been trown, catch the exception and add the info of the exception into the modelstate.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "patientId,firstName,lastName,address,city,provinceCode,postalCode,OHIP,dateOfBirth,deceased,dateOfDeath,homePhone,gender")] patient patient)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    db.Entry(patient).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["messageType"] = "success";
                    TempData["message"] = "One patient record has been updated successfully!";

                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.GetBaseException().Message);
            }

            var provinces = db.provinces.OrderBy(p => p.name);
            ViewBag.provinceCode = new SelectList(provinces, "provinceCode", "name", patient.provinceCode);

            return View(patient);
        }

        //if the passed id is null, then navigate to the index view and display a message, if the id is valid and the patient exists in the database, 
        //then retrieve the patient object and pass the patient to the delete view
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "please select a valid patient from the list";
                return RedirectToAction("index");
            }
            patient patient = db.patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        //get the data which is submitted from the delete view, if the patient object can be found in the database by the passed id, then delete it and navigate to the index view.
        // if there is any exception has been trown, catch the exception and add the info of the exception into the modelstate.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                patient patient = db.patients.Find(id);
                db.patients.Remove(patient);
                db.SaveChanges();
                TempData["messageType"] = "success";
                TempData["message"] = "One new patient record has been deleted successfully!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.GetBaseException().Message);
                TempData["messageType"] = "danger";
                TempData["message"] = "The delete operation failed: " + ex.GetBaseException().ToString();
            }

            return Delete(id);
        }

        //release all the data in this controller 
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
