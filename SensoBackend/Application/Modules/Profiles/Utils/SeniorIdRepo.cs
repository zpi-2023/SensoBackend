namespace SensoBackend.Application.Modules.Profiles.Utils;

public static class SeniorIdRepo
{
    private static readonly Dictionary<int, SeniorDataToEncode> Seniors = new();

    public static SeniorDataToEncode? Get(int hash)
    {
        Seniors.TryGetValue(hash, out var seniorData);
        if(seniorData is null)
        {
            return null;
        }
        Seniors.Remove(hash);

        return seniorData.ValidTo >= DateTime.Now
            ? seniorData
            : null;
    }

    public static void Add(int hash, SeniorDataToEncode seniorData)
    {
        Seniors[hash] = seniorData;
        RemoveOldRecords();
    }
    
    public static int Hash(SeniorDataToEncode seniorData)
        => seniorData.GetHashCode();

    private static void RemoveOldRecords()
    {
        Seniors
            .Where(r => r.Value.ValidTo < DateTime.Now)
            .Select(r => r.Key)
            .ToList()
            .ForEach(r => Seniors.Remove(r));
    }
}
