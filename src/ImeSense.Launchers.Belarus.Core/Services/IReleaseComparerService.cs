namespace ImeSense.Launchers.Belarus.Core.Services;

public interface IReleaseComparerService<T>
{
    Task<bool> IsComparerAsync(T gitStorageRelease);
}
