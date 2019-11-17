import os
import sys
import shutil
import errno
from pathlib import PurePath, Path
import json
import fileinput
import codecs
from distutils.dir_util import copy_tree
import fnmatch

build = {
	# this is unused, it merely explains how build.json is structured
	# since JSON doesn't allow comments.

	# path and dest are relative to the root input and output directories
	"fl_path": "C:/Users/AUDUNVN/AppData/Local/Discovery Freelancer 4.88.1",
	
	# files to copy into the output folder
	"files": [
		{
			"path": "DATA/MISSIONS/mbases.ini",
			"dest": "mbases.ini"
		},
		{
			"path": "DATA/INTERFACE/infocardmap.ini",
			"dest": "infocardmap.ini"
		},
		{
			"path": "DATA/SOLAR/solararch.ini",
			"dest": "solararch.ini"
		},
		{
			"path": "DATA/UNIVERSE/universe.ini",
			"dest": "universe/universe.ini"
		},
		{
			"path": "DATA/UNIVERSE/multiuniverse.ini",
			"dest": "universe/multiuniverse.ini"
		},
		{
			"path": "DATA/UNIVERSE/shortest_illegal_path.ini",
			"dest": "universe/shortest_illegal_path.ini"
		},
		{
			"path": "DATA/UNIVERSE/shortest_legal_path.ini",
			"dest": "universe/shortest_legal_path.ini"
		},
		{
			"path": "DATA/UNIVERSE/systems_shortest_path.ini",
			"dest": "universe/systems_shortest_path.ini"
		},
	],
	# directories to copy into the output folder
	"directories": [
		{
			"path": "IONCROSS",
			"dest": ""
		},
		{
			"path": "DATA/SOLAR/ASTEROIDS",
			"dest": "solar/asteroids"
		},
		{
			"path": "DATA/UNIVERSE/SYSTEMS",
			"dest": "universe/systems"
		},
	]
}

build_input_dir = Path("update")

def get_freelancer_path(search_dir):
	output_path = ""
	for root, dirs, files in os.walk(search_dir):
		for dirname in fnmatch.filter(dirs, "Discovery Freelancer*"):
			output_path = os.path.join(root, dirname)
	return(output_path)

def copy_file(*args, **kwargs):
	dest_path = PurePath(args[1])
	new_path = dest_path.parent / dest_path.name.lower()
	src = args[0]
	shutil.copy2(src, new_path, **kwargs)

def lowercase_rename(dir):
	# renames all subfolders of dir, not including dir itself
	def rename_all(root, items):
		for name in items:
			try:
				os.rename(os.path.join(root, name), os.path.join(root, name.lower()))
			except OSError:
				pass

	# starts from the bottom so paths further up remain valid after renaming
	for root, dirs, files in os.walk(dir, topdown=False):
		rename_all(root, dirs)
		rename_all(root, files)

def copy_into_existing_dir(src, dst):
	if not os.path.exists(dst):
		os.makedirs(dst)
	for item in os.listdir(src):
		s = os.path.join(src, item)
		d = os.path.join(dst, item)
		if os.path.isdir(s):
			copy_into_existing_dir(s, d)
		else:
			if not os.path.exists(d) or os.stat(s).st_mtime - os.stat(d).st_mtime > 1:
				shutil.copy2(s, d)

def copy(src, dest):
	src_path = Path(src)
	if src_path.is_dir():
		try:
			shutil.copytree(src, dest, ignore=shutil.ignore_patterns('BASES', 'BASE_INTERIORS', 'MODELS', '*.txm'), copy_function=copy_file)
		except OSError as e:
			if e.errno == errno.EEXIST:
				copy_into_existing_dir(src, dest)
			else:
				print('Directory not copied. Error: %s' % e)
	elif src_path.is_file():
		copy_file(src, dest)
	else:
		print("Could not copy " + str(src) + " to " + str(dest) + "!")

def format_infocards(out_dir):
	print("Formatting infocards.txt")

	input_name = build_input_dir / "infocards.txt"
	out_name = out_dir / 'infocards.txt'

	with codecs.open(input_name, 'r', encoding='utf-8') as in_file, \
		codecs.open(out_name, 'w', encoding='utf-8') as out_file:
		out_text = ''.join(in_file.readlines()[1:])
		out_text = out_text.replace("\r\n\r\n", "\r\n")
		out_text = out_text.replace("\r\nNAME\r\n", "\r\n")
		out_text = out_text.replace("\r\nINFOCARD\r\n", "\r\n")
		out_file.write(out_text)

def build_update():
	config = build

	with open(build_input_dir / "build.json") as json_file:
		config = json.load(json_file)

	fl_path = Path(config["fl_path"])
	if config["fl_path"] == "":
		print("No FL path set in \"update/build.json\", looking for Discovery install...")
		found_fl_path = get_freelancer_path(os.getenv('LOCALAPPDATA'))
		if found_fl_path == "":
			print("Discovery install directory not found, aborting")
			sys.exit()
		fl_path = Path(found_fl_path)
	print("Freelancer directory set to \"" + str(fl_path) + "\"")

	out_dir = Path(input("Set output directory: "))
	os.mkdir(out_dir)

	print("Run FL Path Generator (in the utils folder) on the FL installation in the \"" + str(fl_path) + "\" folder")
	print("Run FLInfocardIE. Save infocards.txt into the \"" + str(build_input_dir) + "\" folder")
	input("Press Enter to continue once you're done...")

	format_infocards(out_dir)
	
	print("Copying directories")
	for directory in config["directories"]:
		src_path = fl_path / directory["path"]
		out_path = out_dir / directory["dest"]
		print("Copying " + str(src_path) + " to " + str(out_path))
		copy(src_path, out_path)
	
	print("Copying files")
	for file in config["files"]:
		src_path = fl_path / file["path"]
		out_path = out_dir
		if file["dest"] != "":
			out_path = out_dir / file["dest"]
		print("Copying " + str(src_path) + " to " + str(out_path))
		copy(src_path, out_path)

	copy(build_input_dir / "special_systems.txt", out_dir)

	lowercase_rename(out_dir)

	print("Done! Now open index.html and set the dataRootPath variable to the \"" + str(out_dir) + "\" folder")

build_update()
