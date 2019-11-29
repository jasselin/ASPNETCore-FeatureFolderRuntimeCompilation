﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace ASPNETCoreRuntimeCompilation.Features.PlainText
{
    public class PlainTextController : Controller
    {
        public IActionResult Index()
        {
            var content = $"Assembly: {GetType().Assembly.FullName}" + Environment.NewLine
                + $"Location: {GetType().Assembly.Location}"
                + Environment.NewLine + Environment.NewLine;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.FullName == GetType().Assembly.FullName);

            content += $"AppDomain assemblies ({assemblies.Count()}):" + Environment.NewLine;

            foreach (var assembly in assemblies)
                content += assembly.FullName + Environment.NewLine;

            return Content(string.Concat(content, Environment.NewLine, "Text content!"));
        }
    }
}
