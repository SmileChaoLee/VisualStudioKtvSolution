#pragma checksum "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/Languages/Details.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "32458c385667944f190c17409e274c8bef7a66a6"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Languages_Details), @"mvc.1.0.view", @"/Views/Languages/Details.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Languages/Details.cshtml", typeof(AspNetCore.Views_Languages_Details))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/_ViewImports.cshtml"
using VodManageSystem;

#line default
#line hidden
#line 2 "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/_ViewImports.cshtml"
using VodManageSystem.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"32458c385667944f190c17409e274c8bef7a66a6", @"/Views/Languages/Details.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"02cc697e419f53a234b34bb840b9297d7aa1010c", @"/Views/_ViewImports.cshtml")]
    public class Views_Languages_Details : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<VodManageSystem.Models.DataModels.Language>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(51, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 3 "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/Languages/Details.cshtml"
  
    ViewData["Title"] = "Language Details";
    ViewBag.Action = "Details";

#line default
#line hidden
            BeginContext(138, 33, true);
            WriteLiteral("\r\n<h3 style=\"text-align:center;\">");
            EndContext();
            BeginContext(172, 17, false);
#line 8 "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/Languages/Details.cshtml"
                          Write(ViewData["Title"]);

#line default
#line hidden
            EndContext();
            BeginContext(189, 9, true);
            WriteLiteral("</h3>\r\n\r\n");
            EndContext();
            DefineSection("Scripts", async() => {
                BeginContext(216, 2, true);
                WriteLiteral("\r\n");
                EndContext();
#line 11 "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/Languages/Details.cshtml"
      
        await Html.RenderPartialAsync("~/Views/Languages/_LanguageValidationScriptsPartial.cshtml");
    

#line default
#line hidden
            }
            );
            BeginContext(338, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            BeginContext(341, 63, false);
#line 16 "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/Languages/Details.cshtml"
Write(await Html.PartialAsync("_LanguageOneRecordForm.cshtml", Model));

#line default
#line hidden
            EndContext();
            BeginContext(404, 4, true);
            WriteLiteral("\r\n\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<VodManageSystem.Models.DataModels.Language> Html { get; private set; }
    }
}
#pragma warning restore 1591