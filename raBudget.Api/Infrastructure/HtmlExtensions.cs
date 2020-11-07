using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace raBudget.Api.Infrastructure
{
    public static class HtmlExtensions
    {
        public static bool IsFieldInvalid<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var name = htmlHelper.NameFor(expression);
            return htmlHelper.ViewData.ModelState.GetFieldValidationState(name) == ModelValidationState.Invalid;
        }
        public static string ValidationMessageTextFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var sw = new StringWriter();
            var validationSpan = (TagBuilder)htmlHelper.ValidationMessageFor(expression);
            validationSpan.InnerHtml.WriteTo(sw, HtmlEncoder.Default);
            
            return HttpUtility.HtmlDecode(sw.ToString());
        }

        public static IHtmlContent MdcTextBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            var fieldName = htmlHelper.NameFor(expression);
            var fieldId = htmlHelper.IdFor(expression);
            var labelId = fieldId + "_label";
            var outerLabel = new TagBuilder("label");
            outerLabel.MergeAttribute("id", labelId, true);
            outerLabel.AddCssClass("mdc-text-field");
            outerLabel.AddCssClass("mdc-text-field--filled");

            var fieldRipple = new TagBuilder("span");
            fieldRipple.AddCssClass("mdc-text-field__ripple");
            outerLabel.InnerHtml.AppendHtml(fieldRipple);

            var input = new TagBuilder("input");
            input.AddCssClass("mdc-text-field__input");
            input.MergeAttribute("name", fieldName, true);
            input.MergeAttribute("id", fieldId, true);
            if (htmlAttributes != null)
            {
                input.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), true);
            }
            outerLabel.InnerHtml.AppendHtml(input);

            var label = new TagBuilder("span");
            label.AddCssClass("mdc-floating-label");

            label.InnerHtml.AppendHtml(htmlHelper.DisplayNameFor(expression));
            outerLabel.InnerHtml.AppendHtml(label);

            var lineRipple = new TagBuilder("span");
            lineRipple.AddCssClass("mdc-line-ripple");
            outerLabel.InnerHtml.AppendHtml(lineRipple);

            var helperContainer = new TagBuilder("div");
            helperContainer.AddCssClass("mdc-text-field-helper-line");

            var script = new StringBuilder("<script>");
            script.AppendLine("window.addEventListener('load', function(){");
            script.AppendLine($"const {fieldId} = new MDCTextField(document.querySelector('#{labelId}'));");

            if (IsFieldInvalid(htmlHelper, expression))
            {
                script.AppendLine($"{fieldId}.valid = false;");
                var helperMessage = new TagBuilder("div");
                helperMessage.AddCssClass("mdc-text-field-helper-text");
                helperMessage.AddCssClass("mdc-text-field-helper-text--validation-msg");

                helperMessage.InnerHtml.Append(htmlHelper.ValidationMessageTextFor(expression));
                helperContainer.InnerHtml.AppendHtml(helperMessage);
            }
            script.AppendLine("});");
            script.AppendLine("</script>");

            ///htmlHelper.WriteToScript(script);

            var wr = new HtmlContentBuilder();
            return wr.AppendHtml(outerLabel).AppendHtml(helperContainer).AppendHtml(script.ToString());
        }
        public static IHtmlContent MdcCheckBoxFor<TModel>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, object htmlAttributes = null)
        {
            var fieldName = htmlHelper.IdFor(expression);
            var fieldId = fieldName + "Field";
            var checkboxContainerId = fieldId + "Container";
            var formFieldId = fieldId + "FormField";

            var outerContainer = new TagBuilder("div");
            outerContainer.MergeAttribute("id", formFieldId, true);
            outerContainer.AddCssClass("mdc-form-field");

            var checkboxContainer = new TagBuilder("div");
            checkboxContainer.MergeAttribute("id", checkboxContainerId, true);
            checkboxContainer.AddCssClass("mdc-checkbox");

            var input = new TagBuilder("input");
            input.Attributes["type"] = "checkbox";
            input.AddCssClass("mdc-checkbox__native-control");
            input.Attributes["id"] = fieldId;
            input.Attributes["name"] = htmlHelper.NameFor(expression);
            input.Attributes["value"] = htmlHelper.ValueFor(expression);
            outerContainer.InnerHtml.AppendHtml(checkboxContainer);
            checkboxContainer.InnerHtml.AppendHtml(input);

            checkboxContainer.InnerHtml.AppendHtml(@"
<div class=""mdc-checkbox__background"">
    <svg class=""mdc-checkbox__checkmark""
         viewBox=""0 0 24 24"">
        <path class=""mdc-checkbox__checkmark-path""
              fill=""none""
              d=""M1.73,12.91 8.1,19.28 22.79,4.59""/>
    </svg>
    <div class=""mdc-checkbox__mixedmark""></div>
</div>
<div class=""mdc-checkbox__ripple""></div>
");

            var labelContainer = new TagBuilder("label");
            labelContainer.Attributes["for"] = fieldName;
            labelContainer.AddCssClass("my-0 pt-1");
            labelContainer.InnerHtml.AppendHtml(htmlHelper.DisplayNameFor(expression));
            outerContainer.InnerHtml.AppendHtml(labelContainer);

            var helperContainer = new TagBuilder("div");
            helperContainer.AddCssClass("mdc-text-field-helper-line");

            var script = new StringBuilder("<script>");
            script.AppendLine("window.addEventListener('load', function(){");
            script.AppendLine($"const {fieldId} = new MDCCheckbox(document.querySelector('#{checkboxContainerId}'));");
            script.AppendLine($"const {formFieldId} = new MDCFormField(document.querySelector('#{formFieldId}'));");
            script.AppendLine($"{formFieldId}.input = {fieldId};");

            if (IsFieldInvalid(htmlHelper, expression))
            {
                script.AppendLine($"{fieldId}.valid = false;");
                var helperMessage = new TagBuilder("div");
                helperMessage.AddCssClass("mdc-text-field-helper-text");
                helperMessage.AddCssClass("mdc-text-field-helper-text--validation-msg");

                helperMessage.InnerHtml.Append(htmlHelper.ValidationMessageTextFor(expression));
                helperContainer.InnerHtml.AppendHtml(helperMessage);
            }
            script.AppendLine("});");
            script.AppendLine("</script>");
            //htmlHelper.WriteToScript(script);

            var wr = new HtmlContentBuilder();
            return wr.AppendHtml(outerContainer).AppendHtml(helperContainer).AppendHtml(script.ToString());
        }


        public static IHtmlContent MdcButton<TModel>(this IHtmlHelper<TModel> htmlHelper, eButtonType type, string text, string icon = null, object htmlAttributes = null)
        {
            var buttonTag = new TagBuilder("button");
            buttonTag.AddCssClass("mdc-button mdc-button--raised");
            switch (type)
            {
                case eButtonType.Submit:
                    buttonTag.Attributes["type"] = "submit";
                    break;
                case eButtonType.Reset:
                    buttonTag.Attributes["type"] = "reset";
                    break;
            }

            var rippleContainer = new TagBuilder("div");
            rippleContainer.AddCssClass("mdc-button__ripple");
            buttonTag.InnerHtml.AppendHtml(rippleContainer);

            if (!string.IsNullOrEmpty(icon))
            {
                var iconContainer = new TagBuilder("span");
                iconContainer.AddCssClass("iconify");
                iconContainer.AddCssClass("mdc-button__icon");
                iconContainer.Attributes["aria-hidden"] = "true";
                iconContainer.Attributes["data-icon"] = icon;
                buttonTag.InnerHtml.AppendHtml(iconContainer);
            }

            var labelTag = new TagBuilder("span");
            labelTag.AddCssClass("mdc-button__label");
            labelTag.InnerHtml.Append(text);
            buttonTag.InnerHtml.AppendHtml(labelTag);

            var wr = new HtmlContentBuilder();
            return wr.AppendHtml(buttonTag);
        }

        private static void WriteToScript( this IHtmlHelper htmlHelper, object data)
        {
            htmlHelper.ViewContext.HttpContext.Items["_script_" + Guid.NewGuid()] = data;
        }

        public static IHtmlContent RenderScripts(this IHtmlHelper htmlHelper)
        {
            var wr = new HtmlContentBuilder();
            var scripts = htmlHelper.ViewContext.HttpContext.Items.Keys.Where(x => x.ToString().StartsWith("_script_")).Select(x => x.ToString());
            return wr.AppendHtml(string.Join(Environment.NewLine, scripts));
        }
    }

    public enum eButtonType
    {
        Submit,
        Reset
    }

    public enum eButtonStyleType
    {
        Raised
    }
}