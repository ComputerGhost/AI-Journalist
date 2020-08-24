using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AI_Journalist.Contexts
{
    class Calendar
    {
        Settings.ContextsNode.CalendarNode Settings;
        CalendarService Service;

        public Calendar(Settings.ContextsNode.CalendarNode settings)
        {
            Settings = settings;
            Service = new CalendarService(new BaseClientService.Initializer() {
                ApplicationName = settings.ApplicationName,
                ApiKey = settings.ApiKey
            });
        }

        public void AddContext(Context context, Settings.ContextsNode.CalendarNode.LinkedNode linked)
        {
            // Because Google uses calendar time, we need a way to convert to UTC.
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(linked.Timezone);
            var calendarNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timezone);

            // Now we can get the nearby events
            var request = Service.Events.List(linked.Id);
            request.SingleEvents = true;
            request.TimeMin = calendarNow.AddDays(-1 * Settings.PastDays);
            request.TimeMax = calendarNow.AddDays(Settings.FutureDays);
            var events = request.Execute();

            // Find which ones apply to the author
            foreach (var item in events.Items) {
                if (item.Summary.Contains(context.Author.Emoticon)) {

                    var @event = new Context.Event() {
                        StartTime = item.Start.DateTime ?? DateTime.Parse(item.Start.Date),
                        EndTime = item.End.DateTime ?? DateTime.Parse(item.End.Date),
                        IsAllDay = item.Start.DateTime == null,
                        Title = GetSanitizedTitle(item.Summary),
                        Description = GetSanitizedDescription(item.Description),
                    };

                    // Convert from calendar time
                    @event.StartTime = TimeZoneInfo.ConvertTimeToUtc(@event.StartTime, timezone);
                    @event.EndTime = TimeZoneInfo.ConvertTimeFromUtc(@event.EndTime, timezone);

                    // Then add it to past or future list
                    if (@event.StartTime <= DateTime.UtcNow)
                        context.PastEvents.Add(@event);
                    else
                        context.FutureEvents.Add(@event);
                    break;
                }
            }

            // Sort both lists with the nearest events first.
            context.PastEvents.Sort((x, y) => y.StartTime.CompareTo(x.StartTime));
            context.FutureEvents.Sort((x, y) => x.StartTime.CompareTo(y.StartTime));
        }

        string GetSanitizedTitle(string title)
        {
            return Regex.Replace(title, @"^[^\w]*(.*?)[^\w]*$", "$1");
        }

        string GetSanitizedDescription(string description)
        {
            if (description == null)
                return null;

            // I think the first paragraph is basically the only one we want.
            description = description.Split("<br>")[0];

            // We also want to remove the extra parameters from the links
            description = Regex.Replace(description,
                @"<a (?:.*?)href=""(.*?)""(?:.*?)>",
                @"<a href=""\1"" target=""_blank"">");

            return description;
        }
    }
}
