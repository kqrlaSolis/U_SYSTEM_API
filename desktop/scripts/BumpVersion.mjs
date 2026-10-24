// Sube la versión de package.json antes de empaquetar (cada dist genera una versión nueva).
// Uso: node scripts/BumpVersion.mjs [--patch|--minor|--major] [--dry-run]
import { readFile, writeFile } from 'node:fs/promises';
import { fileURLToPath } from 'node:url';
import NodePath from 'node:path';

const Root = NodePath.resolve(NodePath.dirname(fileURLToPath(import.meta.url)), '..');
const PackagePath = NodePath.join(Root, 'package.json');

const ArgumentsList = process.argv.slice(2);
const IsDryRun = ArgumentsList.includes('--dry-run');
const RequestedType =
  ArgumentsList.find((Argument) => ['--patch', '--minor', '--major'].includes(Argument)) ?? '--patch';

const PackageJson = JSON.parse(await readFile(PackagePath, 'utf8'));
const Match = /^(\d+)\.(\d+)\.(\d+)$/.exec(PackageJson.version ?? '');

if (Match === null) {
  console.error(`Versión no semver en package.json: ${PackageJson.version}`);
  process.exit(1);
}

let [Major, Minor, Patch] = [Number(Match[1]), Number(Match[2]), Number(Match[3])];

if (RequestedType === '--major') {
  Major += 1;
  Minor = 0;
  Patch = 0;
} else if (RequestedType === '--minor') {
  Minor += 1;
  Patch = 0;
} else {
  Patch += 1;
}

const NewVersion = `${Major}.${Minor}.${Patch}`;

if (IsDryRun) {
  console.log(`dry-run: ${PackageJson.version} -> ${NewVersion}`);
  process.exit(0);
}

PackageJson.version = NewVersion;
await writeFile(PackagePath, `${JSON.stringify(PackageJson, null, 2)}\n`, 'utf8');
console.log(`Versión: ${Match[0]} -> ${NewVersion} (${RequestedType.replace('--', '')})`);
