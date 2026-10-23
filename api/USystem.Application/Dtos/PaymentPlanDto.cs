// Planes de pago y cargos por alumno.
namespace USystem.Application.Dtos;

public class PaymentPlanDto
{
    public int Id { get; set; }

    public int? CareerId { get; set; }

    public int AcademicTermId { get; set; }

    public string Name { get; set; } = string.Empty;

    public byte InstallmentCount { get; set; }

    public bool IsActive { get; set; }
}

public class PaymentPlanRequest
{
    public int? CareerId { get; set; }

    public int AcademicTermId { get; set; }

    public string Name { get; set; } = string.Empty;

    public byte InstallmentCount { get; set; }

    public bool IsActive { get; set; }
}

public class ChargeDto
{
    public long Id { get; set; }

    public long StudentId { get; set; }

    public int AcademicTermId { get; set; }

    public int PaymentConceptId { get; set; }

    public decimal Amount { get; set; }

    public decimal Discount { get; set; }

    public decimal Surcharge { get; set; }

    public DateOnly DueDate { get; set; }

    // Pending, Partial, Paid, Overdue, Cancelled
    public string Status { get; set; } = string.Empty;

    public decimal Total => Amount - Discount + Surcharge;
}

public class ChargeRequest
{
    public long StudentId { get; set; }

    public int AcademicTermId { get; set; }

    public int PaymentConceptId { get; set; }

    public decimal Amount { get; set; }

    public decimal Discount { get; set; }

    public decimal Surcharge { get; set; }

    public DateOnly DueDate { get; set; }
}
