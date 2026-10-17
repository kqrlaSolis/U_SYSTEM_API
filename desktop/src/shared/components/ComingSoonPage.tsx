import { ITPage } from '@axzydev/axzy_ui_system';
import { useTranslation } from 'react-i18next';

export interface ComingSoonPageProps {
  TitleKey?: string;
}

// Pantalla de tránsito para los módulos que llegan en fases posteriores.
// Toda pantalla vive dentro de ITPage (regla del proyecto: ITLayout + ITPage).
export function ComingSoonPage({ TitleKey = 'Common.ComingSoonTitle' }: ComingSoonPageProps) {
  const Translation = useTranslation();

  return (
    <ITPage
      title={Translation.t(TitleKey)}
      empty
      emptyTitle={Translation.t('Common.ComingSoonTitle')}
      emptyDescription={Translation.t('Common.ComingSoonDescription')}
      maxWidth="5xl"
    >
      {null}
    </ITPage>
  );
}
