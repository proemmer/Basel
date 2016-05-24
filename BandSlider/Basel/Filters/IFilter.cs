namespace Basel.Filters
{
    public interface IFilter
    {
        double Apply(double inputValue, double priorOutputValue);
    }
}