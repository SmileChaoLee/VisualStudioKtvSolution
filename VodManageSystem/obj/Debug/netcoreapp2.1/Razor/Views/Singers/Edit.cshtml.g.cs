#pragma checksum "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Singers\Edit.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "a2890848def6f0737497abe84b31d4ea432ed3a2"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Singers_Edit), @"mvc.1.0.view", @"/Views/Singers/Edit.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Singers/Edit.cshtml", typeof(AspNetCore.Views_Singers_Edit))]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a2890848def6f0737497abe84b31d4ea432ed3a2", @"/Views/Singers/Edit.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"02cc697e419f53a234b34bb840b9297d7aa1010c", @"/Views/_ViewImports.cshtml")]
    public class Views_Singers_Edit : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<VodManageSystem.Models.DataModels.Singer>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(49, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 3 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Singers\Edit.cshtml"
  
    ViewData["Title"] = "Edit Singer";
    ViewBag.Action = "Edit";

#line default
#line hidden
            BeginContext(128, 33, true);
            WriteLiteral("\r\n<h3 style=\"text-align:center;\">");
            EndContext();
            BeginContext(162, 17, false);
#line 8 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Singers\Edit.cshtml"
                          Write(ViewData["Title"]);

#line default
#line hidden
            EndContext();
            BeginContext(179, 9, true);
            WriteLiteral("</h3>\r\n\r\n");
            EndContext();
            DefineSection("Scripts", async() => {
                BeginContext(206, 2, true);
                WriteLiteral("\r\n");
                EndContext();
#line 11 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Singers\Edit.cshtml"
      
        await Html.RenderPartialAsync("~/Views/Singers/_SingerValidationScriptsPartial.cshtml");
    

#line default
#line hidden
            }
            );
            BeginContext(324, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            BeginContext(327, 61, false);
#line 16 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Singers\Edit.cshtml"
Write(await Html.PartialAsync("_SingerOneRecordForm.cshtml", Model));

#line default
#line hidden
            EndContext();
            BeginContext(388, 4, true);
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<VodManageSystem.Models.DataModels.Singer> Html { get; private set; }
    }
}
#pragma warning restore 1591
