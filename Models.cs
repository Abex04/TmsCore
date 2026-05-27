// Represents a quiz assessment
// Grade is calculated by correct answers divided by total questions
public class Quiz : IGradable
{
	public required string Title { get; init; }
	public required int CorrectAnswers { get; init; }
	public required int TotalQuestions { get; init; }

	// Calculate percentage score — returns 0 if no questions to avoid division by zero
	public decimal CalculateGrade()
	{
		if (TotalQuestions == 0) return 0m;
		return (decimal)CorrectAnswers / TotalQuestions * 100m;
	}
}

// Represents a lab assignment assessment
// Grade is weighted: 70% functionality + 30% code quality
public class LabAssignment : IGradable
{
	public required string Title { get; init; }
	public required decimal FunctionalityScore { get; init; }
	public required decimal CodeQualityScore { get; init; }

	// Calculate weighted grade using the CTBE lab grading formula
	public decimal CalculateGrade()
	{
		return (FunctionalityScore * 0.7m) + (CodeQualityScore * 0.3m);
	}
}
// Contract that any gradable assessment must fulfill
// Any class that implements this interface MUST provide Title and CalculateGrade()
public interface IGradable
{
	string Title { get; }
	decimal CalculateGrade();
}
// Represents a student in the TMS system
// Uses validated properties to reject bad data at the point of entry
public class Student
{
	// Unique identifier — set once at creation, cannot be changed
	public required string Id { get; init; }

	// Student's full name — cannot be empty or whitespace
	public required string Name
	{
		get;
		set => field = !string.IsNullOrWhiteSpace(value)
			? value
			: throw new ArgumentException("Name cannot be empty or whitespace.", nameof(value));
	}

	// Student's age — must be between 16 and 100 (CTBE enrollment policy)
	public int Age
	{
		get;
		set => field = value is >= 16 and <= 100
			? value
			: throw new ArgumentOutOfRangeException(nameof(value), "Age must be between 16 and 100.");
	}

	// Grade Point Average — must be between 0.0 and 4.0
	public decimal GPA
	{
		get;
		set => field = value is >= 0.0m and <= 4.0m
			? value
			: throw new ArgumentOutOfRangeException(nameof(value), "GPA must be between 0.0 and 4.0.");
	}
}
// Immutable by design — the logging pipeline cannot corrupt this
public record EnrollmentRecord(string StudentId, string CourseCode, DateTime EnrolledAt);

public class Course
{
	public required string Code { get; init; }
	public required string Title
	{
		get;
		set => field = !string.IsNullOrWhiteSpace(value)
			? value
			: throw new ArgumentException("Title cannot be empty or whitespace.", nameof(value));
	}
	public int Capacity
	{
		get;
		set => field = value > 0
			? value
			: throw new ArgumentOutOfRangeException(nameof(value), "System constraint: Capacity must be greater than zero.");
	}
	public int EnrolledCount { get; set; }
}