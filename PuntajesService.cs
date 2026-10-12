using MiPrimeraApi.Models;

namespace MiPrimeraApi;

public class PuntajesService
{
    private List<Puntajes> puntajes = new List<Puntajes>();

    public List<Puntajes> ObtenerTodos()
    {
        return puntajes;
    }
}