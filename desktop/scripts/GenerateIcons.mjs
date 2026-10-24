// Genera los iconos del instalador y de la ventana a partir del SVG de marca.
// Uso: npm run icons  (los archivos quedan en build/)
import { mkdir, writeFile } from 'node:fs/promises';
import { fileURLToPath } from 'node:url';
import NodePath from 'node:path';
import pngToIco from 'png-to-ico';
import sharp from 'sharp';

const Root = NodePath.resolve(NodePath.dirname(fileURLToPath(import.meta.url)), '..');
const Source = NodePath.join(Root, 'src', 'assets', 'brand', 'UsystemIcon.svg');
const Target = NodePath.join(Root, 'build');

// Windows usa estos tamaños dentro del .ico; electron-builder exige 256 como mínimo.
const IcoSizes = [16, 24, 32, 48, 64, 128, 256];

await mkdir(Target, { recursive: true });

// density alto para que el SVG se rasterice con calidad antes de reducir
const RenderPng = async (Size) =>
  sharp(Source, { density: 384 })
    .resize(Size, Size, { fit: 'contain', background: { r: 0, g: 0, b: 0, alpha: 0 } })
    .png()
    .toBuffer();

const Png512 = await RenderPng(512);
await writeFile(NodePath.join(Target, 'icon.png'), Png512);
await writeFile(NodePath.join(Target, 'icon-256.png'), await RenderPng(256));

const IcoBuffers = await Promise.all(IcoSizes.map((Size) => RenderPng(Size)));
await writeFile(NodePath.join(Target, 'icon.ico'), await pngToIco(IcoBuffers));

console.log(`icon.png (512x512) y icon.ico (${IcoSizes.join(', ')}) generados en ${Target}`);
