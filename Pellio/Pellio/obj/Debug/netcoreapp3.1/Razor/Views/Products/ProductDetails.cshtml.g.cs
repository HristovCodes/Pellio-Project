#pragma checksum "D:\Repos\Pellio-Project\Pellio\Pellio\Views\Products\ProductDetails.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "c93185c3a5e376bce72a28f05f3f76fe595e9dfa"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Products_ProductDetails), @"mvc.1.0.view", @"/Views/Products/ProductDetails.cshtml")]
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
#nullable restore
#line 1 "D:\Repos\Pellio-Project\Pellio\Pellio\Views\_ViewImports.cshtml"
using Pellio;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Repos\Pellio-Project\Pellio\Pellio\Views\_ViewImports.cshtml"
using Pellio.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "D:\Repos\Pellio-Project\Pellio\Pellio\Views\Products\ProductDetails.cshtml"
using Pellio.ViewModels;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c93185c3a5e376bce72a28f05f3f76fe595e9dfa", @"/Views/Products/ProductDetails.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"cb1d730c8b54ec9056634437fcbc780a2f964676", @"/Views/_ViewImports.cshtml")]
    public class Views_Products_ProductDetails : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<ProductDetails>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 3 "D:\Repos\Pellio-Project\Pellio\Pellio\Views\Products\ProductDetails.cshtml"
  
    ViewBag.Title = "Home Page";


#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<div>\r\n    Product name: ");
#nullable restore
#line 9 "D:\Repos\Pellio-Project\Pellio\Pellio\Views\Products\ProductDetails.cshtml"
             Write(Model.Products.ProductName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n</div>\r\n<div>\r\n    Ingredients: ");
#nullable restore
#line 12 "D:\Repos\Pellio-Project\Pellio\Pellio\Views\Products\ProductDetails.cshtml"
            Write(Model.Products.Ingredients);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n</div>\r\n<div>\r\n    Price: ");
#nullable restore
#line 15 "D:\Repos\Pellio-Project\Pellio\Pellio\Views\Products\ProductDetails.cshtml"
      Write(Model.Products.Price);

#line default
#line hidden
#nullable disable
            WriteLiteral(";\r\n</div>\r\n<!-- this will be a for each loop going through all Models.Products.Comments (its a list) and displaying each comment-->\r\n<div>\r\n    Comments:\r\n</div>\r\n<div>\r\n    Username: ");
#nullable restore
#line 22 "D:\Repos\Pellio-Project\Pellio\Pellio\Views\Products\ProductDetails.cshtml"
         Write(Model.Comments.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n</div>\r\n<div>\r\n    Review: ");
#nullable restore
#line 25 "D:\Repos\Pellio-Project\Pellio\Pellio\Views\Products\ProductDetails.cshtml"
       Write(Model.Comments.Comment);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n</div>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<ProductDetails> Html { get; private set; }
    }
}
#pragma warning restore 1591
