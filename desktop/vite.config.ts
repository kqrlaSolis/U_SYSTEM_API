import Tailwindcss from '@tailwindcss/vite';
import ReactPlugin from '@vitejs/plugin-react';
import { defineConfig } from 'vitest/config';

export default defineConfig({
  // Rutas relativas: en producción Electron carga dist/index.html con file://
  base: './',
  // Tailwind v4 procesa @import "tailwindcss" y las capas de Global.css.
  plugins: [Tailwindcss(), ReactPlugin()],
  server: {
    port: 5173,
    strictPort: true,
  },
  build: {
    outDir: 'dist',
    emptyOutDir: true,
    sourcemap: false,
    rollupOptions: {
      // Chunks separados para que el renderer no cargue un único archivo gigante
      output: {
        manualChunks: {
          React: ['react', 'react-dom', 'react-router-dom'],
          AxzyUi: ['@axzydev/axzy_ui_system'],
          I18n: ['i18next', 'react-i18next'],
          Lottie: ['lottie-react', 'lottie-web'],
        },
      },
    },
  },
  test: {
    environment: 'jsdom',
    globals: true,
    include: ['src/**/*.test.{ts,tsx}'],
    setupFiles: ['src/test/Setup.ts'],
    css: false,
    restoreMocks: true,
  },
});
