using System.Diagnostics;

// --- Exercise 6: Async/Await — Thread Starvation Demo ---

// THE WRONG WAY: blocking with Thread.Sleep — thread is held hostage
var sw = Stopwatch.StartNew();
for (int i = 0; i < 5; i++)
{
    Thread.Sleep(300); // thread is HELD for 300ms — cannot serve anyone else
}
Console.WriteLine($"Blocking sequential: {sw.ElapsedMilliseconds}ms");

// ASYNC BUT STILL SEQUENTIAL: thread released but calls are one-at-a-time
sw.Restart();
for (int i = 0; i < 5; i++)
{
    await Task.Delay(300); // thread released while waiting — but still sequential
}
Console.WriteLine($"Async sequential: {sw.ElapsedMilliseconds}ms");

// THE RIGHT WAY: all 5 start simultaneously
sw.Restart();
var tasks = Enumerable.Range(0, 5).Select(_ => Task.Delay(300));
await Task.WhenAll(tasks);
Console.WriteLine($"Async parallel: {sw.ElapsedMilliseconds}ms");

// --- Step 2: Fetch Methods ---

// Simulates loading a student from a database — takes 300ms like a real DB call
async Task<Student> FetchStudentAsync(string id)
{
    Console.WriteLine($"  Fetching {id}...");
    await Task.Delay(300); // simulate database latency
    return new Student
    {
        Id = id,
        Name = $"Student-{id}",
        Age = 20,
        GPA = id switch
        {
            "S1" => 3.8m,
            "S2" => 2.4m,
            "S3" => 3.5m,
            "S4" => 1.9m,
            "S5" => 3.2m,
            _    => 2.5m
        }
    };
}

// Simulates loading a course from a database — takes 200ms
async Task<Course> FetchCourseAsync(string code)
{
    Console.WriteLine($"  Fetching course {code}...");
    await Task.Delay(200); // simulate database latency
    return new Course
    {
        Code = code,
        Title = $"Course-{code}",
        Capacity = code switch
        {
            "CRS-101" => 2,
            "CRS-201" => 30,
            "CRS-301" => 15,
            _         => 25
        }
    };
}

// --- Step 3: Load in Parallel ---

// Start all fetches simultaneously — students AND courses at the same time
string[] studentIds  = ["S1", "S2", "S3", "S4", "S5"];
string[] courseCodes = ["CRS-101", "CRS-201", "CRS-301"];

var studentTasks = studentIds.Select(id => FetchStudentAsync(id));
var courseTasks  = courseCodes.Select(code => FetchCourseAsync(code));

// Both arrays load concurrently — total time = slowest single call (~300ms)
sw.Restart();
Student[] students = await Task.WhenAll(studentTasks);
Course[]  courses  = await Task.WhenAll(courseTasks);

Console.WriteLine($"\nLoaded {students.Length} students and {courses.Length} courses in {sw.ElapsedMilliseconds}ms");
foreach (var s in students)
{
    Console.WriteLine($"  {s.Name} GPA: {s.GPA}");
}

// --- Exercise 6 Part B: Enrollment Engine ---

// Course with capacity 2 — only first 2 students will get in
var enrollCourse = new Course { Code = "CRS-101", Title = "C# Mastery", Capacity = 2 };
var enrollService = new EnrollmentService();
var enrollments = new List<EnrollmentRecord>();
var failures = new List<string>();

// Try to enroll every student — track successes and failures
foreach (var student in students)
{
    try
    {
        var record = enrollService.ProcessRegistration(student, enrollCourse);
        enrollCourse.EnrolledCount++; // increment only on success
        enrollments.Add(record);
        Console.WriteLine($"  Enrolled: {student.Name}");
    }
    catch (InvalidOperationException ex)
    {
        failures.Add($"{student.Name}: {ex.Message}");
        Console.WriteLine($"  Rejected: {student.Name} — {ex.Message}");
    }
}