namespace RedMath.Calculus
{
    public interface IFunction<Domain, Range>
    {
        Range Compute(Domain input);
    }
}
