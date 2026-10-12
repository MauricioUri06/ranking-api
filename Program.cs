using MiPrimeraApi;
using MiPrimeraApi.Models;

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

builder.Services.AddSingleton<PuntajesService>();

var app = builder.Build(); //construye una aplicacion con las configuraciones del builder

app.Use(async (context, next) =>
{
    Console.WriteLine($"Entrando: {context.Request.Method} {context.Request.Path}");
    await next();
    Console.WriteLine($"Saliendo: {context.Response.StatusCode}");
});

app.UseCors("PermitirReact");



//select
app.MapGet("/puntajes",(PuntajesService servicio) => Results.Ok(servicio.ObtenerTodos()));

//select jugador
app.MapGet("/puntajes/{jugador}", (string jugador, PuntajesService servicio) =>
{
    if (jugador.Contains(" ") || jugador.Length < 3)
        return Results.BadRequest("El jugador debe contener mas de 3 letras y sin espacios");

    bool contiene = servicio.ObtenerTodos().Any(p => p.Jugador == jugador);
    if (contiene == false)
    {
        return Results.NotFound("El jugador no existe");
    }
    
    IEnumerable<Puntajes> encontrado = servicio.ObtenerTodos().Where(p => p.Jugador == jugador);
    
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


