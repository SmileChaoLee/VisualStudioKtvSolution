#pragma checksum "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Languages\Add.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "23589fbf751574959b926ce3c37eb2546e280cb7"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Languages_Add), @"mvc.1.0.view", @"/Views/Languages/Add.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Languages/Add.cshtml", typeof(AspNetCore.Views_Languages_Add))]
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
#line 1 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\_ViewImports.cshtml"
using VodManageSystem;

#line default
#line hidden
#line 2 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\_ViewImports.cshtml"
using VodManageSystem.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"23589fbf751574959b926ce3c37eb2546e280cb7", @"/Views/Languages/Add.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"02cc697e419f53a234b34bb840b9297d7aa1010c", @"/Views/_ViewImports.cshtml")]
    public class Views_Languages_Add : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<VodManageSystem.Models.DataModels.Language>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(51, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 3 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Languages\Add.cshtml"
  
    ViewData["Title"] = "Add Languages";
    ViewBag.Action = "Add";

#line default
#line hidden
            BeginContext(131, 33, true);
            WriteLiteral("\r\n<h3 style=\"text-align:center;\">");
            EndContext();
            BeginContext(165, 17, false);
#line 8 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Languages\Add.cshtml"
                          Write(ViewData["Title"]);

#line default
#line hidden
            EndContext();
            BeginContext(182, 9, true);
            WriteLiteral("</h3>\r\n\r\n");
            EndContext();
            DefineSection("Scripts", async() => {
                BeginContext(209, 2, true);
                WriteLiteral("\r\n");
                EndContext();
#line 11 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Languages\Add.cshtml"
      
        await Html.RenderPartialAsync("~/Views/Languages/_LanguageValidationScriptsPartial.cshtml");
    

#line default
#line hidden
            }
            );
            BeginContext(331, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            BeginContext(334, 63, false);
#line 16 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Languages\Add.cshtml"
Write(await Html.PartialAsync("_LanguageOneRecordForm.cshtml", Model));

#line default
#line hidden
            EndContext();
            BeginContext(397, 4, true);
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
