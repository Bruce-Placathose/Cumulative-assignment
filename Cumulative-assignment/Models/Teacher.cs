

namespace Cumulative_assignment.Models
{
  /// <summary>
  /// This model Represents a teacher in the system
  /// </summary>
  /// <remark>
  /// This teacher model will store essential details about teachers, e.g: name and Salary
  /// </remark>
    public class Teacher
    {
      
      /// <summary>
      /// 1. id -> unique identifier for the teacher. Need autogeneration
      /// 2. Teacher first and last name seperated. For easy display
      /// 3. Hiredata for administration purpose
      /// 4. Salary for HR
      /// </summary>

      public int TeacherId { get; set; }
      public string? TeacherFName { get; set; }
      public string? TeacherLName { get; set; }
      public string? TeacherEmployeeNum { get; set; }
      public DateTime TeacherHireDate { get; set; }
      public string? TeacherSalary { get; set; }

    }
}