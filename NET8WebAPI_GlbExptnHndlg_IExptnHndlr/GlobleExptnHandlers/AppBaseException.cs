using System.Net;

namespace NET8WebAPI_GlbExptnHndlg_IExptnHndlr.GlobleExptnHandlers
{
    public abstract class AppBaseException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        protected AppBaseException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
