﻿using Fluid;
using Fluid.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;

namespace AI_Journalist.Article
{
    class Template
    {
        FluidTemplate Fluid;

        public Template(string filename)
        {
            var contents = File.ReadAllText(filename);
            Fluid = FluidTemplate.Parse(contents);

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

            return Fluid.Render(parserContext, HtmlEncoder.Default);
        }

        static FluidValue ToKst(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            var dateValue = ((DateTimeOffset)input.ToObjectValue()).UtcDateTime;

            var timezone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
            return new ObjectValue(TimeZoneInfo.ConvertTimeFromUtc(dateValue, timezone));
        }

    }
}
