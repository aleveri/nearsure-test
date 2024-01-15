
namespace ConwayGameMinimalService.Filters
{
    public class ValidIdentityFilter : IEndpointFilter
    {
        private readonly IConfiguration _configuration;

        public ValidIdentityFilter(IConfiguration configuration) => _configuration = configuration;

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            int id;
            try
            {
                id = context.GetArgument<int>(0);
            }
            catch (Exception) { id = -1; }

            if (id <= 0)
                return Results.Problem(_configuration["Messages:NumberGreaterCero"]);

            return await next(context);
        }
    }
}
