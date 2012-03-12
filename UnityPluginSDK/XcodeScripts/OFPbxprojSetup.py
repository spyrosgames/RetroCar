#!/usr/bin/env python
# encoding: utf-8
"""
OFPbxProjSetup.py

Most of the documentation is found in Pbxproj.py.

Created by Jeff Verkoeyen on 2010-10-18.
Copyright 2009-2010 Facebook
https://github.com/facebook/three20/blob/master/src/scripts/ttmodule.py

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
"""

import logging
import re
import os
import sys
from optparse import OptionParser

# OpenFeint Python Objects
import Paths
from Pbxproj import Pbxproj

# Print the given project's dependencies to stdout.
def print_dependencies(name):
	pbxproj = Pbxproj.get_pbxproj_by_name(name)
	print str(pbxproj)+"..."
	if pbxproj.dependencies():
		[sys.stdout.write("\t"+x+"\n") for x in pbxproj.dependencies()]

def get_dependency_modules(project, dependency_names):
	dependency_modules = {}
	if not dependency_names:
		return dependency_modules

	for name in dependency_names:
#		project = Pbxproj.get_pbxproj_by_name(name)
		dependency_modules[project.uniqueid()] = project

		dependencies = project.dependencies()
		if dependencies is None:
			print "Failed to get dependencies; it's possible that the given target doesn't exist."
			sys.exit(0)

		submodules = get_dependency_modules(project, dependencies)
		for guid, submodule in submodules.items():
			dependency_modules[guid] = submodule

	return dependency_modules

def add_prefix_to_file(filepath):
	sys.stdout.write('Adding dependency to prefix file '+filepath+'...')
	sys.stdout.flush()

	prefix_file = open(filepath, 'r')
	file_data = prefix_file.read()

	match = re.search("#ifdef __OBJC__((?:.|\n)+?)#endif", file_data)
	if not match:
		sys.stdout.write('   FAILED!\n')
		sys.stdout.flush()
		logging.error("Not found OBJC block")

		return False
	else:
		(objc_block,) = match.groups()
		prefix = "#import <OpenFeint/OFDependencies.h>\n"

		if re.search(re.escape(prefix), objc_block):
			logging.info("Dependency already on prefix file.")
		else:
			file_data = file_data[:match.end()-6]+prefix+"#endif"+file_data[match.end():]
			prefix_file = open(filepath, 'w')
			prefix_file.write(file_data)
	
	sys.stdout.write('   DONE!\n')
	sys.stdout.flush()

	return True

def add_header_to_file(path, app_name):
	filepath = path+"/AppController.mm"
	sys.stdout.write('Adding header to file '+filepath+'...')
	sys.stdout.flush()

	prefix_file = open(filepath, 'r')
	file_data = prefix_file.read()

	match = re.search("#import \"AppController.h\"\n", file_data)
	if not match:
		sys.stdout.write('   FAILED!\n')
		sys.stdout.flush()
		logging.error("Not found AppController.h import")

		return False
	else:
		import_block = match.group(0)
		header = "#import \"AppController+"+app_name+".h\"\n"

		if re.search(re.escape(header), file_data):
			logging.info("Header already on file.")
		else:
			file_data = file_data[:match.end()]+header+file_data[match.end():]
			prefix_file = open(filepath, 'w')
			prefix_file.write(file_data)
	
	sys.stdout.write('   DONE!\n')
	sys.stdout.flush()

	return True


