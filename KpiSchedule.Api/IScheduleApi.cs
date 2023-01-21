using KpiSchedule.Api.Entities.Exams;
using KpiSchedule.Api.Entities.Groups;
using KpiSchedule.Api.Entities.Lecturers;
using KpiSchedule.Api.Entities.Lessons;
using KpiSchedule.Api.Entities.Time;
using Refit;

namespace KpiSchedule.Api;

public interface IScheduleApi
{
    [Get("/api/schedule/groups")]
    Task<GroupsDto> GetGroups();
    
    [Get("/api/schedule/lecturer/list")]
    Task<LecturersDto> GetLecturers();
    
    [Get("/api/schedule/lecturer")]
    Task<LecturerInfo> GetLecturerById(string lecturerId);
    
    [Get("/api/time/current")]
    Task<CurrentTimeDto> GetCurrentTime();
    
    [Get("/api/schedule/lessons")]
    Task<LessonsDto> GetLessons(Guid groupId);
    
    [Get("/api/exams/group")]
    Task<ExamsDto> GetExams(Guid groupId);
}