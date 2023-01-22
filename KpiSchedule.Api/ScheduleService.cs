﻿using KpiSchedule.Api.Entities.Exams;
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

    public static int GetLessonIdByStartTime(string time)
    {
        return time switch
        {
            "8.30" => 0,
            "10.25" => 1,
            "12.20" => 2,
            "14.15" => 3,
            "16.10" => 4,
            _ => -1
        };
    }
    
    public static int GetCurrentLessonIdByTime(TimeOnly time)
    {
        if (time >= new TimeOnly(8, 30) && time <= new TimeOnly(10, 5))
        {
            return 0;
        }
        if (time >= new TimeOnly(10, 25) && time <= new TimeOnly(12, 00))
        {
            return 1;
        }
        if (time >= new TimeOnly(12, 20) && time <= new TimeOnly(13, 55))
        {
            return 2;
        }
        if (time >= new TimeOnly(14, 15) && time <= new TimeOnly(15, 50))
        {
            return 3;
        }
        if (time >= new TimeOnly(16, 10) && time <= new TimeOnly(17, 45))
        {
            return 4;
        }

        return -1;
    }
    
    public static Dictionary<TimeOnly, TimeOnly> GetTimeTable()
    {
        return new Dictionary<TimeOnly, TimeOnly>
        {
            { new TimeOnly(8, 30), new TimeOnly(10, 5) },
            { new TimeOnly(10, 25), new TimeOnly(12, 0) },
            { new TimeOnly(12, 20), new TimeOnly(13, 55) },
            { new TimeOnly(14, 15), new TimeOnly(15, 50) },
            { new TimeOnly(16, 10), new TimeOnly(17, 45) },
        };
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
    
    public async Task<Lesson?> GetCurrentLesson(Guid groupId)
    {
        var time = await GetCurrentTime();
        var lessons = await GetLessons(groupId);
        var currentLesson = GetCurrentLessonIdByTime(TimeOnly.FromDateTime(DateTime.Now));

        if (currentLesson == -1) return null;
        
        List<DaySchedule> today;
        if (time.CurrentWeek == 0)
        {
            today = lessons.ScheduleFirstWeek;
        }
        else
        {
            today = lessons.ScheduleSecondWeek;
        }

        return today[(time.CurrentDay+6)%7].Lessons[currentLesson];
    }
    
    public async Task<DaySchedule?> GetTodaySchedule(Guid groupId)
    {
        var time = await GetCurrentTime();
        var lessons = await GetLessons(groupId);

        if (time.CurrentDay == 0)
        {
            return null;
        }
        
        List<DaySchedule> today;
        if (time.CurrentWeek == 0)
        {
            today = lessons.ScheduleFirstWeek;
        }
        else
        {
            today = lessons.ScheduleSecondWeek;
        }

        return today[(time.CurrentDay+6)%7];
    }
    
    public async Task<DaySchedule?> GetTomorrowSchedule(Guid groupId)
    {
        var time = await GetCurrentTime();
        var lessons = await GetLessons(groupId);

        if (time.CurrentDay == 6)
        {
            return null;
        }
        
        List<DaySchedule> today;
        if (time.CurrentDay == 0)
        {
            if (time.CurrentWeek == 0)
            {
                today = lessons.ScheduleSecondWeek;
            }
            else
            {
                today = lessons.ScheduleFirstWeek;
            }
        }
        else
        {
            if (time.CurrentWeek == 0)
            {
                today = lessons.ScheduleFirstWeek;
            }
            else
            {
                today = lessons.ScheduleSecondWeek;
            }
        }
        
        return today[time.CurrentDay];
    }
    
    public async Task<List<DaySchedule>> GetCurrentWeek(Guid groupId)
    {
        var time = await GetCurrentTime();
        var schedule = await GetLessons(groupId);
        
        List<DaySchedule> currentWeek;
        if (time.CurrentWeek == 0)
        {
            currentWeek = schedule.ScheduleFirstWeek;
        }
        else
        {
            currentWeek = schedule.ScheduleSecondWeek;
        }

        return currentWeek;
    }
    
    public async Task<List<DaySchedule>> GetNextWeek(Guid groupId)
    {
        var time = await GetCurrentTime();
        var schedule = await GetLessons(groupId);
        
        List<DaySchedule> currentWeek;
        if (time.CurrentWeek == 0)
        {
            currentWeek = schedule.ScheduleSecondWeek;
        }
        else
        {
            currentWeek = schedule.ScheduleFirstWeek;
        }

        return currentWeek;
    }
    
    public async Task<List<Exam>> GetExams(Guid groupId)
    {
        return (await _scheduleApi.GetExams(groupId)).Data;
    }
}