using Coling.APPI.afiliado.Contratos;
using Coling.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coling.APPI.afiliado.Implementacion
{
    internal class Personalogic : InterPersonaLogic
    {
        private readonly Contexto contexto;

        public Personalogic(Contexto contexto)
        {
            this.contexto = contexto;
        }
        public Task<bool> EliminarPersona(int id)
        {
            throw new NotImplementedException();
            /* try
             {
                 var personaAEliminar = await contexto.Personas.FindAsync(id);
                 if (personaAEliminar == null)
                 {                   
                     return false;
                 }
                 contexto.Personas.Remove(personaAEliminar);
                 int response = await contexto.SaveChangesAsync();

                 return response == 1;
             }
             catch (Exception ex)
             {               
                 Console.WriteLine($"Error al eliminar persona: {ex.Message}");
                 return false;
             }*/
        }

        public async Task<bool> InsertarPersona(Persona persona)
        {
            bool sw = false;
            contexto.Personas.Add(persona);
            int response = await contexto.SaveChangesAsync();
            if (response == 1)
            {
                sw = true;
            }
            return sw;
        }

        public async Task<List<Persona>> ListarPersona()
        {
            var lista = await contexto.Personas.ToListAsync();
            return lista;
        }

        public Task<bool> ModificarPersona(Persona persona, int id)
        {
            /* try
             {
                 var personaExistente = await contexto.Personas.FindAsync(id);
                 if (personaExistente == null)
                 {

                     return false;
                 }

                 personaExistente.Nombre = persona.Nombre;
                 personaExistente.Apellido = persona.Apellido;
                 personaExistente.Fechanacimiento= persona.Fechanacimiento;
                 personaExistente.Estado= persona.Estado;

                  var response = await contexto.SaveChangesAsync();            
                 return response == 1;
             }
             catch (Exception)
             {               
                 return false;
             }*/
            throw new NotImplementedException();
        }

        public Task<Persona> ObtenerPersona(int id)
        {
            throw new NotImplementedException();
        }
    }
}
