using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Infrastructure.Persistence;

internal static class SeedData
{
    public static readonly Guid AdminUserId = Guid.Parse("8a2c929f-3a3f-4f67-9d92-56977d042793");
    public static readonly Guid DadosCarroComponentId = Guid.Parse("a3137f9c-6126-4bf4-9dd4-1f3d9cb1aa10");
    public static readonly Guid InitialTemplateId = Guid.Parse("f0d2fb49-7b89-42cc-9e6b-5f2f0e57291b");
    public static readonly Guid ChecklistItemVisualId = Guid.Parse("d2ccf0a2-49b9-4d3f-97a7-a1a2d1bdc1d5");
    public static readonly Guid ChecklistItemDocumentsId = Guid.Parse("a7c75cf0-7016-4d72-9e76-4e927c6de7a8");
    public static readonly DateTimeOffset SeedTimestamp = new(2024, 01, 01, 0, 0, 0, TimeSpan.Zero);

    public static User AdminUser => new(
        AdminUserId,
        "Administrador OS",
        "admin@serviceorders.local",
        "100000.YIbxL2Hjg2HsztaG1d2ZsQ==.aQ9YkZB0C63nQtw4LHZy8pqhQMR+fb68TyK3En1XagA=", // sample PBKDF2 hash for 'Admin@123'
        "Admin",
        true,
        SeedTimestamp);

    public static CustomInputComponent DadosCarroComponent => new(
        DadosCarroComponentId,
        "DadosCarro",
        """
        {
          "fields": [
            { "name": "placa", "label": "Placa", "type": "text", "required": true },
            { "name": "chassi", "label": "Chassi", "type": "text", "required": true },
            { "name": "modelo", "label": "Modelo", "type": "text", "required": true },
            { "name": "ano", "label": "Ano", "type": "number", "required": true }
          ]
        }
        """,
        SeedTimestamp);

    public static ChecklistTemplate InitialChecklistTemplate => new(
        InitialTemplateId,
        "Checklist Veicular - v1",
        "engenharia@serviceorders.local",
        true,
        SeedTimestamp);

    public static IEnumerable<ChecklistTemplateItem> InitialChecklistItems => new[]
    {
        new ChecklistTemplateItem(
            ChecklistItemVisualId,
            InitialTemplateId,
            "Inspeção visual da carroceria",
            false,
            null,
            ChecklistOutcome.Approved,
            0),
        new ChecklistTemplateItem(
            ChecklistItemDocumentsId,
            InitialTemplateId,
            "Registrar dados do veículo",
            true,
            DadosCarroComponentId,
            ChecklistOutcome.Approved,
            1)
    };
}
