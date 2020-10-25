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
            Fluid = FluidTemplate.Parse(Preprocess(templateContents));
            TemplateContext.GlobalFilters.AddFilter("tokst", ToKst);
            TemplateContext.GlobalFilters.AddFilter("pre", ToPre);
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

            return Postprocess(Fluid.Render(parserContext, HtmlEncoder.Default));
        }

        static FluidValue ToKst(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            var dateValue = ((DateTimeOffset)input.ToObjectValue()).UtcDateTime;

            var timezone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
            return new ObjectValue(TimeZoneInfo.ConvertTimeFromUtc(dateValue, timezone));
        }

        static FluidValue ToPre(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            var stringValue = input.ToStringValue();
            //stringValue = stringValue.Replace("\n", "<br>");
            stringValue = stringValue.Replace("  ", "\xa0 ");
            return new StringValue(stringValue);
        }

        string Preprocess(string template)
        {
            // Remove all comments
            template = new Regex(@"<!--.*?-->").Replace(template, "");

            // Remove all extra whitespace
            template = new Regex(@"[\r\n\t]").Replace(template, " ");
            template = new Regex(@"  +").Replace(template, " ");

            // Replace paragraph tags with double spaces
            template = new Regex(@"<p>(.*?)<\/p>").Replace(template, "\n\n$1\n\n");
            template = template.Replace("\n\n\n\n", "\n\n");

            return template.TrimStart();
        }

        string Postprocess(string rendered)
        {
            return rendered;
        }
    }
}
