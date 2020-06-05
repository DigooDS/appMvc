using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DevIO.App.Extensions
{
    public class EmailTagHelper : TagHelper
    {
        public string EmailDomain { get; set; } = "desenvolvedor.io";
        public string EmailDomainB { get; set; } = "gmail.com";
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // Config para o domínio "desenvolvedor.io"
            output.TagName = "a";
            var content = await output.GetChildContentAsync();
            var target = content.GetContent() + "@" + EmailDomain;
            output.Attributes.SetAttribute("href", "mailto:" + target);
            output.Content.SetContent(target);

            // Config para o domínio "gmail.com"
            output.TagName = "a";
            var contentB = await output.GetChildContentAsync();
            var targetB = contentB.GetContent() + "@" + EmailDomainB;
            output.Attributes.SetAttribute("href", "mailto:" + targetB);
            output.Content.SetContent(targetB);
        }
    }
}
