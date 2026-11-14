import { describe, expect, it } from 'vitest';
import { CalculateTermProgress } from './TermProgress';

describe('CalculateTermProgress', () => {
  it('calcula el avance y los días restantes a mitad del periodo', () => {
    const Progress = CalculateTermProgress('2026-09-01', '2026-12-10', new Date(2026, 10, 7));

    expect(Progress).not.toBeNull();
    expect(Progress?.TotalDays).toBe(101);
    expect(Progress?.DaysLeft).toBe(33);
    expect(Progress?.Percent).toBe(67);
    expect(Progress?.HasStarted).toBe(true);
    expect(Progress?.HasEnded).toBe(false);
  });

  it('marca 0 % antes de que empiece', () => {
    const Progress = CalculateTermProgress('2027-01-10', '2027-04-30', new Date(2026, 10, 7));

    expect(Progress?.Percent).toBe(0);
    expect(Progress?.HasStarted).toBe(false);
  });

  it('marca 100 % y cero días cuando ya terminó', () => {
    const Progress = CalculateTermProgress('2026-01-10', '2026-04-30', new Date(2026, 10, 7));

    expect(Progress?.Percent).toBe(100);
    expect(Progress?.DaysLeft).toBe(0);
    expect(Progress?.HasEnded).toBe(true);
  });

  it('regresa null sin fechas válidas', () => {
    expect(CalculateTermProgress(null, '2026-12-10', new Date())).toBeNull();
    expect(CalculateTermProgress('2026-12-10', '2026-09-01', new Date())).toBeNull();
  });
});
