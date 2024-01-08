namespace StalkerBelarus.Launcher.Core.Services;

public interface IReleaseComparerService<T>
{
    Task<bool> IsComparerAsync(T gitStorageRelease);
}
