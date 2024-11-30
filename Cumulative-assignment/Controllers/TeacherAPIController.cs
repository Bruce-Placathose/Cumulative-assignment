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
        /// Returns a list of Teachers in the system
        /// </summary>
        /// <example>
        /// GET api/Teacher/ListTeachers -> [{"TeacherId":1,"TeacherFname":"Brian", "TeacherLName":"Smith"},{"TeacherId":2,"TeacherFname":"Jillian", "TeacherLName":"Montgomery"},..]
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
        /// Returns an Teacher in the database by their ID
        /// </summary>
        /// <example>
        /// GET api/Teacher/FindTeacher/3 -> {"TeacherId":3,"TeacherFname":"Sam","TeacherLName":"Cooper"}
        /// </example>
        /// <returns>
        /// A matching Teacher object by its ID. Empty object if Teacher not found
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

        // [HttpPost(template:"AddTeacher")]
        // public int AddTeacher([FromBody]Teacher TeacherData)
        // {
        //     // 'using' will close the connection after the code executes
        //     using (MySqlConnection Connection = _context.AccessDatabase())
        //     {
        //         Connection.Open();
        //         //Establish a new query for our database
        //         MySqlCommand Command = Connection.CreateCommand();

        //         // CURRENT_DATE() for the Teacher join date in this context
        //         Command.CommandText = "insert into teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) values (@teacherfname, @teacherlname, @employeenumber, CURRENT_DATE(), @salary)";
        //         Command.Parameters.AddWithValue("@teacherfname", TeacherData.TeacherFName);
        //         Command.Parameters.AddWithValue("@teacherlname", TeacherData.TeacherLName);
        //         Command.Parameters.AddWithValue("@employeenumber", TeacherData.TeacherEmployeeNum);
        //         Command.Parameters.AddWithValue("@salary", TeacherData.TeacherSalary);

        //         Command.ExecuteNonQuery();

        //         return Convert.ToInt32(Command.LastInsertedId);

        //     }
        //     // if failure
        //     return 0;
        // }

        [HttpPost(template: "AddTeacher")]
        public int AddTeacher([FromBody] Teacher TeacherData)
        {
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();

                // Retrieve the last employee number
                MySqlCommand GetLastEmployeeNumberCommand = Connection.CreateCommand();
                GetLastEmployeeNumberCommand.CommandText = "SELECT employeenumber FROM teachers ORDER BY teacherid DESC LIMIT 1";

                string LastEmployeeNumber = GetLastEmployeeNumberCommand.ExecuteScalar()?.ToString();
                
                // Default to T400 if no records exist
                int NewEmployeeNumber = 404; // Start after T400
                if (!string.IsNullOrEmpty(LastEmployeeNumber) && LastEmployeeNumber.StartsWith("T"))
                {
                    NewEmployeeNumber = int.Parse(LastEmployeeNumber.Substring(1)) + 1;
                }

                string GeneratedEmployeeNumber = $"T{NewEmployeeNumber}";

                // Establish a new query to insert the teacher
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) VALUES (@teacherfname, @teacherlname, @employeenumber, CURRENT_DATE(), @salary)";
                Command.Parameters.AddWithValue("@teacherfname", TeacherData.TeacherFName);
                Command.Parameters.AddWithValue("@teacherlname", TeacherData.TeacherLName);
                Command.Parameters.AddWithValue("@employeenumber", GeneratedEmployeeNumber);
                Command.Parameters.AddWithValue("@salary", TeacherData.TeacherSalary);

                Command.ExecuteNonQuery();

                // Return the last inserted ID
                return Convert.ToInt32(Command.LastInsertedId);
            }

            // Return 0 if failure
            return 0;
        }

        [HttpDelete(template:"DeleteTeacher/{TeacherId}")]
        public int DeleteTeacher(int TeacherId)
        {
            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                
                Command.CommandText = "delete from teachers where teacherid=@id";
                Command.Parameters.AddWithValue("@id", TeacherId);
                return Command.ExecuteNonQuery();

            }
            // if failure
            return 0;
        }
    }
}
