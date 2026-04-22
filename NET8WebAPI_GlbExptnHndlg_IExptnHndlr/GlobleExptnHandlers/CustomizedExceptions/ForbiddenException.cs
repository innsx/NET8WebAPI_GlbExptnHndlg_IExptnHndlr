using System.Net;

namespace NET8WebAPI_GlbExptnHndlg_IExptnHndlr.GlobleExptnHandlers.CustomizedExceptions
{
    public sealed class ForbiddenException : AppBaseException
    {
        public ForbiddenException(string message) : base(message, HttpStatusCode.Forbidden)
        {
        }
    }
}