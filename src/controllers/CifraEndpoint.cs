using src.models;
using src.services;

namespace src.controllers
{
    public static class CifraEndpoint
    {
        public static void MapCifraEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("").WithTags("Cifra de César");

            group.MapPost("/cifrar", (CifraService service, CifrarRequest request) =>
            {
                var result = service.Cifrar(request);

                return Results.Ok(result);
            });

            group.MapPost("/decifrar", (CifraService service, DecifrarRequest request) =>
            {
                var result = service.Decifrar(request);

                return Results.Ok(result);
            });

            group.MapPost("/decifrarForcaBruta", () =>
            {
                return Results.Ok();
            });
        }
    }
}