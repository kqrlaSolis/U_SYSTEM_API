import { ITI18nProvider } from '@axzydev/axzy_ui_system';
import type { ReactNode } from 'react';
import { I18nextProvider, useTranslation } from 'react-i18next';
import I18next, { NormalizeLanguage } from './Config';

export interface I18nProviderProps {
  children: ReactNode;
}

// AXZY UI tiene su propio catálogo; se le pasa el locale activo para que siga
// el idioma de la aplicación (sus claves internas no se renombran).
function AxzyLocaleBridge({ children }: I18nProviderProps) {
  const Translation = useTranslation();
  const Language = NormalizeLanguage(Translation.i18n.language);

  return (
    <ITI18nProvider locale={Language} fallbackLocale="es">
      {children}
    </ITI18nProvider>
  );
}

export function I18nProvider({ children }: I18nProviderProps) {
  return (
    <I18nextProvider i18n={I18next}>
      <AxzyLocaleBridge>{children}</AxzyLocaleBridge>
    </I18nextProvider>
  );
}
