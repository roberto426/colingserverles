using Coling.API.Curriculum.Contratos.Repositorio;
using Coling.API.Curriculum.Modelo;
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

namespace Coling.API.Curriculum.EndPoints
{
    public class GradoAcademicoFunction
    {
        private readonly ILogger<GradoAcademicoFunction> _logger;
        private readonly IGradoAcademicoRepositorio repository;

        public GradoAcademicoFunction(ILogger<GradoAcademicoFunction> logger, IGradoAcademicoRepositorio repository)
        {
            _logger = logger;
            this.repository = repository;
        }

        [Function("InsertarGradoAcademico")]
        [ColingAuthorize(AplicacionRoles.Admin)]
        [OpenApiOperation("InsertarGradoAcademico", "InsertarGradoAcademico", Description = "Sirve para ingresar un grado acad�mico")]
        [OpenApiRequestBody("application/json", typeof(GradoAcademico), Description = "Ingresar un nuevo grado acad�mico")]
        public async Task<HttpResponseData> InsertarGradoAcademico([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            HttpResponseData response;
            try
            {
                var gradoAcademico = await req.ReadFromJsonAsync<GradoAcademico>() ?? throw new Exception("Debe ingresar un grado acad�mico con todos sus datos");
                gradoAcademico.RowKey = Guid.NewGuid().ToString();
                gradoAcademico.Timestamp = DateTimeOffset.UtcNow;
                bool success = await repository.Create(gradoAcademico);
                response = req.CreateResponse(success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            }
            catch (Exception)
            {
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }

        [Function("ListarGradosAcademicos")]
        [ColingAuthorize(AplicacionRoles.Admin + "," + AplicacionRoles.Secretaria + "," + AplicacionRoles.Afiliado)]
        [OpenApiOperation("ListarGradosAcademicos", "ListarGradosAcademicos", Description = "Sirve para listar todos los grados acad�micos")]
        public async Task<HttpResponseData> ListarGradosAcademicos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            HttpResponseData response;
            try
            {
                var listaGradosAcademicos = await repository.GetAll();
                response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(listaGradosAcademicos);
            }
            catch (Exception)
            {
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }

        [Function("EditarGradoAcademico")]
        [ColingAuthorize(AplicacionRoles.Admin)]
        [OpenApiOperation("EditarGradoAcademico", "EditarGradoAcademico", Description = "Sirve para editar un grado acad�mico")]
        [OpenApiRequestBody("application/json", typeof(GradoAcademico), Description = "Editar grado acad�mico")]
        [OpenApiParameter(name: "rowKey", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "ID del grado acad�mico", Description = "El ID del grado acad�mico a editar", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> EditarGradoAcademico(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "EditarGradoAcademico/{rowKey}")] HttpRequestData req,
            string rowKey)
        {
            HttpResponseData response;
            try
            {
                var gradoAcademico = await req.ReadFromJsonAsync<GradoAcademico>() ?? throw new Exception("Debe ingresar un grado acad�mico con todos sus datos");
                gradoAcademico.RowKey = rowKey;
                bool success = await repository.Update(gradoAcademico);
                response = req.CreateResponse(success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            }
            catch (Exception)
            {
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }

        [Function("BorrarGradoAcademico")]
        [ColingAuthorize(AplicacionRoles.Admin)]
        [OpenApiOperation("BorrarGradoAcademico", "BorrarGradoAcademico", Description = "Sirve para eliminar un grado acad�mico")]
        [OpenApiParameter(name: "partitionKey", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "PartitionKey del grado acad�mico", Description = "El PartitionKey del grado acad�mico a borrar", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "rowKey", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "RowKey del grado acad�mico", Description = "El RowKey del grado acad�mico a borrar", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> BorrarGradoAcademico(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "BorrarGradoAcademico/{partitionKey}/{rowKey}")] HttpRequestData req,
            string partitionKey, string rowKey)
        {
            HttpResponseData response;
            try
            {
                bool success = await repository.Delete(partitionKey, rowKey);
                response = req.CreateResponse(success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            }
            catch (Exception)
            {
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }

        [Function("ListarGradoAcademicoById")]
        [ColingAuthorize(AplicacionRoles.Admin + "," + AplicacionRoles.Secretaria + "," + AplicacionRoles.Afiliado)]
        [OpenApiOperation("ListarGradoAcademicoById", "ListarGradoAcademicoById", Description = "Sirve para obtener un grado acad�mico por su ID")]
        [OpenApiParameter(name: "rowKey", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "ID del grado acad�mico", Description = "El ID del grado acad�mico a obtener", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> ListarGradoAcademicoById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ListarGradoAcademicoById/{rowKey}")] HttpRequestData req,
            string rowKey)
        {
            HttpResponseData response;
            try
            {
                var gradoAcademico = await repository.Get(rowKey);
                response = req.CreateResponse(gradoAcademico != null ? HttpStatusCode.OK : HttpStatusCode.NotFound);
                if (gradoAcademico != null)
                {
                    await response.WriteAsJsonAsync(gradoAcademico);
                }
            }
            catch (Exception)
            {
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }
    }
}
