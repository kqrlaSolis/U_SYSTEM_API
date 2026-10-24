import { contextBridge, ipcRenderer } from 'electron';

// El preload corre con sandbox: solo se expone lo mínimo necesario al renderer.
const AppVersion = String(ipcRenderer.sendSync('usystem:get-app-version') ?? '0.0.0');

const USystemBridge = {
  AppVersion,
  OpenExternal: async (Url: string): Promise<boolean> => {
    const Result = (await ipcRenderer.invoke('usystem:open-external', Url)) as boolean;
    return Result === true;
  },
};

contextBridge.exposeInMainWorld('USystem', USystemBridge);
