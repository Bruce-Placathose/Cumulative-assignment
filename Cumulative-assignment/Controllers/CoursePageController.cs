using Cumulative_assignment.Controllers;
using Cumulative_assignment.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Cumulative_assignment.Controllers
{
    public class CoursePageController : Controller
    {
        private readonly CourseAPIController _api;
        // dependency
        // Ideally would have an Course service
        // where both the MVC and API can call the service
        public CoursePageController(CourseAPIController api)
        {
            _api = api;
        }

        // GET : CoursePage/List?SearchKey={SearchKey} -> A webpage that shows all Courses in the database
        [HttpGet]
        public IActionResult List(string SearchKey = null)
        {
            Debug.WriteLine($"Web Page received key of {SearchKey}");
            List<Course> Courses = _api.ListCourses(SearchKey);

            // get this information from the database

            // send to our Course list view

            // direct us to the /Views/CoursePage/List.cshtml
            return View(Courses);
        }

        // GET : CoursePage/Show/{id} -> A webpage that shows a particular Course in the database matching the given id
        [HttpGet]
        public IActionResult Show(int id)
        {
            Course SelectedCourse = _api.FindCourse(id);

            // direct to /Views/CoursePage/Show.cshtml
            return View(SelectedCourse);
        }

        // GET : CoursePage/New -> A webpage that prompts the user to enter new Course information
        [HttpGet]
        public IActionResult New()
        {
            // direct to /Views/CoursePage/New.cshtml
            return View();
        }

        // POST: CoursePage/Create -> List Courses page with the new Course added
        // Request Header: Content-Type: application/x-www-url-formencoded
        // Request Body:
        // CourseName={title}&CourseCode={body}
        [HttpPost]
        public IActionResult Create(string CourseName, string CourseCode)
        {
            Debug.WriteLine($"Title {CourseName}");
            Debug.WriteLine($"Code {CourseCode}");

            Course NewCourse = new Course();
            NewCourse.CourseName = CourseName;
            NewCourse.CourseCode = CourseCode;

            int CourseId = _api.AddCourse(NewCourse);

            //redirect to /CoursePage/Show/{Courseid}
            return RedirectToAction("Show", new { id=CourseId });
        }

        // GET: /CoursePage/DeleteConfirm/{id} -> A webpage asking the user if they are sure they want to delete this Course
        [HttpGet]
        public IActionResult DeleteConfirm(int id)
        {
            // problem: get the Course information
            // given: the Course id
            Course SelectedCourse = _api.FindCourse(id);

            // directs to /Views/CoursePage/DeleteConfirm.cshtml
            return View(SelectedCourse);
        }

        // POST: CoursePage/Delete/{id} -> A webpage that lists the Courses
        [HttpPost]
        public IActionResult Delete(int id)
        {
            int RowsAffected = _api.DeleteCourse(id);

            //todo: log rows affected

            //direct to Views/CoursePage/List.cshtml
            return RedirectToAction("List");
        }

    }
}
