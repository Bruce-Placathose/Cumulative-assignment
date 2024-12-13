using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cumulative_assignment.Models;
using System;
using MySql.Data.MySqlClient;

namespace Cumulative_assignment.Controllers
{
    [Route("api/Student")]
    [ApiController]
    public class StudentAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public StudentAPIController(SchoolDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route(template:"ListStudents")]
        public List<Student> ListStudents()
        {
            List<Student> Students = new List<Student>();

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "select * from students";

                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    while (ResultSet.Read())
                    {
                        int Id = Convert.ToInt32(ResultSet["studentid"]);
                        string FirstName = ResultSet["studentfname"]?.ToString() ?? string.Empty;
                        string LastName = ResultSet["studentlname"]?.ToString() ?? string.Empty;
                        DateTime EnrollmentDate = Convert.ToDateTime(ResultSet["enrollmentdate"]);
                        string StudentNum = ResultSet["studentnumber"]?.ToString() ?? string.Empty;

                        Student CurrentStudent = new Student()
                        {
                            StudentId = Id,
                            StudentFName = FirstName,
                            StudentLName = LastName,
                            StudentEnrollmentDate = EnrollmentDate,
                            StudentNumber = StudentNum
                        };

                        Students.Add(CurrentStudent);
                    }
                }
            }

            return Students;
        }

        [HttpGet]
        [Route("FindStudent/{id}")]
        public Student FindStudent(int id)
        {
            Student SelectedStudent = new Student();

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "select * from students where studentid=@id";
                Command.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    while (ResultSet.Read())
                    {
                        int Id = Convert.ToInt32(ResultSet["studentid"]);
                        string FirstName = ResultSet["studentfname"]?.ToString() ?? string.Empty;
                        string LastName = ResultSet["studentlname"]?.ToString() ?? string.Empty;
                        DateTime EnrollmentDate = Convert.ToDateTime(ResultSet["enrollmentdate"]);
                        string StudentNum = ResultSet["studentnumber"]?.ToString() ?? string.Empty;

                        SelectedStudent.StudentId = Id;
                        SelectedStudent.StudentFName = FirstName;
                        SelectedStudent.StudentLName = LastName;
                        SelectedStudent.StudentEnrollmentDate = EnrollmentDate;
                        SelectedStudent.StudentNumber = StudentNum;
                    }
                }
            }
            return SelectedStudent;
        }

        [HttpPost("AddStudent")]
        public int AddStudent([FromBody] Student StudentData)
        {
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();

                MySqlCommand GetLastStudentNumberCommand = Connection.CreateCommand();
                GetLastStudentNumberCommand.CommandText = "SELECT studentnumber FROM students ORDER BY studentid DESC LIMIT 1";

                string LastStudentNumber = GetLastStudentNumberCommand.ExecuteScalar()?.ToString();

                int NewStudentNumber = 1757; // Start after N1756
                if (!string.IsNullOrEmpty(LastStudentNumber) && LastStudentNumber.StartsWith("N"))
                {
                    NewStudentNumber = int.Parse(LastStudentNumber.Substring(1)) + 1;
                }

                string GeneratedStudentNumber = $"N{NewStudentNumber}";

                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "INSERT INTO students (studentfname, studentlname, studentnumber, enrollmentdate) VALUES (@studentfname, @studentlname, @studentnumber, CURRENT_DATE())";
                Command.Parameters.AddWithValue("@studentfname", StudentData.StudentFName);
                Command.Parameters.AddWithValue("@studentlname", StudentData.StudentLName);
                Command.Parameters.AddWithValue("@studentnumber", GeneratedStudentNumber);

                Command.ExecuteNonQuery();

                return Convert.ToInt32(Command.LastInsertedId);
            }

            return 0;
        }

        [HttpDelete("DeleteStudent/{StudentId}")]
        public int DeleteStudent(int StudentId)
        {
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "delete from students where studentid=@id";
                Command.Parameters.AddWithValue("@id", StudentId);
                return Command.ExecuteNonQuery();
            }

            return 0;
        }
    }
}