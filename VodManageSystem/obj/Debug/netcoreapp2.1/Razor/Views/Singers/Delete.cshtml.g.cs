#pragma checksum "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Singers\Delete.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "3e037ab0c539ddb31ff760553395e01eee0c751b"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Singers_Delete), @"mvc.1.0.view", @"/Views/Singers/Delete.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Singers/Delete.cshtml", typeof(AspNetCore.Views_Singers_Delete))]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"3e037ab0c539ddb31ff760553395e01eee0c751b", @"/Views/Singers/Delete.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"02cc697e419f53a234b34bb840b9297d7aa1010c", @"/Views/_ViewImports.cshtml")]
    public class Views_Singers_Delete : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<VodManageSystem.Models.DataModels.Singer>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(49, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 3 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Singers\Delete.cshtml"
  
    ViewData["Title"] = "Delete Singer";
    ViewBag.Action = "Delete";

#line default
#line hidden
            BeginContext(132, 33, true);
            WriteLiteral("\r\n<h3 style=\"text-align:center;\">");
            EndContext();
            BeginContext(166, 17, false);
#line 8 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Singers\Delete.cshtml"
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
#line 11 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Singers\Delete.cshtml"
      
        await Html.RenderPartialAsync("~/Views/Singers/_SingerValidationScriptsPartial.cshtml");
    

#line default
#line hidden
            }
            );
            BeginContext(328, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            BeginContext(331, 61, false);
#line 16 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Singers\Delete.cshtml"
Write(await Html.PartialAsync("_SingerOneRecordForm.cshtml", Model));

#line default
#line hidden
            EndContext();
            BeginContext(392, 4, true);
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
