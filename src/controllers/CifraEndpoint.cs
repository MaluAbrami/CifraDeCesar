namespace src.controllers
{
    public static class CifraEndpoint
    {
        public static void MapCifraEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/");

            group.MapPost("/cifrar", () =>
            {
                return Results.Ok();
            });

            group.MapPost("/decifrar", (int id) =>
            {
                return Results.Ok();
            });

            group.MapPost("/decifrarForcaBruta", () =>
            {
                return Results.Ok();
            });
        }
    }
}