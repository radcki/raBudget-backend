using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using raBudget.Api.Infrastructure;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Models;
using raBudget.Infrastructure.Database;

namespace raBudget.Api
{
    public static partial class StartupExtensions
    {
        public static void UseNpmScript(this IApplicationBuilder app, string script, bool background = true)
        {
            if (string.IsNullOrEmpty(script))
                return;

            if (background)
            {
                Task.Run(async () => await RunNpmScript(script));
            }
            else
            {
                RunNpmScript(script);
            }
        }

        private static Task RunNpmScript(string script)
        {
            var info = new ProcessStartInfo("cmd")
                       {
                           RedirectStandardOutput = true,
                           RedirectStandardError = true,
                           Arguments = $"/C npm run {script}",
                           UseShellExecute = false,
                           WorkingDirectory = Directory.GetCurrentDirectory()
                       };

            var process = Process.Start(info);
            process.EnableRaisingEvents = true;
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.OutputDataReceived += (sender, eventArgs) =>
                                          {
                                              Debug.WriteLine(eventArgs.Data);
                                          };

            process.ErrorDataReceived += (sender, args) =>
                                         {
                                             Debug.WriteLine(args.Data);
                                         };
            return Task.Run(() => process.WaitForExit());
        }
    }

    
}