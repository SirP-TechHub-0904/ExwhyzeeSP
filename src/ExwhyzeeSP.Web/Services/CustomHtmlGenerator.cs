using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace ExwhyzeeSP.Web.Services
{
    public class CustomHtmlGenerator : DefaultHtmlGenerator
    {
        public CustomHtmlGenerator(
            IAntiforgery antiforgery,
            IOptions<MvcViewOptions> optionsAccessor,
            IModelMetadataProvider metadataProvider,
            IUrlHelperFactory urlHelperFactory,
            HtmlEncoder htmlEncoder,
            ValidationHtmlAttributeProvider validationAttributeProvider)
            : base(antiforgery, optionsAccessor, metadataProvider, urlHelperFactory, htmlEncoder, validationAttributeProvider)
        {
        }

        public override TagBuilder GenerateActionLink(
            ViewContext viewContext,
            string linkText,
            string actionName,
            string controllerName,
            string protocol,
            string hostname,
            string fragment,
            object routeValues,
            object htmlAttributes)
        {
            var finalHtmlAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var finalRouteValues = new RouteValueDictionary();

            // 1. Process existing htmlAttributes if provided
            if (htmlAttributes != null)
            {
                if (htmlAttributes is IDictionary<string, object> dict)
                {
                    foreach (var kvp in dict)
                    {
                        var key = kvp.Key.TrimStart('@').Replace('_', '-');
                        finalHtmlAttributes[key] = kvp.Value;
                    }
                }
                else
                {
                    var props = htmlAttributes.GetType().GetProperties();
                    foreach (var p in props)
                    {
                        var val = p.GetValue(htmlAttributes);
                        if (val != null)
                        {
                            var key = p.Name.TrimStart('@').Replace('_', '-');
                            finalHtmlAttributes[key] = val;
                        }
                    }
                }
            }

            // 2. Inspect routeValues to see if any HTML attributes (class, style, target, etc.) were passed as routeValues
            if (routeValues != null)
            {
                if (routeValues is RouteValueDictionary rvd)
                {
                    foreach (var kvp in rvd)
                    {
                        if (IsHtmlAttribute(kvp.Key))
                        {
                            var key = kvp.Key.TrimStart('@').Replace('_', '-');
                            if (!finalHtmlAttributes.ContainsKey(key))
                            {
                                finalHtmlAttributes[key] = kvp.Value!;
                            }
                        }
                        else
                        {
                            finalRouteValues[kvp.Key] = kvp.Value;
                        }
                    }
                }
                else if (routeValues is IDictionary<string, object> rDict)
                {
                    foreach (var kvp in rDict)
                    {
                        if (IsHtmlAttribute(kvp.Key))
                        {
                            var key = kvp.Key.TrimStart('@').Replace('_', '-');
                            if (!finalHtmlAttributes.ContainsKey(key))
                            {
                                finalHtmlAttributes[key] = kvp.Value;
                            }
                        }
                        else
                        {
                            finalRouteValues[kvp.Key] = kvp.Value;
                        }
                    }
                }
                else
                {
                    var props = routeValues.GetType().GetProperties();
                    foreach (var p in props)
                    {
                        var val = p.GetValue(routeValues);
                        if (IsHtmlAttribute(p.Name))
                        {
                            var key = p.Name.TrimStart('@').Replace('_', '-');
                            if (val != null && !finalHtmlAttributes.ContainsKey(key))
                            {
                                finalHtmlAttributes[key] = val;
                            }
                        }
                        else
                        {
                            finalRouteValues[p.Name] = val;
                        }
                    }
                }
            }

            return base.GenerateActionLink(
                viewContext,
                linkText,
                actionName,
                controllerName,
                protocol,
                hostname,
                fragment,
                finalRouteValues.Count > 0 ? finalRouteValues : null,
                finalHtmlAttributes.Count > 0 ? finalHtmlAttributes : null);
        }

        private static bool IsHtmlAttribute(string name)
        {
            var cleanName = name.ToLowerInvariant().TrimStart('@').Replace('_', '-');
            return cleanName == "class"
                || cleanName == "style"
                || cleanName == "target"
                || cleanName == "title"
                || cleanName == "role"
                || cleanName == "rel"
                || cleanName == "tabindex"
                || cleanName == "onclick"
                || cleanName == "download"
                || cleanName.StartsWith("data-")
                || cleanName.StartsWith("aria-");
        }
    }
}
