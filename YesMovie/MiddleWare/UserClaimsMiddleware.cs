namespace YesMovie.MiddleWare
{
    public class UserClaimsMiddleware
    {
        private readonly RequestDelegate _next;

        public UserClaimsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                // Retrieve claims from the user
                var userName = context.User.Identity.Name; // The user's name (default)
                var email = context.User.Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
                var givenName = context.User.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value;
                var familyName = context.User.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value;

                // Add claims to HttpContext.Items so they are accessible elsewhere
                context.Items["UserName"] = userName;
                context.Items["Email"] = email;
                context.Items["GivenName"] = givenName;
                context.Items["FamilyName"] = familyName;
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
