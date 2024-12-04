using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cumulative_assignment.Models;
using System;
using MySql.Data.MySqlClient;

namespace Cumulative_assignment.Controllers
{
    [Route("api/Teacher")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;
        // dependency injection of database context
        public TeacherAPIController(SchoolDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Returns a list of Teachers when calling endpoint api/Teacher
        /// </summary>
        /// <example>
        /// GET api/Teacher/ListTeachers -> [{"TeacherId":1,"TeacherFname":"Aaron", "TeacherLName":"Paul", "TeacherEmployeeNum":"Y401", "TeacherHireDate": "09-02-2017", "TeacherSalary": "50.20" },{"TeacherId":2,"TeacherFname":"Ben", "TeacherLName":"Tennison"},..]
        /// </example>
        /// <returns>
        /// A list of Teacher objects
        /// </returns>
        [HttpGet]
        [Route(template:"ListTeachers")]
        public List<Teacher> ListTeachers()
        {
            // Create an empty list of Teachers
            List<Teacher> Teachers = new List<Teacher>();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                //SQL QUERY
                Command.CommandText = "select * from teachers";

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        //Access Column information by the DB column name as an index
                        int Id = Convert.ToInt32(ResultSet["teacherid"]);
                        string FirstName = ResultSet["teacherfname"]?.ToString() ?? string.Empty;
                        string LastName = ResultSet["teacherlname"]?.ToString() ?? string.Empty;
                        DateTime HireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                        string EmployeeNum = ResultSet["employeenumber"]?.ToString() ?? string.Empty;
                        string Salary =ResultSet["salary"]?.ToString() ?? string.Empty; 


                        //short form for setting all properties while creating the object
                        Teacher CurrentTeacher = new Teacher()
                        {
                            TeacherId=Id,
                            TeacherFName=FirstName,
                            TeacherLName=LastName,
                            TeacherEmployeeNum=EmployeeNum,
                            TeacherHireDate=HireDate,
                            TeacherSalary=Salary
                        };

                        Teachers.Add(CurrentTeacher);

                    }
                }                    
            }
            

            //Return the final list of Teachers
            return Teachers;
        }


        /// <summary>
        /// Returns a specific Teacher in the database from their ID
        /// </summary>
        /// <example>
        /// GET api/Teacher/FindTeacher/1 -> {"TeacherId":1,"TeacherFname":"Aaron", "TeacherLName":"Paul", "TeacherEmployeeNum":"Y401", "TeacherHireDate": "09-02-2017", "TeacherSalary": "50.20" }
        /// </example>
        /// <returns>
        /// A matching Teacher object by its ID. 
        /// </returns>
        [HttpGet]
        [Route(template: "FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {
            
            //Empty Teacher
            Teacher SelectedTeacher = new Teacher();

            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // @id is replaced with a 'sanitized' id
                Command.CommandText = "select * from teachers where teacherid=@id";
                Command.Parameters.AddWithValue("@id", id);

                // Gather Result Set of Query into a variable
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    //Loop Through Each Row the Result Set
                    while (ResultSet.Read())
                    {
                        //Access Column information by the DB column name as an index
                        int Id = Convert.ToInt32(ResultSet["teacherid"]);
                        string FirstName = ResultSet["teacherfname"]?.ToString() ?? string.Empty;
                        string LastName = ResultSet["teacherlname"]?.ToString() ?? string.Empty;
                        DateTime HireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                        string EmployeeNum = ResultSet["employeenumber"]?.ToString() ?? string.Empty;
                        string Salary =ResultSet["salary"]?.ToString() ?? string.Empty; 

                        SelectedTeacher.TeacherId=Id;
                        SelectedTeacher.TeacherFName=FirstName;
                        SelectedTeacher.TeacherLName=LastName;
                        SelectedTeacher.TeacherEmployeeNum=EmployeeNum;
                        SelectedTeacher.TeacherHireDate=HireDate;
                        SelectedTeacher.TeacherSalary=Salary;
                    }
                }
            }
            return SelectedTeacher;
        }
    }
}
