import Tseslint from 'typescript-eslint';

// Regla del proyecto: identificadores propios en inglés y PascalCase.
// Excepciones documentadas (Guides/CodingConventions.md): hooks de React (useAlgo),
// la prop nativa `children`, parámetros de librerías y variables de entorno. Las props
// de React/AXZY UI no se revisan aquí porque son la API de un tercero.
export default Tseslint.config(
  {
    ignores: ['dist/**', 'dist-electron/**', 'release/**', 'coverage/**', 'node_modules/**'],
  },
  ...Tseslint.configs.recommended,
  {
    files: ['**/*.{ts,tsx}'],
    rules: {
      '@typescript-eslint/naming-convention': [
        'error',
        {
          selector: 'variable',
          format: ['PascalCase'],
          leadingUnderscore: 'allow',
        },
        {
          selector: 'function',
          format: ['PascalCase'],
        },
        {
          // Los hooks deben empezar con "use" en minúscula: no se pueden renombrar.
          selector: 'function',
          filter: { regex: '^use[A-Z]', match: true },
          format: ['camelCase'],
        },
        // `children` es la prop nativa de React y no se renombra.
        {
          selector: 'parameter',
          filter: { regex: '^children$', match: true },
          format: ['camelCase'],
        },
        {
          selector: 'parameter',
          format: ['PascalCase'],
          leadingUnderscore: 'allow',
        },
        {
          selector: 'typeLike',
          format: ['PascalCase'],
        },
        {
          selector: 'enumMember',
          format: ['PascalCase'],
        },
      ],
      '@typescript-eslint/consistent-type-imports': [
        'error',
        { prefer: 'type-imports', fixStyle: 'inline-type-imports' },
      ],
    },
  },
  {
    // Los scripts de Node son CommonJS: require() es la API del runtime, no código de la app.
    files: ['scripts/**/*.cjs', '**/*.config.{ts,js}'],
    rules: {
      '@typescript-eslint/naming-convention': 'off',
      '@typescript-eslint/no-require-imports': 'off',
    },
  },
);
