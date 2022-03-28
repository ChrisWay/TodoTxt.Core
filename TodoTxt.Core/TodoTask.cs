using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TodoTxt.Core
{
    public class TodoTask
    {
        const string DateFormat = "yyyy-MM-dd";

        string _rawText;

        public TodoTask(string taskText)
        {
            Description = string.Empty;
            RawText = taskText ?? throw new ArgumentNullException(nameof(taskText));
        }

        public string RawText
        {
            get => _rawText; 
            set
            {
                _rawText = value;
                UpdateTaskData();
            }
        }

        public bool IsCompleted { get; set; }
        public string? Priority { get; set; }
        public DateOnly? CompletionDate { get; set; }
        public DateOnly? CreationDate { get; set; }
        public string Description { get; set; }

        public IDictionary<string, string> KeyValueTags { get; private set; } = new Dictionary<string, string>();
        public IList<string> ProjectTags { get; private set; } = new List<string>();
        public IList<string> ContextTags { get; private set; } = new List<string>();

        public override string ToString() => RawText;

        private void UpdateTaskData()
        {
            if (RawText.Length < 2)
            {
                Description = RawText;
                return;
            }

            int descriptionStartIndex = default;
            var rawSpan = RawText.AsSpan();

            // If the task is completed the first character will be a lower case 'x' followed by a space
            IsCompleted = MemoryExtensions.Equals(rawSpan.Slice(0, 2), "x ", StringComparison.Ordinal);

            // If this task is completed the next series of characters will be the completion date
            if (IsCompleted)
            {
                if (rawSpan.Length >= 12 && DateOnly.TryParseExact(rawSpan.Slice(2, 10), DateFormat, out var completionDate))
                {
                    CompletionDate = completionDate;
                    descriptionStartIndex = 13;
                }
                else
                {
                    // If a task is completed a completion date is required
                    throw new TodoTxtException("Completion date was not present after completion marker");
                }

                // Lets look for the optional creation date after the completion date
                if (rawSpan.Length >= 23 && DateOnly.TryParseExact(rawSpan.Slice(13, 10), DateFormat, out var creationDate))
                {
                    CreationDate = creationDate;
                    descriptionStartIndex = 24;
                }
            }
            else
            {
                // If the task is not completed, lets check if it has a priority
                if (rawSpan.Length >= 3)
                {
                    var maybePriority = rawSpan.Slice(0, 3);

                    if (maybePriority.Length >= 3 && maybePriority[0] == '(' && maybePriority[2] == ')' && char.IsAscii(maybePriority[1]) && char.IsUpper(maybePriority[1]))
                    {
                        Priority = maybePriority[1].ToString();
                        descriptionStartIndex = 3;
                    }
                }

                if (Priority is not null && rawSpan.Length >= 14)
                {
                    var maybeCreationDate = rawSpan.Slice(4, 10);

                    if (DateOnly.TryParseExact(maybeCreationDate, DateFormat, out var creationDate))
                    {
                        CreationDate = creationDate;
                        descriptionStartIndex = 15;
                    }
                }
                else if (rawSpan.Length >= 10)
                {
                    var maybeCreationDate = rawSpan.Slice(0, 10);

                    if (DateOnly.TryParseExact(maybeCreationDate, DateFormat, out var creationDate))
                    {
                        CreationDate = creationDate;
                        descriptionStartIndex = 11;
                    }
                }
            }

            if (descriptionStartIndex >= rawSpan.Length)
            {
                // If ther is no more text left, lets not try to process anymore
                return;
            }

            var descriptionSpan = rawSpan.Slice(descriptionStartIndex, rawSpan.Length - descriptionStartIndex);
            Description = descriptionSpan.ToString();

            ProjectTags = ExtractTags('+', descriptionSpan);
            ContextTags = ExtractTags('@', descriptionSpan);
            KeyValueTags = ExtractKeyValueTags(descriptionSpan);
            
            IList<string> ExtractTags(char prefix, ReadOnlySpan<char> descriptionSpan)
            {
                var index = descriptionSpan.IndexOf(prefix);
                var result = new List<string>();

                while (index != -1)
                {
                    // Are we not at the start of the span and see if the character before is a space
                    if (index > 0 && descriptionSpan[index - 1] != ' ')
                    {
                        // If it isn't then this is not a valid prefix
                        // so lets move the pointer along and check again
                        descriptionSpan = descriptionSpan.Slice(index + 1, descriptionSpan.Length - index - 1);
                        index = descriptionSpan.IndexOf(prefix);
                        continue;                      
                    }

                    // Lets grab the rest of the description from where the tag is
                    descriptionSpan = descriptionSpan.Slice(index + 1, descriptionSpan.Length - index - 1);
                    var endProjectIndex = descriptionSpan.IndexOf(' '); // Do we have more after the tag or are we at the end of the description?

                    if (endProjectIndex == -1)
                    {
                        // If we are at the end of the description, just add all of it
                        result.Add(descriptionSpan.ToString());
                    }
                    else
                    {
                        // If not at the end, add the all the characters until there is a space
                        result.Add(descriptionSpan.Slice(0, endProjectIndex).ToString());
                    }

                    // Reset the index to see if there are more tags
                    index = descriptionSpan.IndexOf(prefix);
                }

                return result;
            }

            IDictionary<string, string> ExtractKeyValueTags(ReadOnlySpan<char> descriptionSpan)
            {
                const char separator = ':';
                var index = descriptionSpan.IndexOf(separator);

                var result = new Dictionary<string, string>();

                while (index != -1)
                {
                    var startIndex = 0;
                    var endIndex = 0;

                    // If we are at the start of the string or
                    // the character before the separator is whitespace or
                    // there are multiple colons
                    // we can not have a key value pair so lets skip those characeters.
                    if (index == 0 || 
                        (index > 0 && char.IsWhiteSpace(descriptionSpan[index - 1])) || 
                        (index < descriptionSpan.Length - 1 && descriptionSpan[index + 1] == ':')) 
                    {
                        descriptionSpan = descriptionSpan.Slice(index + 1, descriptionSpan.Length - index - 1);
                        index = descriptionSpan.IndexOf(separator);
                        continue;
                    }

                    // Work backwards through the string (from the separator) until we find whiespace or the beginning
                    // of the string, this will be the start of the key
                    for (int i = index; i >= 0; i--)
                    {
                        if (char.IsWhiteSpace(descriptionSpan[i]))
                        {
                            break;
                        }

                        startIndex = i;
                    }

                    // Work forwards through the string (from the separator) until we find whitespace or the end
                    // of the string, this will be the end of the value
                    for (int i = index; i < descriptionSpan.Length; i++)
                    {
                        if (char.IsWhiteSpace(descriptionSpan[i]))
                        {
                            break;
                        }

                        endIndex = i;
                    }

                    // If there is whitespace immediately after the separator then we can not have a value
                    if (endIndex == index)
                    {
                        descriptionSpan = descriptionSpan.Slice(index + 1);
                        index = descriptionSpan.IndexOf(separator);
                        continue;
                    }

                    var key = descriptionSpan.Slice(startIndex, index - startIndex);
                    var value = descriptionSpan.Slice(index + 1, endIndex - index);

                    result.Add(key.ToString(), value.ToString());

                    descriptionSpan = descriptionSpan.Slice(index + 1 + value.Length);
                    index = descriptionSpan.IndexOf(separator);
                }

                return result;
            }
        }
    }
}
