namespace SensoBackend.WebApi.Authorization.Data;

// Naming convention:
// - CreateXXX, ReadXXX, UpdateXXX, DeleteXXX - permission to perform a specific CRUD operation
// - MutateXXX - permission to Create, Update and Delete (but not Read)
// - ManageXXX - permission to perform all CRUD operations
public enum Permission
{
    ManageProfiles,
    ManageDashboard,
    ReadNotes,
    MutateNotes,
    ManageReminders
}
