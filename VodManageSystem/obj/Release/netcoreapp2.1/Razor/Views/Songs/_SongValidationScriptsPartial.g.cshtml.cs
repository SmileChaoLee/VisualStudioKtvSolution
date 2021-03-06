#pragma checksum "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/Songs/_SongValidationScriptsPartial.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "4febc9a98d8b35a6f2ddcc23e4339989f42ff040"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Songs__SongValidationScriptsPartial), @"mvc.1.0.view", @"/Views/Songs/_SongValidationScriptsPartial.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Songs/_SongValidationScriptsPartial.cshtml", typeof(AspNetCore.Views_Songs__SongValidationScriptsPartial))]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4febc9a98d8b35a6f2ddcc23e4339989f42ff040", @"/Views/Songs/_SongValidationScriptsPartial.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"02cc697e419f53a234b34bb840b9297d7aa1010c", @"/Views/_ViewImports.cshtml")]
    public class Views_Songs__SongValidationScriptsPartial : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<VodManageSystem.Models.DataModels.Song>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(46, 1, true);
            WriteLiteral("\n");
            EndContext();
#line 3 "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/Songs/_SongValidationScriptsPartial.cshtml"
  
    string boolString = "false";    // default is "Add" or "Edit" --> writable
    string vBag = ViewBag.Action as string;
    if (!string.IsNullOrEmpty(vBag))
    {
        if ( (vBag.Trim() == "Delete") || (vBag.Trim() == "Details") )
        {
            // form is readonly
            boolString = "true";
        }
    }

#line default
#line hidden
            BeginContext(380, 153, true);
            WriteLiteral("\n<script type=\"text/javascript\">\n    // jQuery function to set focus on first input item\n    $( document ).ready(function() {\n        // $(\'#Chor\').val(\"");
            EndContext();
            BeginContext(534, 34, false);
#line 19 "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/Songs/_SongValidationScriptsPartial.cshtml"
                      Write(Html.DisplayFor(model=>model.Chor));

#line default
#line hidden
            EndContext();
            BeginContext(568, 32, true);
            WriteLiteral("\");\n        // $(\'#VodYN\').val(\"");
            EndContext();
            BeginContext(601, 35, false);
#line 20 "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/Songs/_SongValidationScriptsPartial.cshtml"
                       Write(Html.DisplayFor(model=>model.VodYn));

#line default
#line hidden
            EndContext();
            BeginContext(636, 57, true);
            WriteLiteral("\");\n        $(\'form input[type=\"text\"]\').prop(\"readonly\",");
            EndContext();
            BeginContext(694, 10, false);
#line 21 "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/Songs/_SongValidationScriptsPartial.cshtml"
                                                Write(boolString);

#line default
#line hidden
            EndContext();
            BeginContext(704, 58, true);
            WriteLiteral(");\n        $(\'form input[type=\"number\"]\').prop(\"readonly\",");
            EndContext();
            BeginContext(763, 10, false);
#line 22 "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/Songs/_SongValidationScriptsPartial.cshtml"
                                                  Write(boolString);

#line default
#line hidden
            EndContext();
            BeginContext(773, 56, true);
            WriteLiteral(");\n        $(\'form input[type=\"date\"]\').prop(\"readonly\",");
            EndContext();
            BeginContext(830, 10, false);
#line 23 "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/Songs/_SongValidationScriptsPartial.cshtml"
                                                Write(boolString);

#line default
#line hidden
            EndContext();
            BeginContext(840, 44, true);
            WriteLiteral(");\n        $(\'form select\').prop(\"disabled\",");
            EndContext();
            BeginContext(885, 10, false);
#line 24 "/Users/chaolee/VisualStudio/Smile_SoftwareSolution/VodManageSystem/Views/Songs/_SongValidationScriptsPartial.cshtml"
                                    Write(boolString);

