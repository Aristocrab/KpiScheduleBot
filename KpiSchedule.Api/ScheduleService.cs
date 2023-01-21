using KpiSchedule.Api.Entities.Exams;
using KpiSchedule.Api.Entities.Groups;
using KpiSchedule.Api.Entities.Lecturers;
using KpiSchedule.Api.Entities.Lessons;
using KpiSchedule.Api.Entities.Time;
using Refit;

namespace KpiSchedule.Api;

public class ScheduleService
{
    private readonly IScheduleApi _scheduleApi;

    public ScheduleService(string apiUrl = "https://schedule.kpi.ua")
    {
        _scheduleApi = RestService.For<IScheduleApi>(apiUrl);
    }

    public async Task<List<Group>> GetGroups()
    {
        return (await _scheduleApi.GetGroups()).Data;
    }

    public async Task<Group?> GetGroupById(Guid groupId)
    {
        var groups = await _scheduleApi.GetGroups();
        return groups.Data.FirstOrDefault(x => x.Id == groupId);
    }

    public async Task<Group?> GetGroupByCode(string groupCode)
    {
        var groups = await _scheduleApi.GetGroups();
        return groups.Data.FirstOrDefault(x => x.Name == groupCode);
    }

    public async Task<List<Lecturer>> GetLecturers()
    {
        var lecturers = await _scheduleApi.GetLecturers();
        lecturers.Data.RemoveAll(x => x.Name == string.Empty);

        return lecturers.Data;
    }

    public async Task<LecturerInfo> GetLecturerById(string lecturerId)
    {
        var lecturer = await _scheduleApi.GetLecturerById(lecturerId);
        return lecturer;
    }

    public async Task<CurrentTime> GetCurrentTime()
    {
        return (await _scheduleApi.GetCurrentTime()).Data;
    }

    public async Task<Lessons> GetLessons(Guid groupId)
    {
        return (await _scheduleApi.GetLessons(groupId)).Data;
    }
    
    public async Task<List<Exam>> GetExams(Guid groupId)
    {
        return (await _scheduleApi.GetExams(groupId)).Data;
    }
}