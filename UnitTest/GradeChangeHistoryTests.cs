using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using CoreApp.Models;

namespace UnitTest;

public class GradeChangeHistoryTests
{
    [Fact]
    public void Grade_WhenCreated_ShouldInitializeChangeHistoryAsEmptyList()
    {
        var grade = new Grade();

        Assert.NotNull(grade.ChangeHistory);
        Assert.Empty(grade.ChangeHistory);
    }

    [Fact]
    public void Grade_WhenCreated_ShouldSetCreatedAtToCurrentUtcTime()
    {
        var beforeCreation = DateTime.UtcNow;

        var grade = new Grade();

        var afterCreation = DateTime.UtcNow;

        Assert.True(grade.CreatedAt >= beforeCreation && grade.CreatedAt <= afterCreation);
    }

    [Fact]
    public void Grade_WithSetValues_ShouldContainCorrectData()
    {
        var gradeValue = GradeValue.Grade40;
        var gradeType = GradeType.Final;
        var createdBy = "lecturer1";
        var now = DateTime.UtcNow;

        var grade = new Grade
        {
            GradeValue = gradeValue,
            GradeType = gradeType,
            CreatedBy = createdBy,
            Date = now
        };

        Assert.Equal(gradeValue, grade.GradeValue);
        Assert.Equal(gradeType, grade.GradeType);
        Assert.Equal(createdBy, grade.CreatedBy);
        Assert.Equal(now, grade.Date);
    }

    [Fact]
    public void Grade_WhenModified_ShouldUpdateModificationTracking()
    {
        var grade = new Grade
        {
            GradeValue = GradeValue.Grade40,
            CreatedBy = "lecturer1"
        };
        var beforeModification = DateTime.UtcNow;

        grade.GradeValue = GradeValue.Grade50;
        grade.ModifiedBy = "deanoffice1";
        grade.ModifiedAt = DateTime.UtcNow;

        var afterModification = DateTime.UtcNow;

        Assert.NotNull(grade.ModifiedBy);
        Assert.Equal("deanoffice1", grade.ModifiedBy);
        Assert.NotNull(grade.ModifiedAt);
        Assert.True(grade.ModifiedAt >= beforeModification && grade.ModifiedAt <= afterModification);
    }

    [Fact]
    public void GradeChangeHistory_WhenCreated_ShouldSetTimestampToCurrentUtcTime()
    {
        var beforeCreation = DateTime.UtcNow;

        var history = new GradeChangeHistory();

        var afterCreation = DateTime.UtcNow;

        Assert.True(history.ChangedAt >= beforeCreation && history.ChangedAt <= afterCreation);
    }

    [Fact]
    public void GradeChangeHistory_WithAllData_ShouldContainCorrectInformation()
    {
        var gradeId = Guid.NewGuid();
        var previousValue = GradeValue.Grade40;
        var newValue = GradeValue.Grade50;
        var changedBy = "deanoffice1";
        var changedAt = DateTime.UtcNow;

        var history = new GradeChangeHistory
        {
            GradeId = gradeId,
            PreviousValue = previousValue,
            NewValue = newValue,
            ChangedBy = changedBy,
            ChangedAt = changedAt
        };

        Assert.Equal(gradeId, history.GradeId);
        Assert.Equal(previousValue, history.PreviousValue);
        Assert.Equal(newValue, history.NewValue);
        Assert.Equal(changedBy, history.ChangedBy);
        Assert.Equal(changedAt, history.ChangedAt);
    }

    [Fact]
    public void GradeChangeHistory_WithNullPreviousValue_ShouldRepresentNewGrade()
    {
        var history = new GradeChangeHistory
        {
            PreviousValue = null,
            NewValue = GradeValue.Grade40,
            ChangedBy = "lecturer1"
        };

        Assert.Null(history.PreviousValue);
        Assert.Equal(GradeValue.Grade40, history.NewValue);
    }

