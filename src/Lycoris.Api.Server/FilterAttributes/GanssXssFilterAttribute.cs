using Ganss.Xss;
using Lycoris.Api.Core.Logging;
using Lycoris.Api.Server.Shared;
using Lycoris.Base.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lycoris.Api.Server.FilterAttributes
{
    /// <summary>
    /// XSS过滤
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class GanssXssFilterAttribute : BaseActionAttribute
    {
        private readonly ILycorisLogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        public GanssXssFilterAttribute(ILycorisLoggerFactory factory)
        {
            _logger = factory.CreateLogger<GanssXssFilterAttribute>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ActionHandlerBeforeAsync(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Method.Equals("OPTIONS", StringComparison.CurrentCultureIgnoreCase))
                return Task.CompletedTask;

            var sanitizer = new HtmlSanitizer();

            var settings = GetCustomeAttribute<GanssXssSettingsAttribute>(context.ActionDescriptor);
            if (settings != null)
                GanssXssSettings(sanitizer, settings);

            GanssXssGlobalSettings(sanitizer, settings);

            // 获取Action参数集合
            // 只获取string类型和class类型(但不包括文件类型)
            var ps = context.ActionDescriptor.Parameters.WhereIf(settings != null && settings.IgnoreProperties.HasValue(), x => !settings!.IgnoreProperties.Contains(x.Name))
                                                        .Where(x => x.ParameterType.Equals(typeof(string)) || x.ParameterType.IsClass)
                                                        .Where(x => x.ParameterType.Equals(typeof(IFormFile)) == false)
                                                        .Where(x => x.ParameterType.IsPublic && !x.ParameterType.IsAbstract)
                                                        .ToList();

            foreach (var p in ps)
            {
                var arg = context.ActionArguments[p.Name];

                if (p.ParameterType.Equals(typeof(string)) && arg != null)
                    context.ActionArguments[p.Name] = sanitizer.Sanitize((string)arg);
                else if (p.ParameterType.IsClass)
                    ArgumentsFilter(sanitizer, p.ParameterType, arg);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 遍历修改类的字符串属性
        /// </summary>
        /// <param name="sanitizer"></param>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected virtual object? ArgumentsFilter(HtmlSanitizer sanitizer, Type type, object? obj)
        {
            if (obj == null)
                return obj;

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                try
                {
                    if (!property.CanRead || !property.CanWrite)
                        continue;

                    var value = property.GetValue(obj);
                    if (value == null)
                        continue;

                    if (property.PropertyType == typeof(string))
                    {
                        var trimmedValue = ((string)value).Trim();
                        value = sanitizer.Sanitize(trimmedValue);
                    }
                    else if (property.PropertyType.IsArray)
                    {
                        var array = (Array)value;
                        if (array != null)
                        {
                            for (int i = 0; i < array.Length; i++)
                            {
                                var arrayItem = array.GetValue(i);
                                if (arrayItem != null)
                                {
                                    var sanitizedArrayItem = ArgumentsFilter(sanitizer, arrayItem.GetType(), arrayItem);
                                    array.SetValue(sanitizedArrayItem, i);
                                }
                            }
                        }

                        value = array;
                    }
                    else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        var list = (System.Collections.IList)value;
                        if (list != null)
                        {
                            for (int i = 0; i < list.Count; i++)
                            {
                                var listItem = list[i];
                                if (listItem != null)
                                {
                                    var sanitizedListItem = ArgumentsFilter(sanitizer, listItem.GetType(), listItem);
                                    list[i] = sanitizedListItem;
                                }
                            }
                        }

                        value = list;
                    }
                    else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        var dictionary = (System.Collections.IDictionary)value;
                        if (dictionary != null)
                        {
                            var dictionaryKeys = new object[dictionary.Keys.Count];
                            dictionary.Keys.CopyTo(dictionaryKeys, 0);

                            foreach (var key in dictionaryKeys)
                            {
                                var dictionaryValue = dictionary[key];
                                if (dictionaryValue != null)
                                {
                                    var sanitizedDictionaryValue = ArgumentsFilter(sanitizer, dictionaryValue.GetType(), dictionaryValue);
                                    dictionary[key] = sanitizedDictionaryValue;
                                }
                            }
                        }

                        value = dictionary;
                    }
                    else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                        value = ArgumentsFilter(sanitizer, property.PropertyType, value);

                    property.SetValue(obj, value);

                }
                catch (Exception ex)
                {
                    _logger.Error($"global xss filter failed:{property.Name}", ex);
                    continue;
                }
            }

            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sanitizer"></param>
        /// <param name="settings"></param>
        protected static void GanssXssSettings(HtmlSanitizer sanitizer, GanssXssSettingsAttribute settings)
        {
            if (!settings.Attributes.IsNullOrEmpty())
            {
                var attrs = settings.Attributes.Split(",").Where(x => !x.IsNullOrEmpty()).Distinct().ToList() ?? new List<string>();
                attrs.ForEach(x => sanitizer.AllowedAttributes.Add(x));
            }

            if (!settings.Schemes.IsNullOrEmpty())
            {
                var attrs = settings.Schemes.Split(",").Where(x => !x.IsNullOrEmpty()).Distinct().ToList() ?? new List<string>();
                attrs.ForEach(x => sanitizer.AllowedSchemes.Add(x));
            }

            if (!settings.Tags.IsNullOrEmpty())
            {
                var attrs = settings.Tags.Split(",").Where(x => !x.IsNullOrEmpty()).Distinct().ToList() ?? new List<string>();
                attrs.ForEach(x => sanitizer.AllowedTags.Add(x));
            }

            if (!settings.CssProperties.IsNullOrEmpty())
            {
                var attrs = settings.CssProperties.Split(",").Where(x => !x.IsNullOrEmpty()).Distinct().ToList() ?? new List<string>();
                attrs.ForEach(x => sanitizer.AllowedCssProperties.Add(x));
            }

            if (!settings.UriAttributes.IsNullOrEmpty())
            {
                var attrs = settings.UriAttributes.Split(",").Where(x => !x.IsNullOrEmpty()).Distinct().ToList() ?? new List<string>();
                attrs.ForEach(x => sanitizer.UriAttributes.Add(x));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sanitizer"></param>
        /// <param name="settings"></param>
        protected virtual void GanssXssGlobalSettings(HtmlSanitizer sanitizer, GanssXssSettingsAttribute? settings)
        {
            // 通用设置

            if (!sanitizer.AllowedAttributes.Contains("class"))
                sanitizer.AllowedAttributes.Add("class");
        }
    }
}
