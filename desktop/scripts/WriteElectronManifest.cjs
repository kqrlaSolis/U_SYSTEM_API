// El paquete raíz es ESM ("type": "module"), pero Electron necesita CommonJS para
// el proceso principal y, sobre todo, para el preload: con sandbox activo el
// preload no puede ser un módulo ESM. Este manifiesto hace que dist-electron se
// resuelva como CommonJS sin renombrar los archivos que emite tsc.
const FileSystem = require('node:fs');
const NodePath = require('node:path');

const OutputDirectory = NodePath.join(__dirname, '..', 'dist-electron');

FileSystem.mkdirSync(OutputDirectory, { recursive: true });
FileSystem.writeFileSync(
  NodePath.join(OutputDirectory, 'package.json'),
  `${JSON.stringify({ type: 'commonjs' }, null, 2)}\n`,
  'utf8',
);
