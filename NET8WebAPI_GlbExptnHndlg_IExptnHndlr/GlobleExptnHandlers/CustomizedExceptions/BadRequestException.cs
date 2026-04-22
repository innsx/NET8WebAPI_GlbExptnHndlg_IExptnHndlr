using System.Net;

namespace NET8WebAPI_GlbExptnHndlg_IExptnHndlr.GlobleExptnHandlers.CustomizedExceptions
{
    public sealed class BadRequestException : AppBaseException
    {
        public BadRequestException(string message) : base(message, HttpStatusCode.BadRequest)
        {
        }
    }
}
