using Coling.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coling.APPI.afiliado.Contratos
{
    public interface InterPersonaLogic
    {
        public Task<bool> InsertarPersona(Persona persona);
        public Task<bool> ModificarPersona(Persona persona, int id);
        public Task<bool> EliminarPersona(int id);
        public Task<List<Persona>> ListarPersona();
        public Task<Persona> ObtenerPersona(int id);


    }
}
