// Lanza Electron limpiando ELECTRON_RUN_AS_NODE.
// Si esa variable viene del entorno (algunas terminales/IDE la dejan puesta), electron.exe
// arranca como Node y `require('electron').app` es undefined: no aparece ninguna ventana.
const { spawn } = require('node:child_process');
const electronPath = require('electron');

delete process.env.ELECTRON_RUN_AS_NODE;

const Child = spawn(electronPath, ['.'], { stdio: 'inherit', env: process.env });

Child.on('exit', (Code) => {
  process.exit(Code ?? 0);
});
