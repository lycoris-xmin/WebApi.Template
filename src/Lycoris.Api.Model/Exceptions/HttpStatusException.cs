using System.Net;

namespace Lycoris.Api.Model.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpStatusException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public HttpStatusException() : base("InternalServerError")
        {
            this.HttpStatusCode = HttpStatusCode.InternalServerError;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="HttpStatusCode"></param>
        /// <param name="message"></param>
        public HttpStatusException(HttpStatusCode HttpStatusCode, string message) : base(message)
        {
            this.HttpStatusCode = HttpStatusCode;
        }
    }
}
