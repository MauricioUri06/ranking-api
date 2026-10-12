namespace MiPrimeraApi.Models;

public class Puntajes
{
    private static int contador = 1;
    private int id;
    private string jugador;
    private int puntaje;

    public int Id
    {
        get => id;
        private set => id = value;
    }
    public string Jugador
    {
        get => jugador;
        set => jugador = value;
    }

    public int Puntaje
    {
        get => puntaje;
        set => puntaje = value;
    }

    public Puntajes(string jugador, int puntaje)
    {
        this.id = contador;
        this.jugador = jugador;
        this.puntaje = puntaje;
        contador++;
    }
}