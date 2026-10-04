using System;

namespace ExwhyzeeSP.Core.Attributes
{
    /// <summary>
    /// Compatibility attribute for legacy [AllowHtml] from ASP.NET MVC 5.
    /// In ASP.NET Core 9, HTML sanitization and encoding are handled at the view/rendering layer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AllowHtmlAttribute : Attribute
    {
    }
}
