
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
	$(CXXW64) -fPIC -shared -m64 $(shell pkg-config --cflags $(PACKAGES)) $< -o $@ $(shell pkg-config --libs $(PACKAGES))

$(OUTDIR)/x86/PlexNative.dll: PangoTextRenderer.cpp
	mkdir -p $(OUTDIR)/x86
	$(CXXW32) -fPIC -shared -m32 $(shell pkg-config --cflags $(PACKAGES)) $< -o $@ $(shell pkg-config --libs $(PACKAGES))
	cp /c/msys64/mingw32/bin/libgcc_s_dw2-1.dll $(OUTDIR)/x86/libgcc_s_dw2-1.dll
	cp /c/msys64/mingw32/bin/libcairomm-1.0-1.dll $(OUTDIR)/x86/libcairomm-1.0-1.dll
	cp /c/msys64/mingw32/bin/libglibmm-2.4-1.dll $(OUTDIR)/x86/libglibmm-2.4-1.dll
	cp /c/msys64/mingw32/bin/libpangomm-1.4-1.dll $(OUTDIR)/x86/libpangomm-1.4-1.dll
	cp /c/Windows/System32/vcruntime140.dll $(OUTDIR)/x86/vcruntime140.dll
	cp /c/Windows/System32/msvcp140.dll $(OUTDIR)/x86/msvcp140.dll

linux64: $(OUTDIR)/x64/libPlexNative.so
linux32: $(OUTDIR)/x86/libPlexNative.so
linux: linux32 linux64
windows64: $(OUTDIR)/x64/PlexNative.dll
windows32: $(OUTDIR)/x86/PlexNative.dll
windows: windows32 windows64
all: linux windows

