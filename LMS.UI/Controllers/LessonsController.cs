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
using System.Net.Mail;

namespace LMS.UI.Controllers
{
    [Authorize(Roles = "Admin, manager, employee")]
    public class LessonsController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: Lessons
        public ActionResult Index(int? id)
        {       
            var lessons = db.Lessons.Where(x => x.CourseId == id && x.IsActive == true);
            return View(lessons.ToList());
            //return View(lessons.Where(x => x.Cours.CourseId == x.CourseId).ToList());
        }


        public ActionResult LessonListSpecfic()
        {
            var lessons = db.Lessons.Include(l => l.Cours);
            return View(lessons.Where(x => x.Cours.CourseId == x.CourseId).ToList());
        }

        public ActionResult InactiveLessons()
        {
            var lessons = db.Lessons.Where(x => x.IsActive == false);
            return View(lessons.ToList());
        }


        public ActionResult AdminLessons()
        {
            var lessons = db.Lessons.ToList();
            return View(lessons.ToList());
        }


        // GET: Lessons/Details/5
        public ActionResult Details(int id)
        {
            #region logic for recording lessons eompleted once employee views details page

            string userId = User.Identity.GetUserId();

            LessonView lessonView = new LessonView();
            lessonView.UserId = userId;
            lessonView.LessonId = id;
            lessonView.DateViewed = DateTime.Now;

            //prevent duplicate records 
            var firstView = db.LessonViews.Where(x => x.LessonId == id && x.UserId == userId).FirstOrDefault();
            if (User.IsInRole("employee") && firstView == null)
            {
                db.LessonViews.Add(lessonView);
                db.SaveChanges();
            }

            #endregion

            #region logic for coursecompletion
            Lesson lesson = db.Lessons.Find(id);
            int courseLessonCount = db.Lessons.Where(x => x.CourseId == lesson.CourseId && x.IsActive == true).Count();
            int completedLessonCount = db.LessonViews.Where(x => x.Lesson.CourseId == lesson.CourseId && x.UserId == userId && x.Lesson.IsActive == true).Count();

            if (User.IsInRole("employee") && courseLessonCount == completedLessonCount)
            {
                CourseCompletion completion = new CourseCompletion();
                completion.UserId = userId;
                completion.CourseId = lesson.CourseId;
                completion.DateCompleted = DateTime.Now;

                //prevent duplicates
                var firstCompletion = db.CourseCompletions.Where(x => x.UserId == userId && x.CourseId == lesson.CourseId).FirstOrDefault();
                if (firstCompletion == null)
                {
                    db.CourseCompletions.Add(completion);
                    db.SaveChanges();

                    string courseCompleter = db.UserDetails.Where(x => x.UserId == userId).FirstOrDefault().UserId;
                    string completedCourse = db.Courses.Where(x => x.CourseId == lesson.CourseId).FirstOrDefault().CourseName;
                    var completedDate = completion.DateCompleted;

                    string courseFinishMessage = $"{courseCompleter} completed the {completedCourse} course on {completedDate}";

                    MailMessage m = new MailMessage("postmaster@jamesgriffithdev.com", "james.griffith3@outlook.com", "Course Completion", courseFinishMessage);
                    m.IsBodyHtml = true;
                    m.Priority = MailPriority.High;
                    SmtpClient client = new SmtpClient("mail.jamesgriffithdev.com");
                    client.Credentials = new NetworkCredential("postmaster@jamesgriffithdev.com", "!James9601");
                    client.Port = 8889;
                    client.Send(m);
                }
            }

            #endregion

            if (lesson.VideoUrl != null)
            {
                var v = lesson.VideoUrl.IndexOf("v=");
                var amp = lesson.VideoUrl.IndexOf("&", v);
                string vid;
                if (amp == -1)
                {
                    vid = lesson.VideoUrl.Substring(v + 2);
                }
                else
                {
                    vid = lesson.VideoUrl.Substring(v + 2, amp - (v + 2));
                }
                ViewBag.VideoID = vid;
            }
            return View(lesson);
        }

        // GET: Lessons/Create
        [Authorize(Roles = "Admin")]
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
                    string[] goodExts = new string[] { ".pdf" };

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
                return RedirectToAction("AdminLessons");
                
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // GET: Lessons/Edit/5
        [Authorize(Roles = "Admin")]
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
        public ActionResult Edit([Bind(Include = "LessonId,LessonTitle,CourseId,Introduction,VideoUrl,PdfFileName,IsActive")] Lesson lesson,HttpPostedFileBase fupPdf)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(lesson).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
        //    return View(lesson);
        //}
        {
            if (ModelState.IsValid)
            {            
                if (fupPdf != null)
                {
                    //get the fileName (for extension)
                    string pdfName = fupPdf.FileName;
                    //get the file extension from that 
                    string ext = pdfName.Substring(pdfName.LastIndexOf("."));

                    //create a safelist or (whitelist) of extensions 
                    string[] goodExts = new string[] { ".pdf" };

                    //only use this file if it meets our extension criteria

                    if (goodExts.Contains(ext.ToLower()))
                    {
                        //make sure filename is unique to our system. otherwise we just overwrote a previous records pdf
                        //easiest and most reliable technique is GUID + extension.
                        pdfName = Guid.NewGuid().ToString() + ext;
                        //drop that file into the correct folder in the website.
                        fupPdf.SaveAs(Server.MapPath("~/content/pdfs/" + pdfName));

                        if (lesson.PdfFileName != null && lesson.PdfFileName != "NoContent.pdf")
                        {
                            System.IO.File.Delete(Server.MapPath("~/content/pdfs/" + lesson.PdfFileName));
                        }
                        lesson.PdfFileName = pdfName;
                    }


                }
                db.Entry(lesson).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        // GET: Lessons/Delete/5
        [Authorize(Roles = "Admin")]
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
