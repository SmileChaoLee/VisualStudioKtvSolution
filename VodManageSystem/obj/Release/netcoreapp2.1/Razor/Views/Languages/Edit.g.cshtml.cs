#pragma checksum "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/Languages/Edit.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "08a975d7b1731c4c9ac9fcdc9c5ee604ed7b4541"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Languages_Edit), @"mvc.1.0.view", @"/Views/Languages/Edit.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Languages/Edit.cshtml", typeof(AspNetCore.Views_Languages_Edit))]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"08a975d7b1731c4c9ac9fcdc9c5ee604ed7b4541", @"/Views/Languages/Edit.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"02cc697e419f53a234b34bb840b9297d7aa1010c", @"/Views/_ViewImports.cshtml")]
    public class Views_Languages_Edit : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<VodManageSystem.Models.DataModels.Language>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(51, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 3 "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/Languages/Edit.cshtml"
  
    ViewData["Title"] = "Edit Language";
    ViewBag.Action = "Edit";

#line default
#line hidden
            BeginContext(132, 33, true);
            WriteLiteral("\r\n<h3 style=\"text-align:center;\">");
            EndContext();
            BeginContext(166, 17, false);
#line 8 "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/Languages/Edit.cshtml"
                          Write(ViewData["Title"]);

#line default
#line hidden
            EndContext();
            BeginContext(183, 9, true);
            WriteLiteral("</h3>\r\n\r\n");
            EndContext();
            DefineSection("Scripts", async() => {
                BeginContext(210, 2, true);
                WriteLiteral("\r\n");
                EndContext();
#line 11 "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/Languages/Edit.cshtml"
      
        await Html.RenderPartialAsync("~/Views/Languages/_LanguageValidationScriptsPartial.cshtml");
    

#line default
#line hidden
            }
            );
            BeginContext(332, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            BeginContext(335, 63, false);
#line 16 "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/Languages/Edit.cshtml"
Write(await Html.PartialAsync("_LanguageOneRecordForm.cshtml", Model));

#line default
#line hidden
            EndContext();
            BeginContext(398, 4, true);
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