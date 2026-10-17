#!/usr/bin/env python3
# Verifica el acceso al bucket S3 de U SYSTEM en modo de solo lectura.
# Uso: python3 tools/CheckS3.py
# Requiere boto3: python3 -m pip install -r tools/Requirements.txt
# No imprime credenciales ni escribe objetos.

import sys
from pathlib import Path

try:
    import boto3
    from botocore.exceptions import BotoCoreError, ClientError
except ImportError:
    print("Falta boto3. Instala con: python3 -m pip install -r tools/Requirements.txt")
    sys.exit(1)

ProjectRoot = Path(__file__).resolve().parents[1]
EnvironmentFile = ProjectRoot / ".env"


def LoadEnvironment():
    # Lee .env sin sobrescribir variables ya exportadas
    Values = {}
    if not EnvironmentFile.exists():
        print(f"No existe {EnvironmentFile}. Copia .env.example a .env y completa los valores.")
        sys.exit(1)

    for Line in EnvironmentFile.read_text(encoding="utf-8").splitlines():
        Line = Line.strip()
        if not Line or Line.startswith("#") or "=" not in Line:
            continue
        Key, Value = Line.split("=", 1)
        Values[Key.strip()] = Value.strip()

    Required = ["AWS_REGION", "AWS_BUCKET_NAME", "AWS_ACCESS_KEY_ID", "AWS_SECRET_ACCESS_KEY"]
    Missing = [Key for Key in Required if not Values.get(Key)]
    if Missing:
        print("Faltan variables en .env:", ", ".join(Missing))
        sys.exit(1)

    return Values


def Main():
    Config = LoadEnvironment()
    Credentials = {
        "aws_access_key_id": Config["AWS_ACCESS_KEY_ID"],
        "aws_secret_access_key": Config["AWS_SECRET_ACCESS_KEY"],
        "region_name": Config["AWS_REGION"],
    }
    Bucket = Config["AWS_BUCKET_NAME"]
    Failed = False

    print(f"Bucket: {Bucket} | Región configurada: {Config['AWS_REGION']}")

    # 1. Identidad de la credencial
    try:
        Identity = boto3.client("sts", **Credentials).get_caller_identity()
        print(f"Identidad: {Identity['Arn']}")
        print(f"Cuenta: {Identity['Account']}")
    except (ClientError, BotoCoreError) as Error:
        print("Identidad: FAIL", Error)
        sys.exit(1)

    # 2. Existencia y acceso al bucket
    S3 = boto3.client("s3", **Credentials)
    try:
        S3.head_bucket(Bucket=Bucket)
        print("head_bucket: OK")
    except ClientError as Error:
        Failed = True
        Headers = Error.response.get("ResponseMetadata", {}).get("HTTPHeaders", {})
        print("head_bucket: FAIL", Error.response["Error"]["Code"],
              "| región del bucket:", Headers.get("x-amz-bucket-region"))

    # 3. Región real del bucket
    try:
        Location = S3.get_bucket_location(Bucket=Bucket)
        print("Región del bucket:", Location.get("LocationConstraint") or "us-east-1")
    except ClientError as Error:
        Failed = True
        print("Región del bucket: FAIL", Error.response["Error"]["Code"])

    # 4. Lectura del prefijo del proyecto
    try:
        Listing = S3.list_objects_v2(Bucket=Bucket, Prefix="USystem/", MaxKeys=5)
        print("list USystem/: OK | objetos:", Listing.get("KeyCount", 0))
    except ClientError as Error:
        Failed = True
        print("list USystem/: FAIL", Error.response["Error"]["Code"])

    # 5. Permiso de lectura de un objeto (404 = permitido, 403 = denegado)
    try:
        S3.head_object(Bucket=Bucket, Key="USystem/.probe")
        print("head_object: el objeto existe")
    except ClientError as Error:
        Code = Error.response["Error"]["Code"]
        print("head_object:", Code, "(404 = lectura permitida, 403 = sin permiso)")
        if Code == "403":
            Failed = True

    if Failed:
        print("\nResultado: sin acceso completo. Revisa la política IAM en docs/Operations/Secrets.md.")
        sys.exit(1)

    print("\nResultado: acceso de lectura verificado.")


if __name__ == "__main__":
    Main()
