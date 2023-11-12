using SensoBackend.Application.Abstractions;

namespace SensoBackend.Application.Modules.Profiles.Utils;

public sealed class SeniorIdRepo : ISeniorIdRepo
{
    private readonly Dictionary<int, SeniorDataToEncode> _seniors = new();
    private readonly ITimeProvider _timeProvider;

    public SeniorIdRepo(ITimeProvider timeProvider) => _timeProvider = timeProvider;

    public SeniorDataToEncode? Get(int hash)
    {
        if (!_seniors.TryGetValue(hash, out var seniorData))
        {
            return null;
        }

        _seniors.Remove(hash);

        return seniorData.ValidTo >= _timeProvider.Now ? seniorData : null;
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
        foreach (
            var key in _seniors.Where(r => r.Value.ValidTo < _timeProvider.Now).Select(r => r.Key)
        )
        {
            _seniors.Remove(key);
        }
    }
}
