import http.server
import socketserver
import os
import sys

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
DIST_DIR = os.path.join(BASE_DIR, "dist", "TourismTracking")

if not os.path.exists(DIST_DIR) or not os.path.exists(os.path.join(DIST_DIR, "index.html")):
    print("=" * 60)
    print("Bundle no encontrado en dist/TourismTracking.")
    print("Generando bundle inicial (esto solo se hace una vez)...")
    print("=" * 60)
    os.system("npm run build")

class AngularSPAHandler(http.server.SimpleHTTPRequestHandler):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, directory=DIST_DIR, **kwargs)

    def do_GET(self):
        # Resolver ruta física
        path = self.translate_path(self.path)
        
        # Si el archivo no existe o es un directorio sin index.html, redirigir a index.html (SPA Fallback)
        if not os.path.exists(path) or (os.path.isdir(path) and not os.path.exists(os.path.join(path, "index.html"))):
            self.path = "/index.html"
            
        return super().do_GET()

    def end_headers(self):
        # Desactivar cache para que siempre tome cambios inmediatos
        self.send_header('Cache-Control', 'no-store, no-cache, must-revalidate, max-age=0')
        self.send_header('Pragma', 'no-cache')
        self.send_header('Expires', '0')
        super().end_headers()

    def log_message(self, format, *args):
        # Silenciar logs verbosos de cada asset para ahorrar CPU
        pass

if __name__ == "__main__":
    PORT = 4200
    socketserver.TCPServer.allow_reuse_address = True
    print(f"============================================================")
    print(f"  TourismTracking - Servidor Frontend Ultraliviano (SPA)")
    print(f"  URL: http://localhost:{PORT}")
    print(f"  Consumo de RAM: ~20 MB (vs 3.000 MB de ng serve)")
    print(f"============================================================")
    try:
        with socketserver.TCPServer(("", PORT), AngularSPAHandler) as httpd:
            httpd.serve_forever()
    except KeyboardInterrupt:
        print("\nServidor detenido.")
        sys.exit(0)
