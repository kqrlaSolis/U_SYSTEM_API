import I18next from 'i18next';
import { initReactI18next } from 'react-i18next';
import { En } from './En';
import { Es } from './Es';

export const SupportedLanguages = ['es', 'en'] as const;

export type SupportedLanguage = (typeof SupportedLanguages)[number];

export const DefaultLanguage: SupportedLanguage = 'es';

// Preferencia del usuario; ver Architecture/Internationalization.md
export const LanguageStorageKey = 'USystem.Language';

export function NormalizeLanguage(Language: string | undefined): SupportedLanguage {
  const IsEnglish = Language?.toLowerCase().startsWith('en') ?? false;
  return IsEnglish ? 'en' : DefaultLanguage;
}

export function GetStoredLanguage(): SupportedLanguage {
  if (typeof window === 'undefined') {
    return DefaultLanguage;
  }

  try {
    return NormalizeLanguage(window.localStorage.getItem(LanguageStorageKey) ?? undefined);
  } catch {
    return DefaultLanguage;
  }
}

export function PersistLanguage(Language: SupportedLanguage): void {
  try {
    window.localStorage.setItem(LanguageStorageKey, Language);
  } catch {
    // Si el almacenamiento no está disponible se mantiene el idioma en memoria
  }
}

// La API responde Message y ApiError.Message en el idioma solicitado
export function BuildAcceptLanguage(Language: string | undefined): string {
  return NormalizeLanguage(Language) === 'en'
    ? 'en-US,en;q=0.9,es;q=0.8'
    : 'es-MX,es;q=0.9,en;q=0.8';
}

export async function ChangeLanguage(Language: SupportedLanguage): Promise<void> {
  PersistLanguage(Language);
  await I18next.changeLanguage(Language);
}

void I18next.use(initReactI18next).init({
  resources: {
    es: { translation: Es },
    en: { translation: En },
  },
  lng: GetStoredLanguage(),
  fallbackLng: DefaultLanguage,
  supportedLngs: [...SupportedLanguages],
  interpolation: {
    escapeValue: false,
  },
  returnNull: false,
  react: {
    useSuspense: false,
  },
});

export default I18next;
