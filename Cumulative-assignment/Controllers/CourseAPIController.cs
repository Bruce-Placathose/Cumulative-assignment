using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cumulative_assignment.Models;
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace Cumulative_assignment.Models
{
  [Route("api/[controller]")]
    [ApiController]
    public class CourseAPIController : ControllerBase
    {
        // get information about the database
        private readonly SchoolDbContext _context;
        // referred to the example
        public CourseAPIController(SchoolDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// This method will return Courses. Will find Courses that match the optional search key input
        /// </summary>
        /// <param name="SearchKey">The key to search Course titles and Course content against</param>
        /// <example>
        /// GET: api/Course/ListCourses?SearchKey=Travel ->
        /// [{"CourseId":"200","CourseTitle":"My encounter in mexico","CourseBody":"I had a great time travelling"},{"CourseId":"201","CourseTitle":"My encounter in the united states","CourseBody":"I had a not so great time on my travels"}]
        /// 
        /// GET: api/Course/ListCourses ->
        /// [{"CourseId":"200","CourseTitle":"My encounter in mexico","CourseBody":"I had a great time travelling"},{"CourseId":"201","CourseTitle":"My encounter in the united states","CourseBody":"I had a not so great time on my travels"},..]
        /// </example>
        /// <returns>A list of Course objects</returns>
        [HttpGet]
        [Route(template: "ListCourses")]
        public List<Course> ListCourses(string SearchKey = null)
        {
            Debug.WriteLine($"Received Search Key input of :{SearchKey}");

            //create an empty list for the Course titles
            List<Course> Courses = new List<Course>();

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                //Create a connection to the database

                //open the connection to the database
                Connection.Open();

                //create a database command
                MySqlCommand Command = Connection.CreateCommand();

                //create a string for the query ""
                string query = "select * from courses where coursename like @key";

                //set the database command text to the query
                Command.CommandText = query;

                Command.Parameters.AddWithValue("@key", $"%{SearchKey}%");

                Command.Prepare();

                // Gather Result Set of Query into a variable



                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //read through the results in a loop
                    while (ResultSet.Read())
                    {
                        Course CurrentCourse = new Course();
                        // for each result, gather the Course information
                        CurrentCourse.CourseId = Convert.ToInt32(ResultSet["courseid"]);
                        CurrentCourse.CourseName = ResultSet["coursename"].ToString();
                        CurrentCourse.CourseCode = ResultSet["coursecode"].ToString();
                        CurrentCourse.CourseTeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                        CurrentCourse.CourseStartDate = Convert.ToDateTime(ResultSet["startdate"]);
                        CurrentCourse.CourseFinishDate = Convert.ToDateTime(ResultSet["finishdate"]);

                        Courses.Add(CurrentCourse);
                    }

                }

            }


            //return the Courses
            return Courses;
        }


        [HttpGet]
        [Route(template: "FindCourse/{CourseId}")]
        public Course FindCourse(int CourseId)
        {
            Course SelectedCourse = new Course();

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                // finding a specific Course by its id
                string query = "select * from courses left join teachers on courses.teacherid=teachers.id where courses.courseid=@id group by courses.courseid ";

                Connection.Open();

                MySqlCommand Command = Connection.CreateCommand();

                Command.Parameters.AddWithValue("@id", CourseId);

                Command.CommandText = query;

                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {


                    while (ResultSet.Read())
                    {
                        SelectedCourse.CourseId = Convert.ToInt32(ResultSet["courseid"]);
                        SelectedCourse.CourseName = ResultSet["coursename"].ToString();
                        SelectedCourse.CourseCode = ResultSet["coursecode"].ToString();
                        SelectedCourse.CourseTeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                        SelectedCourse.CourseStartDate = Convert.ToDateTime(ResultSet["startdate"]);
                        SelectedCourse.CourseFinishDate = Convert.ToDateTime(ResultSet["finishdate"]);
                    }

                }

            }

            return SelectedCourse;
        }

      

        /// <summary>
        /// This endpoint will receive Course Data and add the Course to the database
        /// </summary>
        /// <returns>
        /// The Course ID that was inserted
        /// </returns>
        /// <example>
        /// POST : api/CourseAPI/AddCourse
        /// Header: Content-Type: application/json
        /// Data: {"CourseTitle":"My encounter in mexico","CourseBody":"I had a great more time"}
        /// -> 
        /// "12"
        /// </example>
        [HttpPost(template: "AddCourse")]
        public int AddCourse([FromBody] Course NewCourse)
        {
            // we have Course information

            // we want to add this Course to the database

            // what SQL command adds a Course into our system?

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                string query = "insert into courses (coursecode, teacherid, startdate, finishdate, coursename) values (@code, @teacher, @start, @finish, @name)";

                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = query;
                Command.Parameters.AddWithValue("@code", NewCourse.CourseCode);
                Command.Parameters.AddWithValue("@teacher", NewCourse.CourseTeacherId);
                Command.Parameters.AddWithValue("@start", NewCourse.CourseStartDate);
                Command.Parameters.AddWithValue("@finish", NewCourse.CourseFinishDate);
                Command.Parameters.AddWithValue("@name", NewCourse.CourseName);
               

                Command.ExecuteNonQuery();
                return Convert.ToInt32(Command.LastInsertedId);
            }

            return 0;


        }

        /// <summary>
        /// Receives an ID and deletes the Course from the system
        /// </summary>
        /// <param name="CourseId">The Course Id primary key to delete</param>
        /// <returns>
        /// The number of Courses deleted
        /// </returns>
        /// <example>
        /// DELETE api/CourseAPI/DeleteCourse/9 -> 1
        /// DELETE api/CourseAPI/DeleteCourse/-19 -> 0
        /// </example>
        [HttpDelete(template:"DeleteCourse/{CourseId}")]
        public int DeleteCourse(int CourseId)
        {

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                string query = "delete from courses where courseid=@id";

                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = query;
                Command.Parameters.AddWithValue("@id", CourseId);
                

                
                return Command.ExecuteNonQuery();
            }
        }


    }
}