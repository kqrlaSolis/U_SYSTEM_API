// Catálogo de conceptos de pago.
namespace USystem.Application.Dtos;

public class PaymentConceptDto
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal BaseAmount { get; set; }

    public bool IsRecurring { get; set; }

    public bool IsActive { get; set; }
}

public class CreatePaymentConceptRequest
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal BaseAmount { get; set; }

    public bool IsRecurring { get; set; }
}

public class UpdatePaymentConceptRequest
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal BaseAmount { get; set; }

    public bool IsRecurring { get; set; }

    public bool IsActive { get; set; }
}
