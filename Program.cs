
// Method that processes ANY gradable assessment through the IGradable contract
// It doesn't know or care if it's a Quiz or LabAssignment — just calls the contract
void PrintGradeReport(IEnumerable<IGradable> assessments)
{
	Console.WriteLine("--- Grade Report ---");
	foreach (var item in assessments)
	{
		Console.WriteLine($"{item.Title}: {item.CalculateGrade():F2}%");
	}
}

// One array holds two completely different assessment types
// Both work because they both implement IGradable
IGradable[] cohortAssessments = [
	new Quiz { Title = "C# Basics", CorrectAnswers = 18, TotalQuestions = 20 },
	new LabAssignment { Title = "Registration API", FunctionalityScore = 90m, CodeQualityScore = 85m }
];

PrintGradeReport(cohortAssessments);