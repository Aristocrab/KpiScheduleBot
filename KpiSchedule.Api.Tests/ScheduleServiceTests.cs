using Xunit;

namespace KpiSchedule.Api.Tests;

public class ScheduleServiceTests
{
    private readonly ScheduleService _sut;
    
    public ScheduleServiceTests()
    {
        _sut = new ScheduleService();
    }

    [Fact]
    public async Task GetGroups_ShouldNotReturnEmptyGroups()
    {
        // Act
        var groups = await _sut.GetGroups();
        
        // Assert
        Assert.All(groups, group =>
        {
            Assert.NotEqual(Guid.Empty, group.Id);
            Assert.False(string.IsNullOrEmpty(group.Name));
        });
    }
    
    [Fact]
    public async Task GetGroupById_ShouldReturnGroup_WhenGroupExists()
    {
        // Arrange
        var groupId = Guid.Parse("edf63937-854d-4afe-941e-8ef0c094f9a4");
        
        // Act
        var group = await _sut.GetGroupById(groupId);

        // Assert
        Assert.NotNull(group);
    }
    
    [Fact]
    public async Task GetGroupById_ShouldReturnNull_WhenGroupDoesntExist()
    {
        // Arrange
        var groupId = Guid.Empty;
        
        // Act
        var group = await _sut.GetGroupById(groupId);

        // Assert
        Assert.Null(group);
    }
    
    [Fact]
    public async Task GetGroupByCode_ShouldReturnGroup_WhenGroupExists()
    {
        // Arrange
        var groupCode = "ІС-12";
        
        // Act
        var group = await _sut.GetGroupByCode(groupCode);

        // Assert
        Assert.NotNull(group);
    }
    
    [Fact]
    public async Task GetGroupByCode_ShouldReturnNull_WhenGroupDoesntExist()
    {
        // Arrange
        var groupId = string.Empty;
        
        // Act
        var group = await _sut.GetGroupByCode(groupId);

        // Assert
        Assert.Null(group);
    }
    
    [Fact]
    public async Task GetLecturers_ShouldNotReturnEmptyLecturers()
    {
        // Act
        var lecturers = await _sut.GetLecturers();

        // Assert
        Assert.All(lecturers, lecturer =>
        {
            Assert.NotEqual(Guid.Empty, lecturer.Id);
            Assert.False(string.IsNullOrEmpty(lecturer.Name));
        });
    }
    
    [Fact]
    public async Task GetLecturerSchedule_ShouldReturnLecturer_WhenLecturerExists()
    {
        // Arrange
        var lecturerId = Guid.Parse("f404d99f-711c-43ad-8e0d-c687e21f0602");
        
        // Act
        var lecturer = await _sut.GetLecturerSchedule(lecturerId);

        // Assert
        Assert.NotNull(lecturer);
    }
    
    [Fact]
    public async Task GetLecturerSchedule_ShouldReturnNull_WhenLecturerDoesntExist()
    {
        // Arrange
        var lecturerId = Guid.Empty;
        
        // Act
        var lecturer = await _sut.GetLecturerSchedule(lecturerId);

        // Assert
        Assert.Null(lecturer);
    }
    
    [Fact]
    public async Task GetLessons_ShouldReturnLecturer_WhenLecturerExists()
    {
        // Arrange
        var lessonsId = Guid.Parse("edf63937-854d-4afe-941e-8ef0c094f9a4");
        
        // Act
        var lessons = await _sut.GetLessons(lessonsId);

        // Assert
        Assert.NotNull(lessons);
    }
    
    [Fact]
    public async Task GetLessons_ShouldReturnNull_WhenLecturerDoesntExist()
    {
        // Arrange
        var lessonsId = Guid.Empty;
        
        // Act
        var lessons = await _sut.GetLessons(lessonsId);

        // Assert
        Assert.Null(lessons);
    }

    [Fact]
    public async Task GetDaySchedule_ShouldReturnDaySchedule_IfDayIsntSunday()
    {
        // Arrange
        var groupId = Guid.Parse("edf63937-854d-4afe-941e-8ef0c094f9a4");
        
        // Act
        var daySchedule = await _sut.GetDaySchedule(groupId, 0, 0);

        // Assert
        Assert.NotNull(daySchedule);
    }
    
    [Fact]
    public async Task GetDaySchedule_ShouldReturnNull_IfGroupDoesntExist()
    {
        // Arrange
        var groupId = Guid.Empty;
        
        // Act
        var daySchedule = await _sut.GetDaySchedule(groupId, 1, 0);

        // Assert
        Assert.Null(daySchedule);
    }
    
    [Fact]
    public async Task GetDaySchedule_ShouldReturnNull_IfDayIsSunday()
    {
        // Arrange
        var groupId = Guid.Parse("edf63937-854d-4afe-941e-8ef0c094f9a4");
        
        // Act
        var daySchedule = await _sut.GetDaySchedule(groupId, 6, 0);

        // Assert
        Assert.Null(daySchedule);
    }
    
    [Fact]
    public async Task GetWeekSchedule_ShouldReturnWeekSchedule_IfGroupExists()
    {
        // Arrange
        var groupId = Guid.Parse("edf63937-854d-4afe-941e-8ef0c094f9a4");
        
        // Act
        var weekSchedule = await _sut.GetWeekSchedule(groupId, 0);

        // Assert
        Assert.NotNull(weekSchedule);
    }
    
    [Fact]
    public async Task GetWeekSchedule_ShouldReturnNull_IfGroupDoesntExist()
    {
        // Arrange
        var groupId = Guid.Empty;
        
        // Act
        var weekSchedule = await _sut.GetWeekSchedule(groupId, 0);

        // Assert
        Assert.Null(weekSchedule);
    }
    
    [Fact]
    public async Task GetExams_ShouldReturnExams_IfGroupExists()
    {
        // Arrange
        var groupId = Guid.Parse("edf63937-854d-4afe-941e-8ef0c094f9a4");
        
        // Act
        var weekSchedule = await _sut.GetExams(groupId);

        // Assert
        Assert.NotNull(weekSchedule);
    }
    
    [Fact]
    public async Task GetExams_ShouldReturnNull_IfGroupDoesntExist()
    {
        // Arrange
        var groupId = Guid.Empty;
        
        // Act
        var weekSchedule = await _sut.GetExams(groupId);

        // Assert
        Assert.Null(weekSchedule);
    }
}