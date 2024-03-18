using Coling.APPI.afiliado.Contratos;
using Coling.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Coling.APPI.afiliado.Endpoints
{
    public class PersonaFunction
    {
        private readonly ILogger<PersonaFunction> _logger;
        private readonly InterPersonaLogic personaLogic;

        public PersonaFunction(ILogger<PersonaFunction> logger, InterPersonaLogic personaLogic)
        {
            _logger = logger;
            this.personaLogic = personaLogic;
        }

        [Function("ListarPersonas")]
        public async Task<HttpResponseData> ListarPersona([HttpTrigger(AuthorizationLevel.Function, "get", Route = "ListarPersona")] HttpRequestData req)
        {
            _logger.LogInformation("ejecutando azure function para insertar personas.");
            try
            {
                var listapersonas = personaLogic.ListarPersona();
                var respuesta = req.CreateResponse(HttpStatusCode.OK);
                await respuesta.WriteAsJsonAsync(listapersonas.Result);
                return respuesta;
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.BadRequest);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }
           
        }
        [Function("InsertarPersona")]
        public async Task<HttpResponseData> InsertarPersona([HttpTrigger(AuthorizationLevel.Function, "get", Route = "InsertarPersona")] HttpRequestData req)
        {
            _logger.LogInformation("ejecutando azure function para insertar personas.");
            try
            {
                var per = await req.ReadFromJsonAsync<Persona>() ?? throw new Exception("Debe ingresar una persona con todos sus datos");
                bool seGuardo = await personaLogic.InsertarPersona(per);
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
       /*[Function("ModificarPersona")]
        public async Task<HttpResponseData> ModificarPersona([HttpTrigger(AuthorizationLevel.Function, "put", Route = "ModificarPersona/{id}")] HttpRequestData req, int id)
        {
            _logger.LogInformation("Ejecutando Azure Function para modificar persona.");
            try
            {
                var per = await req.ReadFromJsonAsync<Persona>() ?? throw new Exception("Debe ingresar una persona con todos sus datos");
                bool seModifico = await personaLogic.ModificarPersona(per, id);
                if (seModifico)
                {
                    var response = req.CreateResponse(HttpStatusCode.OK);
                    return response;
                }
                else
                {
                    var response = req.CreateResponse(HttpStatusCode.NotFound);
                    return response;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error al modificar persona: {e.Message}");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteAsJsonAsync(new { message = "Error al modificar persona", error = e.Message });
                return errorResponse;
            }
        }
        [Function("EliminarPersona")]
        public async Task<HttpResponseData> EliminarPersona([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "EliminarPersona/{id}")] HttpRequestData req,int id)
        {
            _logger.LogInformation("Ejecutando Azure Function para eliminar personas.");
            try
            {
                bool seElimino = await personaLogic.EliminarPersona(id);
                if (seElimino)
                {
                    var response = req.CreateResponse(HttpStatusCode.OK);
                    return response;
                }
                else
                {
                    var response = req.CreateResponse(HttpStatusCode.NotFound);
                    return response;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error al eliminar persona: {e.Message}");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteAsJsonAsync(new { message = "Error al eliminar persona", error = e.Message });
                return errorResponse;
            }
        }*/
    }
}
