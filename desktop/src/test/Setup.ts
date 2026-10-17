import { vi } from 'vitest';

// jsdom no implementa matchMedia ni ResizeObserver y AXZY UI los usa en el tema.
const EmptyMediaQueryList = (Query: string) => ({  matches: false,
  media: Query,
  onchange: null,
  addListener: () => undefined,
  removeListener: () => undefined,
  addEventListener: () => undefined,
  removeEventListener: () => undefined,
  dispatchEvent: () => false,
});

if (typeof window !== 'undefined' && typeof window.matchMedia !== 'function') {
  Object.defineProperty(window, 'matchMedia', {
    writable: true,
    value: EmptyMediaQueryList,
  });
}

if (typeof globalThis.ResizeObserver === 'undefined') {
  class ResizeObserverStub {
    public observe(): void {
      return undefined;
    }

    public unobserve(): void {
      return undefined;
    }

    public disconnect(): void {
      return undefined;
    }
  }

  globalThis.ResizeObserver = ResizeObserverStub as unknown as typeof ResizeObserver;
}

// lottie-web detecta capacidades de canvas al importarse y jsdom no lo soporta:
// en pruebas la animación se reemplaza por un nodo vacío.
vi.mock('lottie-react', () => ({
  Lottie: () => null,
}));
