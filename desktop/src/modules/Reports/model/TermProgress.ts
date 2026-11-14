// Avance del periodo en curso a partir de sus fechas (AAAA-MM-DD), sin depender de la zona horaria.
export interface TermProgress {
  Percent: number;
  DaysLeft: number;
  TotalDays: number;
  HasStarted: boolean;
  HasEnded: boolean;
}

const MillisecondsPerDay = 24 * 60 * 60 * 1000;

// "2026-09-01" con new Date() se lee en UTC y en México se ve como el día anterior
export function ParseLocalDate(Value: string | null): Date | null {
  if (Value === null || Value.length < 10) {
    return null;
  }

  const [Year, Month, Day] = Value.slice(0, 10).split('-').map(Number);
  if (!Year || !Month || !Day) {
    return null;
  }

  return new Date(Year, Month - 1, Day);
}

function StartOfDay(Value: Date): Date {
  return new Date(Value.getFullYear(), Value.getMonth(), Value.getDate());
}

export function CalculateTermProgress(
  StartDate: string | null,
  EndDate: string | null,
  Today: Date,
): TermProgress | null {
  const Start = ParseLocalDate(StartDate);
  const End = ParseLocalDate(EndDate);
  if (Start === null || End === null || End < Start) {
    return null;
  }

  const Current = StartOfDay(Today);
  const TotalDays = Math.round((End.getTime() - Start.getTime()) / MillisecondsPerDay) + 1;
  const ElapsedDays = Math.round((Current.getTime() - Start.getTime()) / MillisecondsPerDay) + 1;
  const ClampedElapsed = Math.min(Math.max(ElapsedDays, 0), TotalDays);

  return {
    Percent: Math.round((ClampedElapsed / TotalDays) * 100),
    DaysLeft: Math.max(TotalDays - Math.max(ElapsedDays, 0), 0),
    TotalDays,
    HasStarted: Current >= Start,
    HasEnded: Current > End,
  };
}
