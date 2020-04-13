using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.DATA;

namespace LMS.UI.Controllers
{
    public class LessonsController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: Lessons
        public ActionResult Index(int? id)
        {
            //var lessons = db.Lessons.Include(l => l.Cours); known working
            var lessons = db.Lessons.Where(x => x.CourseId == id);
            return View(lessons.ToList());
            //return View(lessons.Where(x => x.Cours.CourseId == x.CourseId).ToList());
        }


          public ActionResult LessonListSpecfic()
        {
            var lessons = db.Lessons.Include(l => l.Cours);
            return View(lessons.Where(x => x.Cours.CourseId == x.CourseId).ToList());
        }

        // GET: Lessons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        // GET: Lessons/Create
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName");
            return View();
        }

        // POST: Lessons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LessonId,LessonTitle,CourseId,Introduction,VideoUrl,PdfFileName,IsActive")] Lesson lesson, HttpPostedFileBase fupPdf)
        {
            if (ModelState.IsValid)
            {
                string pdfName = "NoContent.pdf";//set a variable for pdf file name. default for no pdf.
                                                 //db.Lessons.Add(lesson);
                                                 //db.SaveChanges();
                                                 //return RedirectToAction("Index");
                if (fupPdf != null)
                {
                    //get the fileName (for extension)
                    pdfName = fupPdf.FileName;
                    //get the file extension from that 
                    string ext = pdfName.Substring(pdfName.LastIndexOf("."));

                    //create a safelist or (whitelist) of extensions 
                    string[] goodExts = new string[] { ".pdf"};

                    //only use this file if it meets our extension criteria

                    if (goodExts.Contains(ext.ToLower()))
                    {
                        //make sure filename is unique to our system. otherwise we just overwrote a previous records image. 
                        //easiest and most reliable technique is GUID + extension.
                        pdfName = Guid.NewGuid().ToString() + ext;
                        //drop that file into the correct folder in the website.
                        fupPdf.SaveAs(Server.MapPath("~/content/pdfs/" + pdfName));
                    }
                    //if the block above failed to run they gave us a file with an extension not approved above. , lots of options to handle this, we could supply error message.
                    //here we will just ignore the file and set this to default noContent.pdf
                    else
                    {
                        pdfName = "NoContent.pdf";

                    }

                }
                lesson.PdfFileName = pdfName;
                db.Lessons.Add(lesson);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // GET: Lessons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LessonId,LessonTitle,CourseId,Introduction,VideoUrl,PdfFileName,IsActive")] Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lesson).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // GET: Lessons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lesson lesson = db.Lessons.Find(id);
            db.Lessons.Remove(lesson);
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
