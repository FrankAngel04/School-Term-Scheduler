using C971.Classes;
using Plugin.LocalNotification;
using SQLite;

namespace C971.Helpers;

public class DatabaseHelper
{
    private static readonly string DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SchoolTermScheduler.db3");
    private readonly SQLiteAsyncConnection _database;

    public DatabaseHelper()
    {
        _database = new SQLiteAsyncConnection(DbPath);
        _database.CreateTableAsync<Assessment>().Wait();
        _database.CreateTableAsync<Course>().Wait();
        _database.CreateTableAsync<Term>().Wait();
    }
    
    public Task<List<Assessment>> GetAssessmentsByCourseIdAsync(int courseId)
    {
        return _database.Table<Assessment>().Where(a => a.CourseId == courseId).ToListAsync();
    }
    
    public Task<List<Course>> GetCoursesByTermIdAsync(int termId)
    {
        return _database.Table<Course>().Where(c => c.TermId == termId).ToListAsync();
    }

    public Task<List<Assessment>> GetAssessmentsAsync()
    {
        return _database.Table<Assessment>().ToListAsync();
    }
    
    public async Task<List<Course>> GetCoursesAsync()
    {
        var courses = await _database.Table<Course>().ToListAsync();

        foreach (var course in courses)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Course fetched from database: {course.Name}, StartDate: {course.StartDate}, EndDate: {course.EndDate}, Notify: {course.Notify}");
        }

        return courses;
    }
    
    public Task<List<Term>> GetTermsAsync()
    {
        return _database.Table<Term>().ToListAsync();
    }
    
    public Task<int> SaveAssessmentAsync(Assessment assessment)
    {
        if (assessment.Id != 0)
        {
            return _database.UpdateAsync(assessment);
        }
        else
        {
            return _database.InsertAsync(assessment);
        }
    }
    
    public Task<int> SaveCourseAsync(Course course)
    {
        if (course.Id != 0)
        {
            return _database.UpdateAsync(course);
        }
        else
        {
            return _database.InsertAsync(course);
        }
    }
    
    public Task<int> SaveTermAsync(Term term)
    {
        if (term.Id != 0)
        {
            return _database.UpdateAsync(term);
        }
        else
        {
            return _database.InsertAsync(term);
        }
    }
    
    public Task<int> DeleteAssessmentAsync(Assessment assessment)
    {
        var startNotificationId = assessment.Id * 1000 + 101;
        var endNotificationId = assessment.Id * 1000 + 102;

        LocalNotificationCenter.Current.Cancel(startNotificationId);
        LocalNotificationCenter.Current.Cancel(endNotificationId);
    
        return _database.DeleteAsync(assessment);
    }
    
    public async Task<int> DeleteCourseAsync(Course course)
    {
        var startNotificationId = course.Id * 1000 + 1;
        var endNotificationId = course.Id * 1000 + 2;

        LocalNotificationCenter.Current.Cancel(startNotificationId);
        LocalNotificationCenter.Current.Cancel(endNotificationId);

        var assessments = await GetAssessmentsByCourseIdAsync(course.Id);
        foreach (var assessment in assessments)
        {
            await DeleteAssessmentAsync(assessment);
        }

        return await _database.DeleteAsync(course);
    }

    public async Task<int> DeleteTermAsync(Term term)
    {
        var courses = await GetCoursesByTermIdAsync(term.Id);
        foreach (var course in courses)
        {
            await DeleteCourseAsync(course);
        }

        return await _database.DeleteAsync(term);
    }
}