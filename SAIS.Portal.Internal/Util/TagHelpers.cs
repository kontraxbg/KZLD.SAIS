using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SAIS.Model;

namespace SAIS.Portal.TagHelpers
{
    [HtmlTargetElement("input2", Attributes = "asp-for")]
    public class Input2wAspForTagHelper : InputTagHelper
    {
        public Input2wAspForTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            TagHelperUtil.CreateOrMergeAttribute("class", "form-control form-control-sm", output);

            TagBuilder validationSpan = Generator.GenerateValidationMessage(ViewContext, For.ModelExplorer, For.Name, null,
                /*"small" - алтернатива на слагането на класове*/null, new { @class = "col-form-label col-form-label-sm" });
            output.PostElement.AppendHtml(validationSpan.TagBuilderToString());

            if (((Microsoft.AspNetCore.Mvc.ModelBinding.Metadata.DefaultModelMetadata)For.Metadata).Attributes.Attributes.
                    Where(a => a is DataTypeAttribute && ((DataTypeAttribute)a).DataType == DataType.MultilineText).Any())
            {
                output.TagName = "textarea";
                output.TagMode = TagMode.StartTagAndEndTag;

                output.Content.SetHtmlContent(For?.Model?.ToString());
                base.Process(context, output);
                output.Attributes.Remove(output.Attributes["value"]);
            }
            else if (For.Metadata.ModelType == typeof(bool))
            {
                TagHelperUtil.CreateOrMergeAttribute("class", "form-control-checkbox-sm", output);
                base.Process(context, output);
            }
            else
            {
                output.TagName = "input";
                base.Process(context, output);
            }
        }
    }

    [HtmlTargetElement("input2")]
    public class I3TagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context.AllAttributes["asp-for"] == null)
            {
                output.TagName = "input";
                output.TagMode = TagMode.SelfClosing;

                var typeAttribute = context.AllAttributes["type"];
                if (typeAttribute != null && (typeAttribute.Value.ToString() == "submit" || typeAttribute.Value.ToString() == "button"))
                {
                    var valueAttribute = context.AllAttributes["value"];
                    TagHelperUtil.CreateOrMergeAttribute("value", valueAttribute.Value, output);
                    TagHelperUtil.CreateOrMergeAttribute("type", typeAttribute.Value, output);

                    TagHelperUtil.CreateOrMergeAttribute("class", "btn btn-sm btn-default", output);
                }
                else
                {
                    TagHelperUtil.CreateOrMergeAttribute("class", "form-control form-control-sm", output);
                    base.Process(context, output);
                }
            }
        }
    }

    [HtmlTargetElement("label2")]
    public class Label2TagHelper : LabelTagHelper
    {
        public Label2TagHelper(IHtmlGenerator generator) : base(generator)
        {
        }
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            TagHelperUtil.CreateOrMergeAttribute("class", "col-form-label col-form-label-sm", output);
            //output.Attributes.Add("class", "control-label");
            //output.Attributes.Add("asp-for", output.Attributes["asp-for2"].Value);
            output.TagName = "label";
            output.TagMode = TagMode.StartTagAndEndTag;
            if (For != null)
            {
                await base.ProcessAsync(context, output);
            }
        }

        // Process методът не се извикваше за LabelTagHelper, докато за InputTagHelper се извикваше нормално?!
        //public override void Process(TagHelperContext context, TagHelperOutput output)
        //{
        //    output.Attributes.Add("class", "control-label");
        //    output.TagName = "label";
        //    base.Process(context, output);
        //}
    }

    [HtmlTargetElement("button2")]
    public class LinkButtonTagHelper : AnchorTagHelper
    {
        public LinkButtonTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            TagHelperUtil.CreateOrMergeAttribute("class", "btn btn-sm", output);
            if (context.AllAttributes["href"] != null || context.AllAttributes["asp-action"] != null)
            {
                output.TagName = "a";
                await base.ProcessAsync(context, output);
            }
            else
            {
                output.TagName = "button";
            }
        }
    }

    [HtmlTargetElement("select2")]
    public class Select2TagHelper : SelectTagHelper
    {
        [HtmlAttributeName("asp-items")]
        public new object Items { get; set; }

        public Select2TagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        //public override void Process(TagHelperContext context, TagHelperOutput output)
        //{
        //    base.Process(context, output);
        //}

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            TagHelperUtil.CreateOrMergeAttribute("class", "form-control form-control-sm", output);
            output.TagName = "select";
            output.TagMode = TagMode.StartTagAndEndTag;

            TagHelperAttribute aspItemsAttribute = context.AllAttributes["asp-items"];
            object aspItems = aspItemsAttribute.Value;
            object newValue = null;
            if (aspItems is IEnumerable<CodeNameModel>)
            {
                newValue = new Microsoft.AspNetCore.Mvc.Rendering.SelectList((IEnumerable<CodeNameModel>)aspItems, "Code", "Name");
            }
            else if (aspItems is IEnumerable<IdNameModel>)
            {
                newValue = new Microsoft.AspNetCore.Mvc.Rendering.SelectList((IEnumerable<IdNameModel>)aspItems, "Id", "Name");
            }

            if (newValue != null)
            {
                base.Items = (Microsoft.AspNetCore.Mvc.Rendering.SelectList)newValue;
            }
            else
            {
                base.Items = (Microsoft.AspNetCore.Mvc.Rendering.SelectList)aspItems;
            }
            await base.ProcessAsync(context, output);
        }

        //public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        //{
        //    TagHelperUtil.CreateOrMergeAttribute("class", "form-control form-control-sm", output);
        //    output.TagName = "select";
        //    output.TagMode = TagMode.StartTagAndEndTag;

        //    TagHelperAttribute aspItemsAttribute = context.AllAttributes["asp-items"];
        //    object aspItems = aspItemsAttribute.Value;
        //    object newValue = null;
        //    if (aspItems is IEnumerable<CodeNameModel>)
        //    {
        //        newValue = new Microsoft.AspNetCore.Mvc.Rendering.SelectList((IEnumerable<CodeNameModel>)aspItems, "Code", "Name");
        //    }
        //    else if (aspItems is IEnumerable<IdNameModel>)
        //    {
        //        newValue = new Microsoft.AspNetCore.Mvc.Rendering.SelectList((IEnumerable<IdNameModel>)aspItems, "Id", "Name");
        //    }

        //    if (newValue != null)
        //    {
        //        TagHelperAttribute newAttribute = new TagHelperAttribute(aspItemsAttribute.Name, newValue, aspItemsAttribute.ValueStyle);
        //        output.Attributes.Remove(aspItemsAttribute);
        //        output.Attributes.Add(newAttribute);
        //    }
        //    await base.ProcessAsync(context, output);
        //}
    }

    public static class TagHelperUtil
    {
        public static void CreateOrMergeAttribute(string name, object newContent, TagHelperOutput output)
        {
            var currentAttribute = output.Attributes.FirstOrDefault(attribute => attribute.Name == name);
            if (currentAttribute == null)
            {
                var attribute = new TagHelperAttribute(name, newContent);
                output.Attributes.Add(attribute);
            }
            else
            {
                string[] newContentParts = newContent.ToString().Split(' ');
                List<string> contentPartsToAdd = currentAttribute.Value.ToString().Split(' ').ToList();

                foreach (string newContentPart in newContentParts)
                {
                    if (!contentPartsToAdd.Contains(newContentPart))
                    {
                        contentPartsToAdd.Add(newContentPart);
                    }
                }

                var newAttribute = new TagHelperAttribute(
                    name,
                    string.Join(" ", contentPartsToAdd),
                    currentAttribute.ValueStyle);
                output.Attributes.Remove(currentAttribute);
                output.Attributes.Add(newAttribute);
            }
        }

        public static string TagBuilderToString(this TagBuilder tag)
        {
            var writer = new System.IO.StringWriter();
            tag.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
            return writer.ToString();
        }

    }
}
