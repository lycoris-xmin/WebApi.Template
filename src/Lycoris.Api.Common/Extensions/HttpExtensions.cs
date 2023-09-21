using Lycoris.Api.Common.Extensions;
using Lycoris.Base.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace Lycoris.Api.Common.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class HttpExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static T GetService<T>(this ActionContext context)
        {
            var obj = context.HttpContext.RequestServices.GetService(typeof(T));
            return obj == null ? throw new Exception($"{typeof(T).FullName} is not register") : (T)obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetServices<T>(this ActionContext context)
        {
            var obj = context.HttpContext.RequestServices.GetServices(typeof(T));
            return obj == null ? throw new Exception($"{typeof(T).FullName} is not register") : obj.Select(x => (T)obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static T GetService<T>(this HttpContext context)
        {
            var obj = context.RequestServices.GetService(typeof(T));
            return obj == null ? throw new Exception($"{typeof(T).FullName} is not register") : (T)obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetServices<T>(this HttpContext context)
        {
            var obj = context.RequestServices.GetServices(typeof(T));
            return obj == null ? throw new Exception($"{typeof(T).FullName} is not register") : obj.Select(x => (T)obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return request.Headers.ContainsKey("X-Requested-With") && request.Headers["X-Requested-With"].Equals("XMLHttpRequest");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static MediaTypeHeaderValue GetMediaTypeHeader(this HttpRequest request) => MediaTypeHeaderValue.Parse(request.ContentType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string? GetFormBoundary(this HttpRequest request)
        {
            var media = MediaTypeHeaderValue.Parse(request.ContentType);
            if (media == null)
                return null;

            return HeaderUtilities.RemoveQuotes(media.Boundary).Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public static string? GetFormPropertyName(this MultipartSection? section)
        {
            if (section == null)
                return null;

            var strArray = section.ContentDisposition?.Split(';');
            var formData = strArray?.FirstOrDefault(part => part.Contains("name="));
            return formData?.Split('=').Last().Trim('"') ?? "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public static string? GetFormFileName(this MultipartSection? section)
        {
            if (section == null)
                return null;

            var strArray = section.ContentDisposition?.Split(';');
            var formData = strArray?.FirstOrDefault(part => part.Contains("filename="));

            return formData?.Split('=').Last().Trim('"') ?? "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentDisposition"></param>
        /// <returns></returns>
        public static bool IsFileSection(string? contentDisposition) => contentDisposition?.Split(';')?.FirstOrDefault(part => part.Contains("filename="))?.Any() ?? false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<string?> GetRequestBodyAsync(this HttpRequest request)
        {
            if (!request.Body.CanRead)
                throw new Exception("the request body can not read");

            request.EnableBuffering();

            try
            {
                var temp = await new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true).ReadToEndAsync();
                if (temp.IsNullOrEmpty())
                    return "";

                if (request.ContentType != null)
                {
                    if (request.ContentType.Contains("application/json"))
                        return temp.ToJsonString();

                    if (request.ContentType.Contains("multipart/form-data"))
                    {
                        var boundary = request.GetFormBoundary();
                        using var ms = new MemoryStream(Encoding.Default.GetBytes(temp));
                        var reader = new MultipartReader(boundary!, ms);
                        var section = await reader.ReadNextSectionAsync();

                        var result = "";

                        while (section != null)
                        {
                            var propName = section.GetFormPropertyName();

                            if (!propName.IsNullOrEmpty())
                            {
                                result += $"\"{propName}\":";

                                if (!IsFileSection(section.ContentDisposition))
                                {
                                    var propValue = await new StreamReader(section.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true).ReadToEndAsync();
                                    result += $"\"{propValue}\"";
                                }
                                else
                                {
                                    result += $"\"[File({section.GetFormFileName()})]\"";
                                }
                            }

                            section = await reader.ReadNextSectionAsync();
                        }

                        return result.IsNullOrEmpty() ? "" : $"{{{result}}}";
                    }
                }

                return request.QueryString.Value;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
