import { ITBadget, ITCard, ITProgress, ITSkeleton, ITText } from '@axzydev/axzy_ui_system';
import { useTranslation } from 'react-i18next';
import type { TermSummary } from '../model/TermSummary';

export interface TermProgressCardProps {
  Term: TermSummary | null;
  IsLoading: boolean;
  HasNoTerm: boolean;
  // Wide: datos, avance y fechas en una fila (dashboard); si no, apilado (columna del inicio)
  Wide?: boolean;
}

// Periodo en curso: clave, estado, avance y fechas. Lo usan el inicio y el dashboard.
export function TermProgressCard({ Term, IsLoading, HasNoTerm, Wide = false }: TermProgressCardProps) {
  const Translation = useTranslation();

  return (
    <ITCard title={Translation.t('Dashboard.CurrentTerm')}>
      {IsLoading ? (
        <ITSkeleton variant="text" count={3} />
      ) : Term === null ? (
        <ITText as="p" muted>
          {HasNoTerm ? Translation.t('Dashboard.NoTermHint') : '—'}
        </ITText>
      ) : (
        <div className={Wide ? 'USystemTerm USystemTermWide' : 'USystemTerm'}>
          <div className="USystemTermIdentity">
            <div className="USystemTermHeader">
              <span className="USystemTermCode">{Term.Code}</span>
              <ITBadget label={Translation.t(Term.StatusKey)} color="success" size="sm" />
            </div>
            <ITText as="p">{Term.Name}</ITText>
          </div>

          {Term.Progress !== null && (
            <div className="USystemTermProgress">
              <ITProgress value={Term.Progress.Percent} color="success" size="sm" />
              <div className="USystemTermMeta">
                <span>{Translation.t('Home.TermPercent', { Percent: Term.Progress.Percent })}</span>
                <span>
                  {Term.Progress.HasEnded
                    ? Translation.t('Home.TermEnded')
                    : Translation.t('Home.DaysLeft', { Count: Term.Progress.DaysLeft })}
                </span>
              </div>
            </div>
          )}

          <dl className="USystemTermDates">
            <div>
              <dt>{Translation.t('Home.TermStart')}</dt>
              <dd>{Term.StartDate}</dd>
            </div>
            <div>
              <dt>{Translation.t('Home.TermEnd')}</dt>
              <dd>{Term.EndDate}</dd>
            </div>
          </dl>
        </div>
      )}
    </ITCard>
  );
}
