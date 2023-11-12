namespace SensoBackend.Application.Modules.Profiles.Utils;

// TODO: This class is pretty much untestable. Consider refactoring to use a singleton service, and inject ITimerProvider.
public static class SeniorIdRepo
{
    private static readonly Dictionary<int, SeniorDataToEncode> Seniors = new();

    public static SeniorDataToEncode? Get(int hash)
    {
        Seniors.TryGetValue(hash, out var seniorData);
        if (seniorData is null)
        {
            return null;
        }
        Seniors.Remove(hash);

        return seniorData.ValidTo >= DateTimeOffset.UtcNow ? seniorData : null;
    }

    public static void Add(int hash, SeniorDataToEncode seniorData)
    {
        Seniors[hash] = seniorData;
        RemoveOldRecords();
    }

    public static int Hash(SeniorDataToEncode seniorData) => seniorData.GetHashCode();

    private static void RemoveOldRecords()
    {
        Seniors
            .Where(r => r.Value.ValidTo < DateTimeOffset.UtcNow)
            .Select(r => r.Key)
            .ToList()
            .ForEach(r => Seniors.Remove(r));
    }
}