#line default
#line hidden
            EndContext();
            BeginContext(895, 4729, true);
            WriteLiteral(@");
        $('form input:first').focus();

        // validattion
        $(""#OneSongForm"").validate({
            rules: {
                SongNo: {
                    required: true,
                    minlength: 6,
                    maxlength: 6,
                    pattern: ""[0-9]{6}""
                },
                SongNa: {
                    required: true,
                    maxlength: 36
                },
                SNumWord: {
                    required: true,
                    min: 1,
                    max: 25,
                    minlength: 1,
                    maxlength: 2,
                },
                NumFw: {
                    required: true,
                    min: 0,
                    max: 99,
                    minlength: 1,
                    maxlength: 2,
                },
                NumPw: {
                    required: true,
                    minlength: 1,
                    maxlength: 1,
                    pattern:""[A-Z]""
                },");
            WriteLiteral(@"
                NMpeg: {
                    required: true,
                    minlength: 2,
                    maxlength: 2,
                    min: ""00"",
                    max: ""23"",
                    pattern: ""[1-2][1-3]|[0][0]""
                },
                MMpeg: {
                    required: true,
                    minlength: 2,
                    maxlength: 2,
                    min: ""00"",
                    max: ""23"",
                    pattern: ""[1-2][1-3]|[0][0]""
                },
                VodNo: {
                    required: true,
                    minlength: 6,
                    maxlength: 6,
                    pattern: ""[0-9]{6}""
                },
                Pathname: {
                    required: true,
                    maxlength: 6,
                    pattern: ""[0-9A-Z/\\\\]*""
                }
            },
            
            messages: {
                SongNo: {
                    required: ""Song No. cannot be blank."",
                  ");
            WriteLiteral(@"  minlength: ""At least 6 digits."",
                    maxlength: ""At most 6 digits."",
                    pattern: ""Song No. cannot contain alphabet character.""
                },
                SongNa: {
                    required: ""Song Name cannot be blank."",
                    maxlength: ""The maximum string length is 36.""
                },
                SNumWord: {
                    required: ""number of words is required."",
                    min: ""Minimum value is 1."",
                    max: ""Maximim value is 25."",
                    minlength: ""At least 1 digit."",
                    maxlength: ""At most 2 digits""
                },
                NumFw: {
                    required: ""Strokes for the song is required."",
                    min: ""Minimum value is 0."",
                    max: ""Maximim value is 99."",
                    minlength: ""At least 1 digit."",
                    maxlength: ""At most 2 digits.""
                },
                NumPw: {
                    required");
            WriteLiteral(@": ""Pinyin for the song is required."",
                    minlength: ""At least 1 digit."",
                    maxlength: ""At most 1 digits."",
                    pattern: ""Only A-Z.""
                },
                NMpeg: {
                    required: ""Music track must be specified."",
                    minlength: ""It must be 2 digits."",
                    maxlength: ""It must be 2 digits."",
                    min: ""The minimum is 00"",
                    max: ""The maximum is 23."",
                    pattern: ""No such music track.""
                },
                MMpeg: {
                    required: ""Vocal track must be specified."",
                    minlength: ""It must be 2 digits."",
                    maxlength: ""It must be 2 digits."",
                    min: ""The minimum is 00."",
                    max: ""The maximum is 23."",
                    pattern: ""No such vocal track.""
                },
                VodNo: {
                    required: ""Vod No. is required."",
                ");
            WriteLiteral(@"    minlength: ""The length of Vod No. must be 6 numbers."",
                    maxlength: ""The length of Vod No. must be 6 numbers."",
                    pattern: ""Only numbers are allowed.""
                },
                Pathname: {
                    required: ""Path Name cannot be empty."",
                    maxlength: ""The length of Vod No. must be 6 numbers."",
                    pattern: ""Only 0-9, A-Z, /, and \\ allowed.""
                }
            },

            submitHandler: function(form){
                // alert(""submitHandler"");
                form.submit();
            }
        });
    });

</script>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<VodManageSystem.Models.DataModels.Song> Html { get; private set; }
    }
}
#pragma warning restore 1591
