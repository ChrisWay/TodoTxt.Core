using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TodoTxt.Core.Tests
{
    public class TodoTaskTests
    {
        [Fact]
        public void CreateInstance_Null_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new TodoTask(null));
        }

        [Fact]
        public void IsCompleted_ValidCompletedTaskWithCompletionDate_IsCompletedTrue()
        {
            var task = new TodoTask("x 2011-03-03 Call Mum");

            Assert.True(task.IsCompleted);
        }

        [Fact]
        public void IsCompleted_InvalidCompletedTaskWithCompletionDate_IsCompletedFalse()
        {
            var task = new TodoTask("X 2011-03-03 Call Mum");

            Assert.False(task.IsCompleted);
        }

        [Fact]
        public void IsCompleted_NoCompletionMarker_IsCompletedFalse()
        {
            var task = new TodoTask("xylophone lesson");

            Assert.False(task.IsCompleted);
        }

        [Fact]
        public void IsCompleted_PriorityAheadOfCompletionMarker_IsCompletedFalse()
        {
            var task = new TodoTask("(A) x Find ticket prices");

            Assert.False(task.IsCompleted);
        }

        [Fact]
        public void DateCompleted_ValidCompletedTaskWithCompletionDate_DateCompletedSet()
        {
            var task = new TodoTask("x 2011-03-03 Call Mum");

            Assert.Equal(new DateOnly(2011, 3, 3), task.CompletionDate);
        }

        [Fact]
        public void DateCompleted_CompletedTaskWithNoCompletionDate_ThrowsException()
        {
            Assert.Throws<TodoTxtException>(() => new TodoTask("x Call Mum"));
        }

        [Fact]
        public void DateCreated_CompletedTaskWithCompletionDateAndCreationDate_CreationDateSet()
        {
            var task = new TodoTask("x 2011-03-03 2011-03-02 Call Mum");

            Assert.Equal(new DateOnly(2011, 3, 2), task.CreationDate);
        }

        [Fact]
        public void DateCreated_CompletedTaskWithCompletionDateAndCreationDate_CompletionDateSet()
        {
            var task = new TodoTask("x 2011-03-03 2011-03-02 Call Mum");

            Assert.Equal(new DateOnly(2011, 3, 3), task.CompletionDate);
        }

        [Fact]
        public void DateCreated_TaskWithCreationDate_CreationDateSet()
        {
            var task = new TodoTask("2011-03-02 Call Mum");

            Assert.Equal(new DateOnly(2011, 3, 2), task.CreationDate);
        }

        [Fact]
        public void DateCreated_TaskWithNoCreationDate_CreationDateNotSet()
        {
            var task = new TodoTask("(A) Call Mum 2011-03-02");

            Assert.Null(task.CreationDate);
        }

        [Fact]
        public void DateCreated_TaskWithNoCreationDateShortDescription_CreationDateNotSet2()
        {
            var task = new TodoTask("Stuff");

            Assert.Null(task.CreationDate);
        }

        [Fact]
        public void Priority_TaskWithPriority_PrioritySet()
        {
            var task = new TodoTask("(A) Call Mum 2011-03-02");

            Assert.Equal("A", task.Priority);
        }

        [Fact]
        public void Priority_TaskWithShortText_PriorityNotSet()
        {
            var task = new TodoTask("A");

            Assert.Null(task.Priority);
        }

        [Fact]
        public void Priority_TaskWithNoPriority_PriorityNotSet()
        {
            var task = new TodoTask("A Call Mum 2011-03-02");

            Assert.Null(task.Priority);
        }

        [Fact]
        public void Priority_TaskWithInvalidPriority_PriorityNotSet()
        {
            var task = new TodoTask("(1) Call Mum 2011-03-02");

            Assert.Null(task.Priority);
        }

        [Fact]
        public void Priority_TaskWithInvalidPriority_PriorityNotSet2()
        {
            var task = new TodoTask("(a) Call Mum 2011-03-02");

            Assert.Null(task.Priority);
        }

        [Fact]
        public void Priority_TaskWithInvalidPriority_PriorityNotSet3()
        {
            var task = new TodoTask("(A ) Call Mum 2011-03-02");

            Assert.Null(task.Priority);
        }

        [Fact]
        public void Priority_TaskWithInvalidPriority_PriorityNotSet4()
        {
            var task = new TodoTask("A) Call Mum 2011-03-02");

            Assert.Null(task.Priority);
        }

        [Fact]
        public void Priority_TaskWithInvalidPriority_PriorityNotSet5()
        {
            var task = new TodoTask("(A Call Mum 2011-03-02");

            Assert.Null(task.Priority);
        }

        [Fact]
        public void Description_ShortDescriptiom_DescriptionSet()
        {
            var task = new TodoTask("a");

            Assert.Equal("a", task.Description);
        }

        [Fact]
        public void Description_EmptyString_DescriptionEmpty()
        {
            var task = new TodoTask("");

            Assert.Equal("", task.Description);
        }

        [Fact]
        public void Description_ValidCompletedTaskWithCompletionDate_DescriptionSet()
        {
            var task = new TodoTask("x 2011-03-03 Call Mum");

            Assert.Equal("Call Mum", task.Description);
        }

        [Fact]
        public void Description_ValidCompletedTaskWithCompletionDateAndCreationDate_DescriptionSet()
        {
            var task = new TodoTask("x 2011-03-03 2011-03-03 Call Mum");

            Assert.Equal("Call Mum", task.Description);
        }

        [Fact]
        public void Description_ValidTask_DescriptionSet()
        {
            var task = new TodoTask("Call Mum");

            Assert.Equal("Call Mum", task.Description);
        }

        [Fact]
        public void Description_ValidTaskWithNoDescription_DescriptionSetToEmptyString()
        {
            var task = new TodoTask("x 2022-03-16");

            Assert.Equal("", task.Description);
        }

        [Fact]
        public void Description_ValidTaskWithMultipleValidProject_DescriptionContainsProjects()
        {
            var task = new TodoTask("x 2022-03-16 Call Mum +Family +Life");

            Assert.Equal("Call Mum +Family +Life", task.Description);
        }

        [Fact]
        public void Description_ValidTaskWithMultipleValidContexts_DescriptionContainsContexts()
        {
            var task = new TodoTask("x 2022-03-16 Call Mum @phone @mobile");

            Assert.Equal("Call Mum @phone @mobile", task.Description);
        }

        [Fact]
        public void Projects_ValidTaskWithOneValidProject_ProjectAddedToCollection()
        {
            var task = new TodoTask("x 2022-03-16 Call Mum +Family");

            Assert.Contains("Family", task.ProjectTags);
        }

        [Fact]
        public void Projects_ValidTaskWithOneValidProjectFirst_ProjectAddedToCollection()
        {
            var task = new TodoTask("+Family Call Mum");

            Assert.Contains("Family", task.ProjectTags);
            Assert.Equal("+Family Call Mum", task.Description);
        }

        [Fact]
        public void Projects_ValidTaskWithOneInvalidProjectFirst_ProjectNotAddedToCollection()
        {
            var task = new TodoTask("Some+Family Call Mum");


            Assert.DoesNotContain("Family", task.ProjectTags);
            Assert.DoesNotContain("Some+Family", task.ProjectTags);
            Assert.Equal("Some+Family Call Mum", task.Description);
        }

        [Fact]
        public void Projects_ValidTaskWithInvalidProject_ProjectNotAddedToCollection()
        {
            var task = new TodoTask("x 2022-03-16 Call Mum Some+Family");

            Assert.DoesNotContain("Family", task.ProjectTags);
            Assert.DoesNotContain("Some+Family", task.ProjectTags);
        }

        [Fact]
        public void Projects_ValidTaskWithMultipleValidProject_ProjectsAddedToCollection()
        {
            var task = new TodoTask("x 2022-03-16 Call Mum +Family +Life");

            Assert.Contains("Family", task.ProjectTags);
            Assert.Contains("Life", task.ProjectTags);
        }

        [Fact]
        public void Projects_ValidTaskWithInvalidAndValidProject_ValidProjectAddedToCollection()
        {
            var task = new TodoTask("x 2022-03-16 Call Mum Some+Family +Life");

            Assert.Contains("Life", task.ProjectTags);
            Assert.DoesNotContain("Family", task.ProjectTags);
            Assert.DoesNotContain("Some+Family", task.ProjectTags);
        }

        [Fact]
        public void Contexts_ValidTaskWithOneValidContexts_ContextsAddedToCollection()
        {
            var task = new TodoTask("x 2022-03-16 Call Mum @phone");

            Assert.Contains("phone", task.ContextTags);
        }

        [Fact]
        public void Contexts_ValidTaskWithInvalidContexts_ContextsNotAddedToCollection()
        {
            var task = new TodoTask("x 2022-03-16 Call Mum someone@example.com");

            Assert.DoesNotContain("someone", task.ContextTags);
            Assert.DoesNotContain("example.com", task.ContextTags);
            Assert.DoesNotContain("someone@example.com", task.ContextTags);
        }

        [Fact]
        public void Contexts_ValidTaskWithMultipleValidContexts_ContextsAddedToCollection()
        {
            var task = new TodoTask("x 2022-03-16 Call Mum @phone @mobile");

            Assert.Contains("phone", task.ContextTags);
            Assert.Contains("mobile", task.ContextTags);
        }

        [Fact]
        public void Contexts_ValidTaskWithInvalidAndValidContexts_ValidContextsAddedToCollection()
        {
            var task = new TodoTask("x 2022-03-16 Call Mum someone@example.com @phone");

            Assert.Contains("phone", task.ContextTags);
            Assert.DoesNotContain("example.com", task.ContextTags);
            Assert.DoesNotContain("someone@example.com", task.ContextTags);
        }

        [Fact]
        public void Contexts_ValidTaskWithMultipleValidContextsAndProjects_ContextsAndProjectsAddedToCollection()
        {
            var task = new TodoTask("x 2022-03-16 Call Mum @phone @mobile +Life +Family");

            Assert.Contains("phone", task.ContextTags);
            Assert.Contains("mobile", task.ContextTags);
            Assert.Contains("Family", task.ProjectTags);
            Assert.Contains("Life", task.ProjectTags);
        }

        [Fact]
        public void KeyValue_ValidTaskWithKeyValueTag_KeyValueAddedToCollection()
        {
            var task = new TodoTask("x 2022-03-16 Call Mum key1:value1");

            var value = Assert.Contains("key1", task.KeyValueTags);
            Assert.Equal("value1", value);
        }

        [Fact]
        public void KeyValue_ValidTaskWithMultipleKeyValueTag_KeyValuesAddedToCollection()
        {
            var task = new TodoTask("x 2022-03-16 Call Mum key1:value1 key2:value2");

            var value1 = Assert.Contains("key1", task.KeyValueTags);
            Assert.Equal("value1", value1);

            var value2 = Assert.Contains("key2", task.KeyValueTags);
            Assert.Equal("value2", value2);
        }

        [Fact]
        public void KeyValue_ValidTaskWithInvalidKeyValueTag_KeyValueNotAddedToCollection()
        {
            var task = new TodoTask("x 2022-03-16 Call Mum key1::value1");

            Assert.DoesNotContain("key1", task.KeyValueTags);
        }

        [Fact]
        public void KeyValue_TaskWithInvalidKeyValueTag_KeyValueNotAddedToCollection()
        {
            var task = new TodoTask(":value1");

            Assert.DoesNotContain("value1", task.KeyValueTags);
        }

        [Fact]
        public void KeyValue_TaskWithColon_KeyValueDoesNotThrowException()
        {
            var task = new TodoTask(":");

            Assert.Equal(":", task.Description);
        }

        [Fact]
        public void KeyValue_ValidTaskWithKeyValueTagAtStart_KeyValueAddedToCollection()
        {
            var task = new TodoTask("key1:value1 My Task");

            var value = Assert.Contains("key1", task.KeyValueTags);
            Assert.Equal("value1", value);
            Assert.Equal("key1:value1 My Task", task.Description);
        }
    }
}
