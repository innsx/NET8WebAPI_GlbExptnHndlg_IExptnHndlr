using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NET8WebAPI_GlbExptnHndlg_IExptnHndlr.GlobleExptnHandlers
{
    //injecting dependencies DIRECTLY in a class in .Net 8+
    // public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService _problemDetailsService) : IExceptionHandler


    // or dependency injection in a class’s constructor in .NET 7-
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private IProblemDetailsService _problemDetailsService;

        //INJECTING Dependencies
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService)
        {
            _logger = logger;
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError($"Unhandled exception occurred. TraceId: {httpContext.TraceIdentifier}");

            var (statusCode, title) = MapException(exception);

            httpContext.Response.StatusCode = statusCode;
            //httpContext.Response.Clear();
            //httpContext.Response.StatusCode = statusCode;
            //httpContext.Response.ContentType = "application/json";

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = GetProblemType(statusCode),
                //Instance = httpContext.Request.Path,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
                Detail = GetSafeErrorMessage(exception, httpContext)
            };

            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
            problemDetails.Extensions["timestamp"] = DateTime.UtcNow;
            //problemDetails.Extensions["User"] = httpContext.User;

            //In ASP.NET Core, ProblemDetailsContext is a native configuration class
            //that encapsulates all the contextual metadata needed to generate a standardized,
            //machine-readable HTTP error response.
            //It contains vital components such as the current HTTP context,
            //the thrown exception, and the ProblemDetails payload instance itself
            //The ProblemDetailsContext Class contains four primary properties used during error handling customization
            var problemDetailsContext = new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            };

            //NOTE: in SWAGGER documentation UI, you must MANUALLY specified HEADER Media Type to: application/json
            //but in POSTMAN, the default Media Type was set to application/json so you DO NOT NEEDED to MANUALLY specified
            var isProblemDetailsWritten = await _problemDetailsService.TryWriteAsync(problemDetailsContext);

            //If TryWriteAsync method returns false, it usually means there is an Accept header mismatch.
            //For example, if the client sends Accept: text/plain, the default JSON writer will skip it.            
            if (isProblemDetailsWritten == false)
            {
                try
                {
                    //WriteAsJsonAsync: An extension method that serializes a 'problemDetailsContext' object into JSON
                    //and writes it directly to the HTTP response body.
                    //It automatically sets the HTTP Content-Type to application/json
                    //(or application/problem+json depending on configuration)

                    //<ProblemDetailsContext> specified that the input type is "ProblemDetailsContext"

                    //(problemDetailsContext): The specific data object being sent
                    await httpContext.Response.WriteAsJsonAsync<ProblemDetailsContext>(problemDetailsContext);

                    isProblemDetailsWritten = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    isProblemDetailsWritten = false;
                }
            }

            return isProblemDetailsWritten;

        }

        private (int statusCode, string title) MapException(Exception exception) => exception switch
        {
            AppBaseException appEx => ((int)appEx.StatusCode, appEx.Message),
            ArgumentNullException => (StatusCodes.Status400BadRequest, "Invalid argument provided"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid argument provided"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),

            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };


        private static string GetProblemType(int statusCode) => statusCode switch
        {
            400 => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            401 => "https://tools.ietf.org/html/rfc9110#section-15.5.2",
            403 => "https://tools.ietf.org/html/rfc9110#section-15.5.4",
            404 => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            409 => "https://tools.ietf.org/html/rfc9110#section-15.5.10",

            //default
            _ => "https://tools.ietf.org/html/rfc9110#section-15.6.1"
        };


        private string GetSafeErrorMessage(Exception exception, HttpContext httpContext)
        {
            // Only expose details in development
            var env = httpContext.RequestServices.GetRequiredService<IHostEnvironment>();

            if (env.IsDevelopment())
            {
                return exception.Message;
            }

            // In production, only expose messages from our own exceptions
            return exception is AppBaseException ? exception.Message : null!;
        }
    }

}



//using Microsoft.AspNetCore.Diagnostics;
//using Microsoft.AspNetCore.Mvc;
//using System;

//namespace NET8WebAPI_GlbExptnHndlg_IExptnHndlr.GlobleExptnHandlers
//{
//    //injecting dependencies DIRECTLY in a class in .Net 8+
//    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger,
//        IProblemDetailsService problemDetailsService) : IExceptionHandler
//    {
//        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
//        {
//            // 1. Log the unhandled exception
//            logger.LogError($"Unhandled exception occurred. TraceId: {httpContext.TraceIdentifier}");
//            logger.LogError($"Exception: {exception.Message}");

