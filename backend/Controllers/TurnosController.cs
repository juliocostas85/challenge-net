using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Data;
using TurnosMedicos.Helpers;
using TurnosMedicos.Models;

namespace TurnosMedicos.Controllers;

[ApiController]
[Route("[controller]")]
public class TurnosController : ControllerBase
{
    private readonly AppDbContext _context;

    public TurnosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var turnos = await _context.Turnos
            .Include(t => t.Paciente)
            .Include(t => t.Medico)
            .ToListAsync();
        return Ok(turnos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var turno = await _context.Turnos
            .Include(t => t.Paciente)
            .Include(t => t.Medico)
            .FirstOrDefaultAsync(t => t.Id == id);
        if (turno == null) return NotFound();
        return Ok(turno);
    }

    [HttpPost]
    public async Task<IActionResult> CrearTurno([FromBody] Turno turno)
    {
        //fix: validar explícitamente que PacienteId no sea null
        if (turno.PacienteId == null)
            return BadRequest(new { mensaje = "PacienteId es requerido." });

        var paciente = await _context.Pacientes.FindAsync(turno.PacienteId);
        if (paciente == null)
            return NotFound(new { mensaje = "Paciente no encontrado." });

        if (paciente.Bloqueado)
        {
            // Si pasaron 30 días desde el bloqueo, se desbloquea automáticamente
            if (paciente.FechaBloqueo.HasValue && (DateTime.UtcNow - paciente.FechaBloqueo.Value).TotalDays >= 30)
            {
                paciente.Bloqueado = false;
                paciente.FechaBloqueo = null;
                paciente.NoShowCount = 0;
                await _context.SaveChangesAsync();
            }
            else
            {
                return BadRequest(new { mensaje = "El paciente se encuentra bloqueado para agendar turnos online." });
            }
        }

        var medicoExiste = await _context.Medicos.AnyAsync(m => m.Id == turno.MedicoId);
        if (!medicoExiste)
            return NotFound(new { mensaje = "Médico no encontrado." });

        var turnoConflicto = await _context.Turnos.AnyAsync(t =>
            t.MedicoId == turno.MedicoId &&
            t.FechaHora == turno.FechaHora &&
            t.Estado != EstadoTurno.Cancelado);
        if (turnoConflicto)
            return BadRequest(new { mensaje = "El médico ya tiene un turno en ese horario." });

        turno.FechaCreacion = DateTime.UtcNow;
        turno.Estado = EstadoTurno.Pendiente;
        _context.Turnos.Add(turno);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = turno.Id }, turno);
    }

    //fix: era [HttpGet], las acciones que modifican estado deben usar [HttpPut]
    [HttpPut("cancelar/{id}")]
    public async Task<IActionResult> CancelarTurno(int id)
    {
        var turno = await _context.Turnos.FindAsync(id);
        if (turno == null) return NotFound();

        // Bug 2 fix: el umbral era 23 horas pero el enunciado del challenge dice 24
        if (turno.FechaHora - DateTime.UtcNow < TimeSpan.FromHours(24))
            return BadRequest(new { mensaje = "No se puede cancelar con menos de 24 horas de anticipación." });

        turno.Estado = EstadoTurno.Cancelado;
        await _context.SaveChangesAsync();
        return Ok(turno);
    }

    [HttpPost("{id}/ausencia")]
    public async Task<IActionResult> MarcarAusencia(int id)
    {
        var turno = await _context.Turnos.FindAsync(id);
        if (turno == null) return NotFound();

        if (!turno.FechaHora.IsWithinCancellationWindow())
            return BadRequest(new { mensaje = "La ausencia solo puede registrarse dentro de las 24 horas del turno." });

        turno.Estado = EstadoTurno.NoShow;

        //fix: incrementar NoShowCount y bloquear al paciente si llega a 3
        if (turno.PacienteId != null)
        {
            var paciente = await _context.Pacientes.FindAsync(turno.PacienteId);
            if (paciente != null)
            {
                paciente.NoShowCount++;
                if (paciente.NoShowCount >= 3)
                {
                    paciente.Bloqueado = true;
                    paciente.FechaBloqueo = DateTime.UtcNow;
                }
            }
        }

        await _context.SaveChangesAsync();
        return Ok(turno);
    }

    [HttpPut("{id}/estado")]
    public async Task<IActionResult> ActualizarEstado(int id, [FromBody] ActualizarEstadoRequest request)
    {
        var turno = await _context.Turnos.FindAsync(id);
        if (turno == null) return NotFound();

        turno.Estado = request.Estado;
        await _context.SaveChangesAsync();
        return Ok(turno);
    }
}

public class ActualizarEstadoRequest
{
    public EstadoTurno Estado { get; set; }
}
