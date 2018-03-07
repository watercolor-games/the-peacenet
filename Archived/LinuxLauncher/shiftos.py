#!/usr/bin/env python
import os, xdg.BaseDirectory, sys, subprocess, tempfile, distutils.spawn

def GetPrimaryDisplayCurrentResolution():
	xrandr = subprocess.check_output(["xrandr"])
	if not isinstance(xrandr, str):
		xrandr = xrandr.decode()
	elems = xrandr.splitlines()
	item = ""
	res = None
	while not item.startswith("   "):
		item = elems.pop()
	while item.startswith("   "):
		if "*" in item:
			res = item.split("   ")[1]
			break
		item = elems.pop()
	if not res:
		raise OSError("Failed to find the screen resolution.")
	return res

def RunCmd(cmd):
	if os.system(cmd) != 0:
		raise OSError(cmd)

def SetRegistryKeys(dictionary):
	with tempfile.NamedTemporaryFile(suffix = ".reg") as reg:
		regdata = "Windows Registry Editor Version 5.00\r\n"
		for key, val in dictionary.items():
			seg = key.split("\\")
			path = "\\".join(seg[:-1])
			realkey = seg[-1]
			regdata += '\r\n[{0}]\r\n"{1}"="{2}"\r\n'.format(path, realkey, val)
		reg.write(regdata.encode())
		reg.flush()
		RunCmd("{0} regedit /C {1}".format(WinePath, reg.name))

def UpdateSymlinks(src, dest):
	src = os.path.abspath(src)
	dest = os.path.abspath(dest)
	
	# Add new directories and symlinks to the destination folder.
	for subdir, dirs, files in os.walk(src):
		for dirname in dirs:
			destpath = os.path.join(dest, os.path.relpath(subdir, src), dirname)
			if not os.path.isdir(destpath):
				os.makedirs(destpath)
		for fname in files:
			srcpath = os.path.join(subdir, fname)
			destpath = os.path.join(dest, os.path.relpath(subdir, src), fname)
			if not os.path.exists(destpath):
				os.symlink(srcpath, destpath)
	
	# Prune old symlinks from the destination folder.
	for subdir, dirs, files in os.walk(dest):
		for fname in files:
			srcpath = os.path.join(subdir, fname)
			destpath = os.path.join(dest, os.path.relpath(subdir, src), fname)
			if os.path.islink(destpath):
				if os.readlink(destpath).startswith(src):
					if not os.path.exists(srcpath):
						os.remove(destpath)

GamePath = os.path.dirname(os.path.realpath(__file__))

GlobalDataPath = os.path.join(GamePath, "data")

WinePath = distutils.spawn.find_executable("wine64")
if not WinePath:
	WinePath = distutils.spawn.find_executable("wine")
if not WinePath:
	raise FileNotFoundError("Could not find 'wine64' or 'wine'.")

HomePath = os.path.join(xdg.BaseDirectory.xdg_data_home, "ShiftOS")
LocalDataPath = os.path.join(HomePath, "drive_c", "ShiftOS")

if __name__ == "__main__":

	os.environ["WINEPREFIX"] = HomePath

	if not os.path.exists(HomePath):
		RunCmd("wineboot")

	UpdateSymlinks(GlobalDataPath, LocalDataPath)

	os.chdir(LocalDataPath)
	
	SetRegistryKeys(
	{
	r"HKEY_CURRENT_USER\Software\Wine\Explorer\Desktop": "Default",
	r"HKEY_CURRENT_USER\Software\Wine\Explorer\Desktops\Default": GetPrimaryDisplayCurrentResolution()
	})
	
	RunCmd("{0} ShiftOS.WinForms.exe".format(WinePath))
