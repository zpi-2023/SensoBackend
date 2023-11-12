using SensoBackend.Application.Modules.Profiles.Utils;

namespace SensoBackend.Application.Abstractions;

public interface ISeniorIdRepo
{
    SeniorDataToEncode? Get(int hash);
    int AssignHash(SeniorDataToEncode seniorData);
}
