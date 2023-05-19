public class FlightDetail
{
    public string From { get; set; }
    public string To { get; set; }

    public override string ToString()
    {
        return string.Format("\nFrom - {0} \nTo - {1}", From, To);
    }
}