//            var (statusCode, title) = MapException(exception);

//            httpContext.Response.StatusCode = statusCode;
//            //httpContext.Response.Clear();
//            //httpContext.Response.StatusCode = statusCode;
//            //httpContext.Response.ContentType = "application/json";

//            // 3. Create a standardized Problem Details response
//            var problemDetails = new ProblemDetails
//            {
//                Status = statusCode,
//                Title = title,
//                Type = GetProblemType(statusCode),
//                Instance = httpContext.Request.Path,
//                Detail = GetSafeErrorMessage(exception, httpContext)
//            };

//            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
//            problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

//            ProblemDetailsContext problemDetailsContext = new ProblemDetailsContext
//            {
//                HttpContext = httpContext,
//                ProblemDetails = problemDetails
//            };


//            //NOTE: must specified HEADER Media Type: "application/json" in SWAGGER or in POSTMAN
//            //https://github.com/dotnet/aspnetcore/issues/56259
//            //Make it clear what accept header supported when using IProblemDetailsService
//            var isProblemDetailsWritten = await problemDetailsService.TryWriteAsync(problemDetailsContext);

//            //If TryWriteAsync method returns false, it usually means there is an Accept header mismatch.
//            //For example, if the client sends Accept: text/plain, the default JSON writer will skip it.
//            if (isProblemDetailsWritten == false)
//            {
//                try
//                {
//                    await httpContext.Response.WriteAsJsonAsync<ProblemDetailsContext>(problemDetailsContext);
//                    isProblemDetailsWritten = true;
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine(ex.Message);
//                    isProblemDetailsWritten = false;
//                }
//            }

//            // 5. Return 'true' to signal that the exception has been successfully handled
//            // else returns 'false'
//            return isProblemDetailsWritten;
//        }

//        // 2. Map different exceptions to appropriate status codes
//        private (int statusCode, string title) MapException(Exception exception) => exception switch
//        {
//            AppBaseException appBaseException => ((int)appBaseException.StatusCode, appBaseException.Message),
//            ArgumentNullException => (StatusCodes.Status400BadRequest, "Invalid argument provided"),
//            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid argument provided"),
//            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),

//            //default
//            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
//        };


//        private static string GetProblemType(int statusCode) => statusCode switch
//        {
//            400 => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
//            401 => "https://tools.ietf.org/html/rfc9110#section-15.5.2",
//            403 => "https://tools.ietf.org/html/rfc9110#section-15.5.4",
//            404 => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
//            409 => "https://tools.ietf.org/html/rfc9110#section-15.5.10",
//            _ => "https://tools.ietf.org/html/rfc9110#section-15.6.1"
//        };


//        private string GetSafeErrorMessage(Exception exception, HttpContext httpContext)
//        {
//            // Only expose details in development
//            var env = httpContext.RequestServices.GetRequiredService<IHostEnvironment>();

//            if (env.IsDevelopment())
//            {
//                return exception.Message;
//            }

//            // In production, only expose messages from our own exceptions
//            return exception is AppBaseException ? exception.Message : null!;
//        }
//    }

//}





//var problemDetails = new ProblemDetails
//{
//    Status = statusCode,
//    Title = title,
//    Type = GetProblemType(statusCode),
//    Instance = httpContext.Request.Path,
//    Detail = GetSafeErrorMessage(exception, httpContext)
//};

//problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
//problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

//var problemDetailsContext = new ProblemDetailsContext
//{
//    HttpContext = httpContext,
//    ProblemDetails = problemDetails
//};

//httpContext.Response.Clear();
//httpContext.Response.StatusCode = statusCode;
//httpContext.Response.ContentType = "application/json";

//var isResultWritten = await problemDetailsService.TryWriteAsync(problemDetailsContext);

////It returns true if a registered IProblemDetailsWriter successfully wrote the response,
////and false otherwise.
//if (isResultWritten == false)
//{
//    try
//    {
//        await httpContext.Response.WriteAsJsonAsync<ProblemDetailsContext>(problemDetailsContext);
//        isResultWritten = true;
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine(ex.ToString());
//        isResultWritten = false;
//    }
//}

//return isResultWritten;





//--------------------------------------------------
//var httpResponse = httpContext.Response;
//httpResponse.ContentType = "application/json";
//var problemDetailContext = new ProblemDetailsContext
//{
//    HttpContext = httpContext,
//    ProblemDetails = problemDetails,
//    Exception = exception
//};

//var serializedResponse = JsonSerializer.Serialize(problemDetailContext);

//await httpResponse.WriteAsync(serializedResponse);