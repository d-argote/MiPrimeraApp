#!/bin/bash
# Compila MiPrimeraApp para Waydroid (x86_64) y la lanza.
#   ./run-waydroid.sh          -> compila + instala + lanza
#   ./run-waydroid.sh --launch -> solo lanza (lo que usa el acceso directo)
set -euo pipefail
cd "$(dirname "$0")"

PKG=com.companyname.miprimeraapp
APK=bin/Release/net10.0-android/android-x64/publish/$PKG-Signed.apk

SESSION_LOG=${XDG_RUNTIME_DIR:-/tmp}/waydroid-session.log

# `waydroid status` dice "Session: RUNNING" varios segundos ANTES de que Android
# esté realmente listo. Si se lanza en esa ventana, el intent se descarta en
# silencio (rc=0, sin ventana). Hay que esperar a "Android with user 0 is ready".
start_session() {
  echo "→ Arrancando sesión de Waydroid..."
  waydroid session stop >/dev/null 2>&1 || true
  sleep 3
  : >"$SESSION_LOG"
  setsid waydroid session start >"$SESSION_LOG" 2>&1 &
  for _ in $(seq 1 60); do
    grep -q "is ready" "$SESSION_LOG" 2>/dev/null && return 0
    sleep 1
  done
  echo "✗ La sesión de Waydroid no terminó de arrancar."; return 1
}

# Verificación sin root: en multi_windows cada app es una ventana propia de
# Hyprland con class waydroid.<paquete>. Es la única señal fiable de que
# realmente arrancó (`waydroid app launch` devuelve 0 aunque no haga nada).
app_window_up() {
  command -v hyprctl >/dev/null 2>&1 || return 0
  hyprctl clients -j 2>/dev/null | grep -q "waydroid.$PKG"
}

try_launch() {
  waydroid app launch "$PKG" 2>&1 | grep -v '^$' || true
  for _ in $(seq 1 20); do
    app_window_up && return 0
    sleep 1
  done
  return 1
}

launch_app() {
  waydroid status 2>/dev/null | grep -q "Session:.*RUNNING" || start_session || return 1
  app_window_up && { echo "✓ Ya estaba abierta."; return 0; }
  try_launch && return 0
  echo "→ La sesión no respondió; reiniciándola y reintentando..."
  start_session || return 1
  try_launch && return 0
  echo "✗ No arrancó. Probá reiniciar el contenedor:"
  echo "    sudo systemctl restart waydroid-container"
  return 1
}

if [[ "${1:-}" == "--launch" ]]; then
  launch_app
  exit $?
fi

# Waydroid corre x86_64 y sideload necesita los ensamblados dentro del APK,
# no Fast Deployment (que es lo que hace `dotnet build -c Debug` y hace que
# la app aborte con "No assemblies found ... .__override__/x86_64").
echo "→ Compilando APK Release para android-x64..."
dotnet publish -f net10.0-android -c Release \
  -p:RuntimeIdentifier=android-x64 \
  -p:AndroidExtractNativeLibs=true \
  --nologo -v q

[[ -f "$APK" ]] || { echo "✗ No se generó el APK: $APK"; exit 1; }

ensure_session

# `waydroid app install` falla EN SILENCIO (rc=0, sin instalar nada) si el
# servicio "package" del contenedor está caído. Instalamos por `pm install`
# vía el directorio compartido /data y verificamos el resultado.
WD_DATA=$HOME/.local/share/waydroid/data
echo "→ Instalando $APK"
pkexec sh -c "
  mkdir -p '$WD_DATA/local/tmp' &&
  cp '$PWD/$APK' '$WD_DATA/local/tmp/app.apk' &&
  chmod 644 '$WD_DATA/local/tmp/app.apk' &&
  waydroid shell -- pm install -r -d /data/local/tmp/app.apk
"

if ! pkexec waydroid shell -- pm path "$PKG" >/dev/null 2>&1; then
  echo "✗ La instalación no quedó registrada."
  echo "  Suele ser el PackageManager del contenedor caído. Reinícialo con:"
  echo "    waydroid session stop && sudo systemctl restart waydroid-container"
  exit 1
fi

echo "→ Lanzando..."
launch_app || exit 1
echo "✓ Listo. Acceso directo: MiPrimeraApp (buscalo en el launcher)"
