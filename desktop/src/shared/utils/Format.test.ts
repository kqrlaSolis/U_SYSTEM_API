import { describe, expect, it } from 'vitest';
import { FormatCurrency, FormatDate, FormatNumber } from './Format';

describe('Format', () => {
  it('formatea fechas con el locale del idioma activo', () => {
    const SpanishDate = FormatDate('2024-03-05T12:00:00', 'es');
    const EnglishDate = FormatDate('2024-03-05T12:00:00', 'en');

    expect(SpanishDate).toBe(
      new Intl.DateTimeFormat('es-MX', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
      }).format(new Date('2024-03-05T12:00:00')),
    );
    expect(EnglishDate).toBe(
      new Intl.DateTimeFormat('en-US', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
      }).format(new Date('2024-03-05T12:00:00')),
    );
    expect(SpanishDate).not.toBe(EnglishDate);
  });

  it('devuelve cadena vacía con una fecha inválida', () => {
    expect(FormatDate('no-es-fecha')).toBe('');
  });


  it('formatea números y moneda con Intl según el idioma', () => {
    expect(FormatNumber(1234.5, 'es')).toBe(new Intl.NumberFormat('es-MX').format(1234.5));
    expect(FormatNumber(1234.5, 'en')).toBe(new Intl.NumberFormat('en-US').format(1234.5));
    expect(FormatCurrency(1234.5, 'es')).toBe(
      new Intl.NumberFormat('es-MX', {
        style: 'currency',
        currency: 'MXN',
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      }).format(1234.5),
    );
  });

  it('usa pesos mexicanos por defecto y permite otra divisa', () => {
    expect(FormatCurrency(10, 'es')).toContain('$');
    expect(FormatCurrency(10, 'en', 'USD')).toBe(
      new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      }).format(10),
    );
  });
});
