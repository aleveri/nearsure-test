namespace ConwayGameMinimalService.Filters
{
    public class ValidPositionsFilter : IEndpointFilter
    {
        private readonly IConfiguration _configuration;

        public ValidPositionsFilter(IConfiguration configuration) => _configuration = configuration;

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            int x, y;
            try
            {
                x = context.GetArgument<int>(1);
                y = context.GetArgument<int>(2);
            }
            catch (Exception)
            {
                x = -1;
                y = -1;
            }

            if (x <= 0 || y <= 0)
                return Results.Problem(_configuration["Messages:NumberGreaterCero"]);

            return await next(context);
        }
    }
}
