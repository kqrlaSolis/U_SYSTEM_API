// Contrato del puente que el preload publica en window.USystem.
export interface USystemBridge {
  AppVersion: string;
  OpenExternal(Url: string): Promise<boolean>;
}

declare global {
  interface Window {
    // Opcional a propósito: en las pruebas y en el navegador no existe el preload.
    USystem?: USystemBridge;
  }
}

export {};
