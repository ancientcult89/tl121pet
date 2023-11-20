using System.Net;

namespace tl121pet.Infrastructure
{
    public record ExceptionResponse(HttpStatusCode StatusCode, string Description);
}
