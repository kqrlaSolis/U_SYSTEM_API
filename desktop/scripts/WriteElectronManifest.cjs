// El paquete raíz es ESM ("type": "module"), pero Electron necesita CommonJS para
// el proceso principal y, sobre todo, para el preload: con sandbox activo el
// preload no puede ser un módulo ESM. Este manifiesto hace que dist-electron se
// resuelva como CommonJS sin renombrar los archivos que emite tsc.
const FileSystem = require('node:fs');
const NodePath = require('node:path');

const OutputDirectory = NodePath.join(__dirname, '..', 'dist-electron');
const EnvironmentFile = NodePath.join(__dirname, '..', '.env');

function ReadEnvironmentValue(Key) {
  try {
    const Contents = FileSystem.readFileSync(EnvironmentFile, 'utf8');
    for (const Line of Contents.split(/\r?\n/)) {
      const Trimmed = Line.trim();
      if (Trimmed.length === 0 || Trimmed.startsWith('#')) {
        continue;
      }
      const Separator = Trimmed.indexOf('=');
      if (Separator === -1) {
        continue;
      }
      if (Trimmed.slice(0, Separator).trim() !== Key) {
        continue;
      }
      return Trimmed
        .slice(Separator + 1)
        .trim()
        .replace(/^["']|["']$/g, '');
    }
  } catch {
    return undefined;
  }
  return undefined;
}

FileSystem.mkdirSync(OutputDirectory, { recursive: true });
FileSystem.writeFileSync(
  NodePath.join(OutputDirectory, 'package.json'),
  `${JSON.stringify({ type: 'commonjs' }, null, 2)}\n`,
  'utf8',
);

const ApiBaseUrl = ReadEnvironmentValue('VITE_API_BASE_URL') ?? 'http://localhost:5000';
FileSystem.writeFileSync(
  NodePath.join(OutputDirectory, 'Environment.json'),
  `${JSON.stringify({ ApiBaseUrl }, null, 2)}\n`,
  'utf8',
);
