using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using FPLedit.Shared.Templating;
using FPLedit.Shared;
using Jint;

namespace FPLedit.Templating
{
    // Based on: https://www.codeproject.com/Articles/15728/Write-your-own-Code-Generator-or-Template-Engine-i
    internal class JavascriptTemplate : ITemplate
    {
        public string TemplateType { get; private set; }
        public string TemplateName { get; private set; }
        public string Identifier { get; }
        public string TemplateSource { get; }
        public string CompiledCode { get; }

        private readonly IInfo info;
        private readonly string nl = Environment.NewLine;

        private const int CURRENT_VERSION = 2;

        public JavascriptTemplate(string code, string identifier, IInfo info)
        {
            TemplateSource = code;
            Identifier = identifier;
            this.info = info;

            CompiledCode = ParseTemplate();
        }

        #region Parser

        private string ParseTemplate()
        {
            var code = TemplateSource;
            const RegexOptions ro = RegexOptions.Singleline | RegexOptions.IgnoreCase;
            const RegexOptions rom = RegexOptions.Multiline | RegexOptions.IgnoreCase;

            // Template definition tag
            code = Regex.Replace(code, @"<#@\s*fpledit-template(.*?)#>", TemplateDefinition, rom);
            code = code.Trim('\r', '\n', ' ', '\t');

            code = ProcessTextBlocks(code);
            code = Regex.Replace(code, @"<#=(.*?)#>", TransformCalls, ro);
            code = Regex.Replace(code, @"<##>", "", ro);
            code = Regex.Replace(code, @"<#[^=|@](.*?)#>", TransformCodeTag, ro);

            return code;
        }

        private string TemplateDefinition(Match m)
        {
            if (TemplateType != null)
                throw new Exception("Nur eine fpledit-template-Direktive ist pro Vorlage erlaubt!");
            var tparams = new ArgsParser(m.Groups[1].ToString().Trim());
            if (tparams.Require("name", "type", "version"))
                throw new Exception("Fehlende Angabe type, version oder name in der fpledit-template-Direktive!");
            if (tparams["version"] != CURRENT_VERSION.ToString())
                throw new Exception($"Template-version mismatch! (Current: {CURRENT_VERSION} vs {tparams["version"]})");
            TemplateType = tparams["type"];
            TemplateName = "JS: " + tparams["name"];
            return ""; // remove this match.
        }

        private string TransformCalls(Match m) => "__builder += " + m.Groups[1].ToString().Trim() + ";" + nl;
        private string TransformCodeTag(Match m) => m.Groups[1].ToString().Trim() + nl;

        private string ProcessTextBlocks(string code) // leaves code blocks untouched
        {
            if (string.IsNullOrEmpty(code))
                return string.Empty;

            const string startTag = "<#";
            const string endTag = "#>";

            var scriptBuilder = new StringBuilder();

            var nextBlockSearchIdx = 0;
            var blockStartIdx = code.IndexOf(startTag, nextBlockSearchIdx, StringComparison.Ordinal);

            while (blockStartIdx >= 0)
            {
                // Output plaintext until the start of this code block.
                scriptBuilder.Append(BuildMultilineAppend(code.Substring(nextBlockSearchIdx, blockStartIdx - nextBlockSearchIdx)));

                // Find corresponding (=next) end tag
                var blockEndIdx = code.IndexOf(endTag, blockStartIdx, StringComparison.Ordinal);
                if (blockEndIdx < 0) // This code block does not end. Throw.
                    throw new Exception("Error parsing template script: No closing code block tag found.");

                scriptBuilder.Append(code.Substring(blockStartIdx, blockEndIdx - blockStartIdx + startTag.Length)); // Add this code block to output.

                nextBlockSearchIdx = blockEndIdx + endTag.Length; // The next block can only start after the current one.
                blockStartIdx = code.IndexOf(startTag, nextBlockSearchIdx, StringComparison.Ordinal); // Find next code block
            }

            // Write out the final block of non-code text (No more code blocks found).
            scriptBuilder.Append(BuildMultilineAppend(code.Substring(nextBlockSearchIdx, code.Length - nextBlockSearchIdx)));

            return scriptBuilder.ToString();
        }

        private string BuildMultilineAppend(string text)
        {
            var lines = text
                .Replace("\\", "\\\\") // escape backslash
                .Replace("\"", "\\\"") // escape quotes
                .Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(nl, lines
                       .Take(lines.Length - 1)
                       .Select(l => "__builder += \"" + l + "\\n\";"))
                   + (lines.Length > 0 ? "__builder += \"" + lines[lines.Length - 1] + "\";" : "")
                   + nl;
        }

        #endregion

        public string GenerateResult(Timetable tt)
        {
            // Allowed types whitlisted by extensions (for this specific template type or generic (e-.g. type == null)).
            var extensionAllowedTypes = info.GetRegistered<ITemplateWhitelist>()
                .Where(w => w.TemplateType == TemplateType || w.TemplateType == null)
                .Select(w => w.GetWhitelistType());
            // Globally whitelisted: From FPLedit.Shared, marked with TemplateSafeAttribute.
            var allowedTypes = typeof(Timetable).Assembly.GetTypes()
                .Where(type => type.GetCustomAttributes(typeof(TemplateSafeAttribute), true).Length > 0)
                .Concat(extensionAllowedTypes)
                .Concat(new[] { typeof(TimeSpan), typeof(Enumerable), }); // Also whitelist type used for time entries

            var engine = new Engine();
            foreach (var type in allowedTypes) // Register all allowed types
                engine.SetValue(type.Name, type);

            TemplateDebugger.GetInstance().SetContext(this); // Move "Debugger" context to current template.

            return engine
                .SetValue("tt", tt)
                .SetValue("debug", new Action<object>((o) => info.Logger.Info($"{o?.GetType()?.FullName ?? "null"}: {o ?? "null"}")))
                .SetValue("debug_print", new Action<object>((o) => info.Logger.Info($"{o}")))
                .SetValue("clr_typename", new Func<object,string>(o => o.GetType().Name))
                .SetValue("clr_typefullname", new Func<object,string>(o => o.GetType().FullName))
                .Execute(ResourceHelper.GetStringResource("Templating.TemplatePolyfills.js")) // Load polyfills
                .Execute("let __builder = '';") // Create output variable
                .Execute(CompiledCode)
                .GetValue("__builder")
                .ToString();
        }
    }
}