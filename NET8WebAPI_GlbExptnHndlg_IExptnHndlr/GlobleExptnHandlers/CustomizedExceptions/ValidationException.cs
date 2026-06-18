using System.Net;

namespace NET8WebAPI_GlbExptnHndlg_IExptnHndlr.GlobleExptnHandlers.CustomizedExceptions
{
    public sealed class ValidationException : AppBaseException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(IDictionary<string, string[]> errors)
            : base("One or more validation errors occurred.", HttpStatusCode.BadRequest)
        {
            Errors = errors;
        }

        public ValidationException(string field, string error)
            : base("One or more validation errors occurred.", HttpStatusCode.BadRequest)
        {
            Errors = new Dictionary<string, string[]>
            {
                { field, [error] }
            };
        }
    }
}