def add_modules_to_project(module_names, project, app_name, configs, add_classes, of_framework, all_sdks):
	logging.info(project)
	logging.info("Checking dependencies...")
	if project.dependencies() is None:
		logging.error("Failed to get dependencies. Check the error logs for more details.")
		sys.exit(0)
	if len(project.dependencies()) == 0:
		logging.info("\tNo dependencies.")
	else:
		logging.info("Existing dependencies:")
		[logging.info("\t"+x) for x in project.dependencies()]

	modules = get_dependency_modules(project, module_names)

	logging.info("Requested dependency list:")
	[logging.info("\t"+str(x)) for k,x in modules.items()]
	
	logging.info("Adding dependencies...")
	failed = []
	for k,v in modules.items():
		#add frameworks
		sys.stdout.write('Adding frameworks...')
		sys.stdout.flush()
		project.add_framework('AddressBook.framework', None, None)
		project.add_framework('AddressBookUI.framework', None, None)
		project.add_framework('CoreGraphics.framework', None, None)
		project.add_framework('CoreText.framework', None, None)
		project.add_framework('GameKit.framework', None, None)
		project.add_framework('MapKit.framework', None, None)
		project.add_framework('MobileCoreServices.framework', None, None)
		project.add_framework('Security.framework', None, None)
		project.add_framework('StoreKit.framework', None, None)
		project.add_framework('CoreTelephony.framework', None, None)
		sys.stdout.write('   DONE!\n')
		sys.stdout.flush()

		#add libraries
		sys.stdout.write('Adding libraries...')
		sys.stdout.flush()
		project.add_library('libsqlite3.0.dylib')
		project.add_library('libz.1.2.5.dylib')		
		sys.stdout.write('   DONE!\n')
		sys.stdout.flush()
			
		#refactoring frameworks and libraries
		sys.stdout.write('Refactoring frameworks and libraries...')
		sys.stdout.flush()
		project.refactor_frameworks()		
		project.refactor_libraries()
		sys.stdout.write('   DONE!\n')
		sys.stdout.flush()

		#add classes or search_paths
		if add_classes:
			sys.stdout.write('Adding classes...\n')
			sys.stdout.flush()
		else:
			sys.stdout.write('Adding search_paths...\n')
			sys.stdout.flush()
				
		#add sdk classes
		sys.stdout.write('SDK and OFUnity Framework:\n')
		sys.stdout.flush()
		sdk_include_list=None
		if not project.add_sdks_classes(v, 'vendor', None, add_classes):
			failed.append(k)	
			sys.stdout.write('   FAILED!\n')
			sys.stdout.flush()
		else:
			sys.stdout.write('   DONE!\n')
			sys.stdout.flush()

		#add customization classes
		sys.stdout.write('Customizations:\n')
		sys.stdout.flush()
		if not project.add_customization_classes(v, 'Customizations', app_name, add_classes):
			failed.append(k)	
			sys.stdout.write('   FAILED!\n')
			sys.stdout.flush()
		else:
			sys.stdout.write('   DONE!\n')
			sys.stdout.flush()


	if configs:
		for config in configs:
			project.add_static_header_search_paths(config)
			project.add_static_framework_search_paths(config)
			project.add_static_library_search_paths(config)

			sys.stdout.write('Adding build settings...')
			sys.stdout.flush()
			project.add_build_setting(config, 'OTHER_LDFLAGS', '-ObjC')
			project.replace_build_setting(config, 'ALWAYS_SEARCH_USER_PATHS', 'NO', 'YES')
			sys.stdout.write('   DONE!\n')
			sys.stdout.flush()
	else:
		for configuration in project.configurations:
			project.add_static_header_search_paths(configuration[1])
			project.add_static_framework_search_paths(configuration[1])
			project.add_static_library_search_paths(configuration[1])

			for k,v in modules.items():
				sys.stdout.write('Adding build settings...')
				sys.stdout.flush()
				project.add_build_setting(configuration[1], 'OTHER_LDFLAGS', '-ObjC')
				project.replace_build_setting(configuration[1], 'ALWAYS_SEARCH_USER_PATHS', 'NO', 'YES')
				sys.stdout.write('   DONE!\n')
				sys.stdout.flush()

	if len(failed) > 0:
		logging.error("Some dependencies failed to be added:")
		[logging.error("\t"+str(x)+"\n") for x in failed]
		sys.stdout.write('Some dependencies failed to be added:\n')
		sys.stdout.flush()
	else:
		sys.stdout.write('Dependencies process completed succesfully!!!\n')
		sys.stdout.flush()

	file_path = os.path.join(project.get_project_classes_folder(),"iPhone_target_Prefix.pch")
	add_prefix_to_file(file_path)

	file_path = project.get_project_classes_folder()
	add_header_to_file(file_path, app_name)

