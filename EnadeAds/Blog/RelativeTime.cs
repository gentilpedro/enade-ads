namespace EnadeAds.Blog;

public static class RelativeTime
{
    /// <summary>"agora", "há 5 min", "há 3 h", "há 2 dias"; depois de uma semana, a data.</summary>
    public static string Format(DateTimeOffset moment, DateTimeOffset now, DateTime localMoment)
    {
        var elapsed = now - moment;
        return elapsed.TotalMinutes switch
        {
            < 1 => "agora",
            < 60 => $"há {(int)elapsed.TotalMinutes} min",
            < 60 * 24 => $"há {(int)elapsed.TotalHours} h",
            < 60 * 24 * 7 => (int)elapsed.TotalDays == 1 ? "há 1 dia" : $"há {(int)elapsed.TotalDays} dias",
            _ => localMoment.ToString("dd/MM/yyyy"),
        };
    }
}
