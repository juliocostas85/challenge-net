namespace TurnosMedicos.Helpers;

public static class DateTimeExtensions
{
    //fix: usar UtcNow para consistencia con el resto del sistema
    public static bool IsWithinCancellationWindow(this DateTime fechaTurno)
    {
        return (fechaTurno - DateTime.UtcNow).TotalHours <= 24;
    }
}
