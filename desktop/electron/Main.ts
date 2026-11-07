import { app, BrowserWindow, ipcMain, Menu, session, shell } from 'electron';
import { existsSync } from 'node:fs';
import NodePath from 'node:path';
import NodeUrl from 'node:url';

// En desarrollo el renderer lo sirve Vite; en producción se carga dist/index.html.
const DevServerUrl = process.env.VITE_DEV_SERVER_URL ?? '';
const IsDevelopment = DevServerUrl.length > 0;

const DefaultWindowWidth = 1440;
const DefaultWindowHeight = 900;
const MinimumWindowWidth = 1180;
const MinimumWindowHeight = 720;

const AllowedExternalProtocols = ['https:', 'http:'];

let MainWindow: BrowserWindow | null = null;

// Icono de la ventana: en desarrollo sale de build/ y en la app empaquetada de resources/.
const WindowIconPath = (() => {
  const Candidates = IsDevelopment
    ? [NodePath.join(__dirname, '..', 'build', 'icon.png')]
    : [
        NodePath.join(process.resourcesPath, 'icon.png'),
        NodePath.join(__dirname, '..', 'build', 'icon.png'),
      ];
  return Candidates.find((Candidate) => existsSync(Candidate));
})();

function CreateMainWindow(): BrowserWindow {
  const NewWindow = new BrowserWindow({
    width: DefaultWindowWidth,
    height: DefaultWindowHeight,
    minWidth: MinimumWindowWidth,
    minHeight: MinimumWindowHeight,
    show: false,
    autoHideMenuBar: false,
    backgroundColor: '#f5f6fa',
    title: 'U SYSTEM',
    ...(WindowIconPath === undefined ? {} : { icon: WindowIconPath }),
    webPreferences: {
      preload: NodePath.join(__dirname, 'Preload.js'),
      contextIsolation: true,
      nodeIntegration: false,
      sandbox: true,
      webSecurity: true,
      spellcheck: false,
    },
  });

  NewWindow.once('ready-to-show', () => {
    NewWindow.show();
  });

  // El renderer nunca abre ventanas nuevas: los enlaces externos van al navegador.
  NewWindow.webContents.setWindowOpenHandler((Details) => {
    void OpenExternalUrl(Details.url);
    return { action: 'deny' };
  });

  NewWindow.webContents.on('will-navigate', (Event, Url) => {
    const IsSameApp = IsDevelopment ? Url.startsWith(DevServerUrl) : Url.startsWith('file://');
    if (!IsSameApp) {
      Event.preventDefault();
      void OpenExternalUrl(Url);
    }
  });

  if (IsDevelopment) {
    void NewWindow.loadURL(DevServerUrl);
  } else {
    void NewWindow.loadFile(NodePath.join(__dirname, '..', 'dist', 'index.html'));
  }

  NewWindow.on('closed', () => {
    MainWindow = null;
  });

  return NewWindow;
}

async function OpenExternalUrl(Url: string): Promise<boolean> {
  try {
    const ParsedUrl = new NodeUrl.URL(Url);
    if (!AllowedExternalProtocols.includes(ParsedUrl.protocol)) {
      return false;
    }
    await shell.openExternal(ParsedUrl.toString());
    return true;
  } catch {
    // Una URL inválida no debe romper la aplicación
    return false;
  }
}

// CSP solo en producción: en desarrollo Vite inyecta scripts inline para el HMR.
function ApplyContentSecurityPolicy(): void {
  if (IsDevelopment) {
    return;
  }

  session.defaultSession.webRequest.onHeadersReceived((Details, Callback) => {
    Callback({
      responseHeaders: {
        ...Details.responseHeaders,
        'Content-Security-Policy': [
          "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self' data:; connect-src 'self' http://localhost:5000;",
        ],
      },
    });
  });
}

function RegisterIpcHandlers(): void {
  ipcMain.on('usystem:get-app-version', (Event) => {
    Event.returnValue = app.getVersion();
  });

  ipcMain.handle('usystem:open-external', async (_Event, Url: string) => OpenExternalUrl(Url));
}

// Menú mínimo construido con roles: Electron los traduce al idioma del sistema.
function BuildApplicationMenu(): void {
  const Template: Electron.MenuItemConstructorOptions[] = [
    { role: 'fileMenu' },
    { role: 'editMenu' },
    { role: 'viewMenu' },
    { role: 'windowMenu' },
  ];

  Menu.setApplicationMenu(Menu.buildFromTemplate(Template));
}

const HasSingleInstanceLock = app.requestSingleInstanceLock();

if (!HasSingleInstanceLock) {
  app.quit();
} else {
  app.on('second-instance', () => {
    if (MainWindow === null) {
      return;
    }
    if (MainWindow.isMinimized()) {
      MainWindow.restore();
    }
    MainWindow.focus();
  });

  void app.whenReady().then(() => {
    ApplyContentSecurityPolicy();
    RegisterIpcHandlers();
    BuildApplicationMenu();

    MainWindow = CreateMainWindow();

    app.on('activate', () => {
      if (BrowserWindow.getAllWindows().length === 0) {
        MainWindow = CreateMainWindow();
      }
    });
  });

  app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') {
      app.quit();
    }
  });
}