def main():
	usage = '''%prog [options] module(s)

The OpenFeint Module Script.
Easily add OpenFeint modules to your projects.

MODULES:

    Modules may take the form <module-name>(:<module-target>)
    <module-target> defaults to <module-name> if it is not specified
    <module-name> may be a path to a .pbxproj file.

EXAMPLES:

    Most common use case:
    > %prog -p path/to/myApp/myApp.xcodeproj Unity-iPhone -n MyAppName
    
    For adding Xcode 4 support to an Xcode 3.2.# project:
    > %prog -p path/to/myApp/myApp.xcodeproj Unity-iPhone --xcode-version=4
    
    Print all dependencies for the OpenFeint module
    > %prog -d Unity-iPhone
        
    Add the OpenFeint project settings specifically to the Debug and Release configurations.
    By default, all OpenFeint settings are added to all project configurations.
    This includes adding the header search path and linker flags.
    > %prog -p path/to/myApp.xcodeproj -c Debug -c Release'''
	parser = OptionParser(usage = usage)
	
	parser.add_option("-d", "--dependencies", dest="print_dependencies",
	                  help="Print dependencies for the given modules",
	                  action="store_true")
	
	parser.add_option("-v", "--verbose", dest="verbose",
	                  help="Display verbose output",
	                  action="store_true")

	parser.add_option("-p", "--project", dest="projects",
	                  help="Add the given modules to this project", 
	                  action="append")

	parser.add_option("--xcode-version", dest="xcode_version",
	                  help="Set the xcode version you plan to open this project in. By default uses xcodebuild to determine your latest Xcode version.")
	
	parser.add_option("-c", "--config", dest="configs",
	                  help="Explicit configurations to add OpenFeint settings to (example: Debug). By default, ttmodule will add configuration settings to every configuration for the given target", 
	                  action="append")

	parser.add_option("-n", "--appname", dest="appname",
	                  help="Add the given appname to this project", 
	                  action="append")

	parser.add_option("-f", "--frameworks", dest="frameworks",
	                  help="The name of the OF framework to use. If not provided then use the source code.",
	                  action="append")

	parser.add_option("-s", "--search_paths", dest="search_paths",
	                  help="Add frameworks, libraries and search_paths. Do not add classes",
	                  action="store_true")

	parser.add_option("-a", "--all_sdks", dest="all_sdks",
	                  help="Include all the sdks.",
	                  action="store_true")

	parser.add_option("-t", "--target", dest="target",
	                  help="Set the target", 
	                  action="append")
	
	(options, args) = parser.parse_args()

	if options.verbose:
		log_level = logging.INFO
	else:
		log_level = logging.WARNING

	logging.basicConfig(level=log_level)

	did_anything = False

	if options.print_dependencies:
		[print_dependencies(x) for x in args]
		did_anything = True

	if options.projects is not None:
		did_anything = True
		
		if not options.xcode_version:
			f=os.popen("xcodebuild -version")
			xcodebuild_version = f.readlines()[0]
			match = re.search('Xcode ([a-zA-Z0-9.]+)', xcodebuild_version)
			if match:
				(options.xcode_version, ) = match.groups()
		
		for name in options.projects:
			project = Pbxproj.get_pbxproj_by_name(name, xcode_version = options.xcode_version)
			app_name=None
			framework=None
			if options.appname:
				app_name=options.appname[0]
			if options.frameworks:
				framework=options.frameworks[0]
			add_modules_to_project(options.target, project, app_name, options.configs, not options.search_paths, framework, options.all_sdks)

	if not did_anything:
		parser.print_help()


if __name__ == "__main__":
	sys.exit(main())