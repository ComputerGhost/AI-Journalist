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
            var googleEvents = request.Execute();

            // Find the ones that apply to the author
            foreach (var googleEvent in googleEvents.Items) {
                if (googleEvent.Summary.Contains(context.Author.Emoticon)) {

                    var ourEvent = new Context.Event() {
                        StartTime = googleEvent.Start.DateTime ?? DateTime.Parse(googleEvent.Start.Date),
                        EndTime = googleEvent.End.DateTime ?? DateTime.Parse(googleEvent.End.Date),
                        IsAllDay = googleEvent.Start.DateTime == null,
                        Title = GetSanitizedTitle(googleEvent.Summary),
                        Description = GetSanitizedDescription(googleEvent.Description),
                    };

                    // Convert from calendar time to UTC
                    ourEvent.StartTime = TimeZoneInfo.ConvertTimeToUtc(
                        DateTime.SpecifyKind(ourEvent.StartTime, DateTimeKind.Unspecified), 
                        timezone);
                    ourEvent.EndTime = TimeZoneInfo.ConvertTimeToUtc(
                        DateTime.SpecifyKind(ourEvent.EndTime, DateTimeKind.Unspecified), 
                        timezone);

                    // Then add it to past or future list
                    if (ourEvent.StartTime <= calendarNow)
                        context.PastEvents.Add(ourEvent);
                    else
                        context.FutureEvents.Add(ourEvent);
                }
            }

            // Sort both lists with the nearest events first.
            context.PastEvents.Sort((x, y) => y.StartTime.CompareTo(x.StartTime));
            context.FutureEvents.Sort((x, y) => x.StartTime.CompareTo(y.StartTime));
        }

        string GetSanitizedTitle(string title)
        {
            return Regex.Replace(title, @"^[^\w\(\)]*(.*?)[^\w\(\)]*$", "$1");
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
