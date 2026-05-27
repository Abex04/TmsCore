// Handles student enrollment processing for the TMS system
// Uses guard clauses to validate inputs before processing
public class EnrollmentService
{
    public EnrollmentRecord ProcessRegistration(Student? student, Course? course)
    {
        // Guard clause 1: student must not be null
        if (student is null) throw new ArgumentNullException(nameof(student));

        // Guard clause 2: course must not be null
        if (course is null) throw new ArgumentNullException(nameof(course));

        // Guard clause 3: course must not be full
        if (course.EnrolledCount >= course.Capacity)
            throw new InvalidOperationException($"Course {course.Code} is full.");

        // Classify academic standing using a switch expression on GPA
        string standing = student.GPA switch
        {
            >= 3.5m => "Honors",
            >= 2.5m => "Good Standing",
            _ => "Academic Warning"
        };

        Console.WriteLine($"{student.Name} is in {standing}.");

        // Return a new immutable enrollment record
        return new EnrollmentRecord(student.Id, course.Code, DateTime.UtcNow);
    }
}