// Helpers de formato: siempre con Intl y con el locale del idioma activo.
const LocaleByLanguage: Record<string, string> = {
  es: 'es-MX',
  en: 'en-US',
};

const DefaultCurrency = 'MXN';

function ResolveLocale(Language?: string): string {
  const NormalizedLanguage = Language?.toLowerCase().startsWith('en') ? 'en' : 'es';
  return LocaleByLanguage[NormalizedLanguage] ?? 'es-MX';
}

export function FormatDate(Value: Date | string, Language?: string): string {
  const DateValue = typeof Value === 'string' ? new Date(Value) : Value;

  if (Number.isNaN(DateValue.getTime())) {
    return '';
  }

  return new Intl.DateTimeFormat(ResolveLocale(Language), {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
  }).format(DateValue);
}

export function FormatDateTime(Value: Date | string, Language?: string): string {
  const DateValue = typeof Value === 'string' ? new Date(Value) : Value;

  if (Number.isNaN(DateValue.getTime())) {
    return '';
  }

  return new Intl.DateTimeFormat(ResolveLocale(Language), {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  }).format(DateValue);
}

export function FormatNumber(
  Value: number,
  Language?: string,
  Options?: Intl.NumberFormatOptions,
): string {
  return new Intl.NumberFormat(ResolveLocale(Language), Options).format(Value);
}

export function FormatCurrency(
  Value: number,
  Language?: string,
  Currency: string = DefaultCurrency,
): string {
  return new Intl.NumberFormat(ResolveLocale(Language), {
    style: 'currency',
    currency: Currency,
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(Value);
}
