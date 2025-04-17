using System;

public class ForecastEntry
{
    public int Id { get; set; }
    public DateTime CheckedAt { get; set; }
    public DateTime ForecastDate { get; set; }
    public double DayTemp { get; set; }
    public double NightTemp { get; set; }
    public double WindSpeed { get; set; }
    public double Rain { get; set; }
    public bool ConditionsMet { get; set; }
}
