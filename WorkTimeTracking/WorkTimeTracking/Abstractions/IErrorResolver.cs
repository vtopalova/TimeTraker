namespace WorkTimeTracking.Abstractions
{
    internal interface IErrorResolver
    {
        void Resolve(IResult result);
    }
}
