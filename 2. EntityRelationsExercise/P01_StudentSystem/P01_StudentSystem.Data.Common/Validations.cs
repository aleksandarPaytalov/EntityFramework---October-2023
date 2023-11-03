namespace P01_StudentSystem.Data.Common;

public class Validations
{
    //Student
    public const int StudentNameMaxLength = 100;
    public const string VarcharType = "varchar";
    public const string NvarcharType = "nvarchar";
    public const int StudentNumberMaxLength = 10;

    //Course
    public const int CourseMaxNameLength = 80;
    public const int CourseMaxDescriptionLength = 500;

    //HomeWork
    public const int HomeworkContentMaxLength = 255;

    //Resources
    public const int ResourcesNameMaxLength = 50;
    public const int ResourcesUrlMaxLength = 255;
}