    [Fact]
    public void Grade_WithMultipleChanges_ShouldTrackAllInChangeHistory()
    {
        var grade = new Grade
        {
            GradeValue = GradeValue.Grade40,
            CreatedBy = "lecturer1"
        };

        var history1 = new GradeChangeHistory
        {
            GradeId = grade.Id,
            PreviousValue = null,
            NewValue = GradeValue.Grade40,
            ChangedBy = "lecturer1",
            ChangedAt = DateTime.UtcNow
        };
        grade.ChangeHistory.Add(history1);

        var history2 = new GradeChangeHistory
        {
            GradeId = grade.Id,
            PreviousValue = GradeValue.Grade40,
            NewValue = GradeValue.Grade50,
            ChangedBy = "deanoffice1",
            ChangedAt = DateTime.UtcNow.AddSeconds(5)
        };
        grade.ChangeHistory.Add(history2);

        Assert.Equal(2, grade.ChangeHistory.Count);
        Assert.Null(grade.ChangeHistory[0].PreviousValue);
        Assert.Equal(GradeValue.Grade40, grade.ChangeHistory[0].NewValue);
        Assert.Equal("lecturer1", grade.ChangeHistory[0].ChangedBy);
        
        Assert.Equal(GradeValue.Grade40, grade.ChangeHistory[1].PreviousValue);
        Assert.Equal(GradeValue.Grade50, grade.ChangeHistory[1].NewValue);
        Assert.Equal("deanoffice1", grade.ChangeHistory[1].ChangedBy);
    }

    [Fact]
    public void Grade_ChangeHistoryOrder_ShouldMaintainChronologicalSequence()
    {
        var grade = new Grade { CreatedBy = "lecturer1" };
        var now = DateTime.UtcNow;
        var gradeValues = new[] { GradeValue.Grade30, GradeValue.Grade40, GradeValue.Grade50 };

        for (int i = 0; i < 3; i++)
        {
            var history = new GradeChangeHistory
            {
                GradeId = grade.Id,
                PreviousValue = i > 0 ? gradeValues[i - 1] : null,
                NewValue = gradeValues[i],
                ChangedBy = $"user{i}",
                ChangedAt = now.AddSeconds(i)
            };
            grade.ChangeHistory.Add(history);
        }

        Assert.Equal(3, grade.ChangeHistory.Count);
        for (int i = 0; i < 3; i++)
        {
            Assert.Equal($"user{i}", grade.ChangeHistory[i].ChangedBy);
            Assert.Equal(gradeValues[i], grade.ChangeHistory[i].NewValue);
        }
    }

    [Fact]
    public void GradeChangeHistory_ShouldTrackWhoMadeTheChange()
    {
        var history = new GradeChangeHistory
        {
            ChangedBy = "deanoffice1"
        };

        Assert.Equal("deanoffice1", history.ChangedBy);
        Assert.NotEmpty(history.ChangedBy);
    }

    [Fact]
    public void GradeChangeHistory_ShouldTrackWhenChangeWasMade()
    {
        var beforeChange = DateTime.UtcNow;

        var history = new GradeChangeHistory
        {
            ChangedAt = DateTime.UtcNow
        };

        var afterChange = DateTime.UtcNow;

        Assert.True(history.ChangedAt >= beforeChange && history.ChangedAt <= afterChange);
    }

    [Fact]
    public void Grade_CreatedAndModifiedTimestamps_ShouldDiffer()
    {
        var grade = new Grade { CreatedBy = "lecturer1" };
        var createdTime = grade.CreatedAt;

        System.Threading.Thread.Sleep(100);
        grade.ModifiedBy = "deanoffice1";
        grade.ModifiedAt = DateTime.UtcNow;

        Assert.NotNull(grade.ModifiedAt);
        Assert.True(grade.ModifiedAt > createdTime);
    }

    [Fact]
    public void Grade_BeforeModification_ShouldHaveNullModifyingData()
    {
        var grade = new Grade { CreatedBy = "lecturer1" };

        Assert.Null(grade.ModifiedBy);
        Assert.Null(grade.ModifiedAt);
    }

    [Fact]
    public void GradeChangeHistory_CanBeMappedInChangeList()
    {
        var grade = new Grade { CreatedBy = "lecturer1" };
        var gradeValues = new[] { GradeValue.Grade30, GradeValue.Grade40, GradeValue.Grade45, GradeValue.Grade50, GradeValue.Grade35 };
        var histories = Enumerable.Range(0, 5).Select(i => new GradeChangeHistory
        {
            GradeId = grade.Id,
            NewValue = gradeValues[i],
            ChangedBy = $"user{i + 1}",
            ChangedAt = DateTime.UtcNow.AddSeconds(i)
        }).ToList();
        grade.ChangeHistory = histories;

        var allChangedBy = grade.ChangeHistory.Select(h => h.ChangedBy).ToList();

        Assert.Equal(5, allChangedBy.Count);
        for (int i = 0; i < 5; i++)
        {
            Assert.Equal($"user{i + 1}", allChangedBy[i]);
        }
    }
}











