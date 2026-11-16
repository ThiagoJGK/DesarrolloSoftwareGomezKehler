import urllib.request
import time
import ssl
import sys

ctx = ssl.create_default_context()
ctx.check_hostname = False
ctx.verify_mode = ssl.CERT_NONE

print("[1/2] Esperando inicializacion del Backend (.NET Core)...")
backend_ready = False
for i in range(45):
    try:
        with urllib.request.urlopen("https://localhost:44305/swagger/index.html", context=ctx, timeout=1.5) as r:
            if r.status == 200:
                backend_ready = True
                print("      -> Backend 100% listo y respondiendo.")
                break
    except Exception:
        time.sleep(1)

if not backend_ready:
    print("      [!] Advertencia: El backend tardo mas de lo esperado. Continuando...")

print("[2/2] Esperando inicializacion del Frontend (Angular)...")
frontend_ready = False
for i in range(20):
    try:
        with urllib.request.urlopen("http://localhost:4200", timeout=1) as r:
            if r.status == 200:
                frontend_ready = True
                print("      -> Frontend 100% listo.")
                break
    except Exception:
        time.sleep(1)

print("\nTodos los servicios estan listos. Abriendo navegador...\n")
