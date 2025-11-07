using System.ComponentModel.DataAnnotations;
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
                try
                {
                    var result = service.Cifrar(request);

                    return Results.Ok(result);
                }
                catch (ValidationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            });

            group.MapPost("/decifrar", (CifraService service, DecifrarRequest request) =>
            {
                try
                {
                    var result = service.Decifrar(request);

                    return Results.Ok(result);
                }
                catch (ValidationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            });

            group.MapPost("/decifrarForcaBruta", async (CifraService service, DecifrarForcaBrutaRequest request) =>
            {
                try
                {
                    var result = await service.DecifrarForcaBruta(request);

                    return Results.Ok(result);
                }
                catch (ValidationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            });
        }
    }
}