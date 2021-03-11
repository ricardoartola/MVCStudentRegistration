using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using MVCStudentRegistration.Helpers;
using MVCStudentRegistration.Instraestructure;
using MVCStudentRegistration.Models;
using MVCStudentRegistration.ViewModels;
using PagedList;

namespace MVCStudentRegistration.Controllers
{
    public class StudentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Students
        public ActionResult Index(string sortDir, string currentFilter, string searchString, int? page, string sortOrder = "")
        {
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;
            ViewBag.sortOrder = sortOrder;
            ViewBag.sortDir = sortDir;

            var students = db.Students.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
                students = students.Where(s => s.Name.Contains(searchString));
            switch (sortOrder.ToLower())
            {
                case "name":
                    if (sortDir.ToLower() == "desc")
                        students = students.OrderByDescending(s => s.Name);
                    else
                        students = students.OrderBy(s => s.Name);
                    break;
                case "enrollmentdate":
                    if (sortDir.ToLower() == "desc")
                        students = students.OrderByDescending(s => s.EnrollmentDate);
                    else
                        students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.Name);
                    break;
            }
            int pageSize = 2;
            int pageNumber = (page ?? 1);
            var data = students.ToPagedList(pageNumber, pageSize);
            if (Request.IsAjaxRequest())
                return PartialView("_SearchList", data);
            else
                return View(data);
        }

        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,EnrollmentDate,Photo")] StudentViewModel studentVM)
        {
            if (ModelState.IsValid)
            {
                //Mapper.CreateMap<StudentViewModel, Student>();
                //var student = Mapper.Map<Student>(studentVM);
                var student = new Student();
                student.Name = studentVM.Name;
                student.EnrollmentDate = studentVM.EnrollmentDate;
                if (studentVM.Photo != null)
                    student.Photo = ImageConverter.ByteArrayFromPostedFile(studentVM.Photo);
                db.Students.Add(student);
                db.SaveChanges();//commit into the database
                return RedirectToAction("Index");
            }

            return View(studentVM);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }

            Mapper.CreateMap<Student, StudentViewModel>().ForMember(x => x.Photo, opt => opt.Ignore());
            var studentVM = Mapper.Map<StudentViewModel>(student);
            studentVM.PhotoDB = student.Photo;
            return View(studentVM);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StudentViewModel studentVM)
        {
            if (ModelState.IsValid)
            {
                Student student = db.Students.Find(studentVM.Id);
                if (studentVM != null && studentVM.Photo != null)
                    student.Photo = ImageConverter.ByteArrayFromPostedFile(studentVM.Photo);
                student.Name = studentVM.Name;
                student.EnrollmentDate = studentVM.EnrollmentDate;
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(studentVM);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
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
