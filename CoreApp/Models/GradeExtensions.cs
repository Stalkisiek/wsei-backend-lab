namespace CoreApp.Models;

public enum GradeValue
{
    Grade20 = 20, Grade30 = 30, Grade35 = 35, Grade40 = 40, Grade45 = 45, Grade50 = 50
}

public static class GradeExtensions
{
    public static double Value(this GradeValue gradeType)
    {
        return (int)gradeType / 10.0;
    }
    

    public static GradeValue Parse(string gradeString)
    {
        return gradeString switch
        {
            "2.0" => GradeValue.Grade20,
            "3.0" => GradeValue.Grade30,
            "3.5" => GradeValue.Grade35,
            "4.0" => GradeValue.Grade40,
            "4.5" => GradeValue.Grade45,
            "5.0" => GradeValue.Grade50,
            _ => throw new ArgumentException($"Invalid grade: {gradeString}")
        };
    }

    public static List<String> GradeValues()
    {
        return Enum.GetValues<GradeValue>().Select(g => g.Value().ToString("N1")).ToList();
    }

    public static string PolishName(this GradeValue gradeValue)
    {
        if(gradeValue == GradeValue.Grade20) return "niedostateczny";
        else if(gradeValue == GradeValue.Grade30) return "dostateczny";
        else if(gradeValue == GradeValue.Grade35) return "dostateczny plus";
        else if(gradeValue == GradeValue.Grade40) return "dobry";
        else if(gradeValue == GradeValue.Grade45) return "dobry plus";
        else if(gradeValue == GradeValue.Grade50) return "bardzo dobry";
        else throw new ArgumentException($"Invalid grade value: {gradeValue}");
    }
    
}