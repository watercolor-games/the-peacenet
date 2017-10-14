
OUTDIR := .
CXX := c++
CXXW32 := i686-w64-mingw32-c++
CXXW64 := x86_64-w64-mingw32-c++
PACKAGES := pangomm-1.4
CFLAGSBASE := -fPIC -static
CFLAGS := $(CFLAGSBASE) $(shell pkg-config --cflags --libs $(PACKAGES))

ifeq ($(OS),Windows_NT)
	ifeq ($(PROCESSOR_ARCHITECTURE), AMD64)
		DEFAULT := windows64
	else
		DEFAULT := windows32
	endif
else
	ifeq ($(shell uname -m), x86_64)
		DEFAULT := linux64
	else
		DEFAULT := linux32
	endif
endif

default: $(DEFAULT)

%_lin64.o: %.cpp
	$(CXX) $(CFLAGS) -m64 -c -o $@ $<

%_lin32.o: %.cpp
	$(CXX) $(CFLAGS) -m32 -c -o $@ $<

$(OUTDIR)/x64/libPlexNative.so: PangoTextRenderer_lin64.o
	mkdir -p $(OUTDIR)/x64
	$(CXX) $(CFLAGS) -m64 -o $@ $<

$(OUTDIR)/x86/libPlexNative.so: PangoTextRenderer_lin32.o
	mkdir -p $(OUTDIR)/x86
	$(CXX) $(CFLAGS) -m32 -o $@ $<

$(OUTDIR)/x64/PlexNative.dll: PangoTextRenderer.cpp
	mkdir -p $(OUTDIR)/x64
	$(CXXW64) -fPIC -shared -m64 $(shell pkg-config --cflags $(PACKAGES)) $< -o $@ $(shell pkg-config --libs $(PACKAGES)) -Wl,--add-stdcall-alias

$(OUTDIR)/x86/PlexNative.dll: PangoTextRenderer.cpp
	mkdir -p $(OUTDIR)/x86
	cp /c/msys64/mingw32/bin/libbz2-1.dll $(OUTDIR)/x86/libbz2-1.dll
	cp /c/msys64/mingw32/bin/libcairo-2.dll $(OUTDIR)/x86/libcairo-2.dll
	cp /c/msys64/mingw32/bin/libcairomm-1.0-1.dll $(OUTDIR)/x86/libcairomm-1.0-1.dll
	cp /c/msys64/mingw32/bin/libexpat-1.dll $(OUTDIR)/x86/libexpat-1.dll
	cp /c/msys64/mingw32/bin/libffi-6.dll $(OUTDIR)/x86/libffi-6.dll
	cp /c/msys64/mingw32/bin/libfontconfig-1.dll $(OUTDIR)/x86/libfontconfig-1.dll
	cp /c/msys64/mingw32/bin/libfreetype-6.dll $(OUTDIR)/x86/libfreetype-6.dll
	cp /c/msys64/mingw32/bin/libgcc_s_dw2-1.dll $(OUTDIR)/x86/libgcc_s_dw2-1.dll
	cp /c/msys64/mingw32/bin/libglib-2.0-0.dll $(OUTDIR)/x86/libglib-2.0-0.dll
	cp /c/msys64/mingw32/bin/libglibmm-2.4-1.dll $(OUTDIR)/x86/libglibmm-2.4-1.dll
	cp /c/msys64/mingw32/bin/libgmodule-2.0-0.dll $(OUTDIR)/x86/libgmodule-2.0-0.dll
	cp /c/msys64/mingw32/bin/libgobject-2.0-0.dll $(OUTDIR)/x86/libgobject-2.0-0.dll
	cp /c/msys64/mingw32/bin/libgraphite2.dll $(OUTDIR)/x86/libgraphite2.dll
	cp /c/msys64/mingw32/bin/libharfbuzz-0.dll $(OUTDIR)/x86/libharfbuzz-0.dll
	cp /c/msys64/mingw32/bin/libiconv-2.dll $(OUTDIR)/x86/libiconv-2.dll
	cp /c/msys64/mingw32/bin/libintl-8.dll $(OUTDIR)/x86/libintl-8.dll
	cp /c/msys64/mingw32/bin/libpango-1.0-0.dll $(OUTDIR)/x86/libpango-1.0-0.dll
	cp /c/msys64/mingw32/bin/libpangocairo-1.0-0.dll $(OUTDIR)/x86/libpangocairo-1.0-0.dll
	cp /c/msys64/mingw32/bin/libpangoft2-1.0-0.dll $(OUTDIR)/x86/libpangoft2-1.0-0.dll
	cp /c/msys64/mingw32/bin/libpangomm-1.4-1.dll $(OUTDIR)/x86/libpangomm-1.4-1.dll
	cp /c/msys64/mingw32/bin/libpangowin32-1.0-0.dll $(OUTDIR)/x86/libpangowin32-1.0-0.dll
	cp /c/msys64/mingw32/bin/libpcre-1.dll $(OUTDIR)/x86/libpcre-1.dll
	cp /c/msys64/mingw32/bin/libpixman-1-0.dll $(OUTDIR)/x86/libpixman-1-0.dll
	cp /c/msys64/mingw32/bin/libpng16-16.dll $(OUTDIR)/x86/libpng16-16.dll
	cp /c/msys64/mingw32/bin/libsigc-2.0-0.dll $(OUTDIR)/x86/libsigc-2.0-0.dll
	cp /c/msys64/mingw32/bin/libstdc++-6.dll $(OUTDIR)/x86/libstdc++-6.dll
	cp /c/msys64/mingw32/bin/libwinpthread-1.dll $(OUTDIR)/x86/libwinpthread-1.dll
	$(CXXW32) -fPIC -shared -m32 $(shell pkg-config --cflags $(PACKAGES)) $< -o $@ $(shell pkg-config --libs $(PACKAGES)) -Wl,--add-stdcall-alias

linux64: $(OUTDIR)/x64/libPlexNative.so
linux32: $(OUTDIR)/x86/libPlexNative.so
linux: linux32 linux64
windows64: $(OUTDIR)/x64/PlexNative.dll
windows32: $(OUTDIR)/x86/PlexNative.dll
windows: windows32 windows64
all: linux windows

