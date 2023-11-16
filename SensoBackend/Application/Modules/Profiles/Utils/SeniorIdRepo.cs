using SensoBackend.Application.Abstractions;

namespace SensoBackend.Application.Modules.Profiles.Utils;

public sealed class SeniorIdRepo(TimeProvider timeProvider) : ISeniorIdRepo
{
    private readonly Dictionary<int, SeniorDataToEncode> _seniors =  [ ];

    public SeniorDataToEncode? Get(int hash)
    {
        if (!_seniors.TryGetValue(hash, out var seniorData))
        {
            return null;
        }

        _seniors.Remove(hash);

        return seniorData.ValidTo >= timeProvider.GetUtcNow() ? seniorData : null;
    }

    public int AssignHash(SeniorDataToEncode seniorData)
    {
        RemoveOldRecords();
        var hash = seniorData.GetHashCode();
        _seniors[hash] = seniorData;
        return hash;
    }

    private void RemoveOldRecords()
    {
        // A bit ugly, but it avoids unnecessary memory allocations
        foreach (
            var key in _seniors
                .Where(r => r.Value.ValidTo < timeProvider.GetUtcNow())
                .Select(r => r.Key)
        )
        {
            _seniors.Remove(key);
        }
    }
}
