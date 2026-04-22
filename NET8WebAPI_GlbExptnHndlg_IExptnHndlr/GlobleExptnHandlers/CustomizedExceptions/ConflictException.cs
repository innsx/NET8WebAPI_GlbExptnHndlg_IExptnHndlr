using System.Net;

namespace NET8WebAPI_GlbExptnHndlg_IExptnHndlr.GlobleExptnHandlers.CustomizedExceptions
{
    public sealed class ConflictException : AppBaseException
    {
        public ConflictException(string message) : base(message, HttpStatusCode.Conflict)
        {
        }
    }
}
