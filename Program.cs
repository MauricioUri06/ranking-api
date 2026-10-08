var builder = WebApplication.CreateBuilder(args); //configurador donde se registra lo que necesita el servidor (puerto, entorno, servicios ej: cors etc.)

builder.Services.AddCors(options => //registra servicio CORS(Cross-Origin Resource Service)
{
    options.AddPolicy("PermitirReact", policy =>
    {
        policy.WithOrigins("http://localhost:5173"); //permite hacer peticiones desde este origen
        policy.AllowAnyMethod() //permite todos los verbos HTTP (GET, POST, PUT, DELETE)
            .AllowAnyHeader(); 
    });
});

var app = builder.Build(); //construye una aplicacion con las configuraciones del builder

app.UseCors("PermitirReact");

var  puntajes = new List<Puntajes>();

//select
app.MapGet("/puntajes",() => Results.Ok(puntajes));

//select jugador
app.MapGet("/puntajes/{jugador}", (string jugador) =>
{
    if (jugador.Contains(" ") || jugador.Length < 3)
        return Results.BadRequest("El jugador debe contener mas de 3 letras y sin espacios");

    bool contiene = puntajes.Any(p => p.Jugador == jugador);
    if (contiene == false)
    {
        return Results.NotFound("El jugador no existe");
    }
    
    IEnumerable<Puntajes> encontrado = puntajes.Where(p => p.Jugador == jugador);
    
    return Results.Ok(encontrado);
});

//insert
app.MapPost("/puntajes", (Puntajes puntaje) =>
{
    bool nombreValido = true;
    bool puntajeValido = true;

    if (string.IsNullOrWhiteSpace(puntaje.Jugador) || puntaje.Jugador.Length < 3 || puntaje.Jugador.Contains(" "))
    {
        nombreValido = false;
    }

    if (puntaje.Puntaje < 0)
    {
        puntajeValido = false;
    }

    if (nombreValido && puntajeValido)
    {
        puntajes.Add(puntaje);
        return Results.Ok(puntajes); 
    }
    else if (!nombreValido)
    {
      return Results.BadRequest("El nombre ingresado es invalido. El nombre no puede estar vacio, contener espacios y debe tener una longitud de 3 letras");   
    }
    else
    {
        return Results.BadRequest("El puntaje no puede ser menor a 0");
    }
});

//update puntaje
app.MapPut("/puntajes/{id}", (int id, ActualizarPuntajeDto puntaje) =>
{
    bool encontrado = false;
    bool puntajeValido = true;
    bool puntajeIgual = false;
    
    if (puntaje.Valor < 0)
    {
        puntajeValido = false;
    }
    
    foreach (Puntajes p in puntajes)
    {
        
        if (id == p.Id)
        {
            if (puntaje.Valor == p.Puntaje)
            {
                puntajeIgual = true;
            }
            else
            {
                if (puntajeValido)
                {
                    p.Puntaje = puntaje.Valor;   
                }
            }
            encontrado = true;
            break;
        }
    } 
    
    if (!encontrado)
    {
        return Results.NotFound();
    }
    else if (!puntajeValido)
    {
        return Results.BadRequest("El puntaje nuevo no puede ser menor a 0");
    }
    else if (puntajeIgual)
    {
        return Results.BadRequest("El puntaje nuevo no puede ser igual al actual");
    }
    
    return Results.Ok(puntajes);
});

//Delete instancia de jugador
app.MapDelete("/puntajes/{id}", (int id) => {
    bool encontrado = false;
    foreach (Puntajes p in puntajes)
    {
        if (id == p.Id)
        {
            puntajes.Remove(p);
            encontrado = true;
            break;
        }
    }

    if (!encontrado)
    {
        return Results.NotFound();
    }
    
    return Results.Ok(puntajes);
});

app.Run();

record ActualizarPuntajeDto(int Valor); //clase cuyo unico proposito es trapasar datos de un lado al otro.
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

