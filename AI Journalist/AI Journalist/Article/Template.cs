using Fluid;
using Fluid.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace AI_Journalist.Article
{
    class Template
    {
        FluidTemplate Fluid;

        public Template(string templateContents)
        {
            Fluid = FluidTemplate.Parse(templateContents);
            TemplateContext.GlobalFilters.AddFilter("tokst", ToKst);
        }

        public string Render(Contexts.Context context)
        {
            var parserContext = new TemplateContext();
            parserContext.MemberAccessStrategy.Register<Contexts.Context>();
            parserContext.MemberAccessStrategy.Register<Contexts.Context.Account>();
            parserContext.MemberAccessStrategy.Register<Contexts.Context.Pronoun>();
            parserContext.MemberAccessStrategy.Register<Contexts.Context.Event>();
            parserContext.MemberAccessStrategy.Register<Contexts.Context.MediaDescription>();
            parserContext.MemberAccessStrategy.Register<Sources.Update>();
            parserContext.MemberAccessStrategy.Register<Sources.Update.Media>();
            parserContext.SetValue("Context", context);

            return PostProcess(Fluid.Render(parserContext, HtmlEncoder.Default));
        }

        static FluidValue ToKst(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            var dateValue = ((DateTimeOffset)input.ToObjectValue()).UtcDateTime;

            var timezone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
            return new ObjectValue(TimeZoneInfo.ConvertTimeFromUtc(dateValue, timezone));
        }

        string PostProcess(string rendered)
        {
            // Remove comments that are not WordPress comments
            rendered = new Regex(@"<!--(?![ \/]*wp).*?-->").Replace(rendered, "");

            // Replace all newlines and tabs with spaces
            rendered = new Regex(@"[\r\n\t]").Replace(rendered, " ");

            // Replace double spaces with a single space
            rendered = new Regex(@"  +").Replace(rendered, " ");

            // Add newlines around each (wordpress) comment
            rendered = new Regex(@"(<!--.*?-->)").Replace(rendered, "\n$1\n");

            // Remove trailing spaces on lines and after tag openings
            rendered = new Regex(@"(^|<[^\/][^>]*?>) (?!$)").Replace(rendered, "$1");

            return rendered;
        }
    }
}
