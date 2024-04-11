using Coling.API.Afiliados.Contratos;
using Coling.Shared;
using Coling.Utilitarios.Attributes;
using Coling.Utilitarios.Roles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Coling.API.Afiliados.Endpoints
{
    public class TelefonoFunction
    {
        private readonly ILogger<TelefonoFunction> _logger;
        private readonly ITelefonoLogic telefonoLogic;

        public TelefonoFunction(ILogger<TelefonoFunction> logger, ITelefonoLogic telefonoLogic)
        {
            _logger = logger;
            this.telefonoLogic = telefonoLogic;
        }

        [Function("ListarTelefonosPorPersona")]
        [ColingAuthorize(AplicacionRoles.Admin + "," + AplicacionRoles.Secretaria + "," + AplicacionRoles.Afiliado)]
        [OpenApiOperation("ListarTelefonosPorPersona", "ListarTelefonosPorPersona", Description = "Lista los tel�fonos asociados a una persona.")]
        [OpenApiParameter(name: "idPersona", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID de la persona", Description = "El ID de la persona de la cual se desean listar los tel�fonos", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> ListarTelefonosPorPersona(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "listartelefonosporpersona/{idPersona}")] HttpRequestData req,
            int idPersona)
        {
            _logger.LogInformation($"Ejecutando azure function para listar tel�fonos de la persona con Id: {idPersona}.");
            try
            {
                var listaTelefonos = await telefonoLogic.ListarTelefonosPorPersona(idPersona);
                var respuesta = req.CreateResponse(HttpStatusCode.OK);
                await respuesta.WriteAsJsonAsync(listaTelefonos);
                return respuesta;
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }
        }

        [Function("InsertarTelefono")]
        [ColingAuthorize(AplicacionRoles.Admin)]
        [OpenApiOperation("InsertarTelefono", "InsertarTelefono", Description = "Inserta un nuevo tel�fono.")]
        public async Task<HttpResponseData> InsertarTelefono([HttpTrigger(AuthorizationLevel.Function, "post", Route = "insertartelefono")] HttpRequestData req)
        {
            _logger.LogInformation("Ejecutando azure function para insertar tel�fono.");
            try
            {
                var telefono = await req.ReadFromJsonAsync<Telefono>() ?? throw new Exception("Debe ingresar un tel�fono con todos sus datos");
                bool seGuardo = await telefonoLogic.InsertarTelefono(telefono);

                if (seGuardo)
                {
                    var respuesta = req.CreateResponse(HttpStatusCode.OK);
                    return respuesta;
                }

                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }
        }

        [Function("ModificarTelefono")]
        [ColingAuthorize(AplicacionRoles.Admin)]
        [OpenApiOperation("ModificarTelefono", "ModificarTelefono", Description = "Modifica un tel�fono existente.")]
        public async Task<HttpResponseData> ModificarTelefono(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "modificartelefono/{id}")] HttpRequestData req,
            int id)
        {
            _logger.LogInformation($"Ejecutando azure function para modificar tel�fono con Id: {id}.");
            try
            {
                var telefono = await req.ReadFromJsonAsync<Telefono>() ?? throw new Exception("Debe ingresar un tel�fono con todos sus datos");
                bool seModifico = await telefonoLogic.ModificarTelefono(telefono, id);

                if (seModifico)
                {
                    var respuesta = req.CreateResponse(HttpStatusCode.OK);
                    return respuesta;
                }

                return req.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }
        }

        [Function("EliminarTelefono")]
        [ColingAuthorize(AplicacionRoles.Admin)]
        [OpenApiOperation("EliminarTelefono", "EliminarTelefono", Description = "Elimina un tel�fono existente.")]
        public async Task<HttpResponseData> EliminarTelefono(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "eliminartelefono/{id}")] HttpRequestData req,
            int id)
        {
            _logger.LogInformation($"Ejecutando azure function para eliminar tel�fono con Id: {id}.");
            try
            {
                bool seElimino = await telefonoLogic.EliminarTelefono(id);

                if (seElimino)
                {
                    var respuesta = req.CreateResponse(HttpStatusCode.OK);
                    return respuesta;
                }

                return req.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }
        }

        [Function("ObtenerTelefonoById")]
        [ColingAuthorize(AplicacionRoles.Admin + "," + AplicacionRoles.Secretaria + "," + AplicacionRoles.Afiliado)]
        [OpenApiOperation("ObtenerTelefonoById", "ObtenerTelefonoById", Description = "Obtiene un tel�fono por su ID.")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID del tel�fono", Description = "El ID del tel�fono a obtener", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> ObtenerTelefonoById(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "obtenertelefonobyid/{id}")] HttpRequestData req,
    int id)
        {
            _logger.LogInformation($"Ejecutando azure function para obtener tel�fono con Id: {id}.");
            try
            {
                var telefono = await telefonoLogic.ObtenerTelefonoById(id);

                if (telefono != null)
                {
                    var respuesta = req.CreateResponse(HttpStatusCode.OK);
                    await respuesta.WriteAsJsonAsync(telefono);
                    return respuesta;
                }

                return req.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }
        }
    }
}
