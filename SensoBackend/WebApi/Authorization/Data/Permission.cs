namespace SensoBackend.WebApi.Authorization.Data;

public enum Permission //TODO: add some more permissions if needed
{
    // Admin only
    AdminAccess = 1,

    // General permissions
    Write = 11,
    Read = 12,
}
