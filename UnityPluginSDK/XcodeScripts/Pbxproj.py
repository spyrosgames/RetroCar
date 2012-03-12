#!/usr/bin/env python
# encoding: utf-8
"""
Pbxproj.py

Working with the pbxproj file format is a pain in the ass.

This object provides a couple basic features for parsing pbxproj files:

* Getting a dependency list
* Adding one pbxproj to another pbxproj as a dependency

Version 1.2.

History:
1.0 - October 20, 2010: Initial hacked-together version finished. It is alive!
1.1 - January 11, 2011: Add configuration settings to all configurations by default.
1.2 - January 3, 2012: Add OpenFeint dependencies.
1.3 - January 12, 2012: Add referenced classes and groups
1.4 - January 21, 2012: search_paths find automatically

Created by Jeff Verkoeyen on 2010-10-18.
Copyright 2009-2011 Facebook
https://github.com/facebook/three20/blob/master/src/scripts/Pbxproj.py

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

import hashlib
import logging
import os
import re
import sys
import Paths

pbxproj_cache = {}

# The following relative path methods recyled from:
# http://code.activestate.com/recipes/208993-compute-relative-path-from-one-directory-to-anothe/
# Author: Cimarron Taylor
# Date: July 6, 2003
def pathsplit(p, rest=[]):
    (h,t) = os.path.split(p)
    if len(h) < 1: return [t]+rest
    if len(t) < 1: return [h]+rest
    return pathsplit(h,[t]+rest)

def commonpath(l1, l2, common=[]):
    if len(l1) < 1: return (common, l1, l2)
    if len(l2) < 1: return (common, l1, l2)
    if l1[0] != l2[0]: return (common, l1, l2)
    return commonpath(l1[1:], l2[1:], common+[l1[0]])

def relpath(p1, p2):
    (common,l1,l2) = commonpath(pathsplit(p1), pathsplit(p2))
    p = []
    if len(l1) > 0:
        p = [ '../' * len(l1) ]
    p = p + l2
    return os.path.join( *p )

class Pbxproj(object):

	@staticmethod
	def get_pbxproj_by_name(name, xcode_version = None):
		if name not in pbxproj_cache:
			pbxproj_cache[name] = Pbxproj(name, xcode_version = xcode_version)

		return pbxproj_cache[name]

	# /path/to/project.xcodeproj/project.pbxproj
	def __init__(self, name, xcode_version = None):
		self._project_data = None

		parts = name.split(':')
		self.name = parts[0]

		if len(parts) > 1:
			self.target = parts[1]
		else:
			valid_file_chars = '[a-zA-Z0-9\.\-:+ "\'!@#$%^&*\(\)]';
			if re.match('^'+valid_file_chars+'+$', self.name):
				self.target = self.name
			else:
				result = re.search('('+valid_file_chars+'+)\.xcodeproj', self.name)
				if not result:
					self.target = self.name
				else:
					(self.target, ) = result.groups()

		match = re.search('([^/\\\\]+)\.xcodeproj', self.name)
		if not match:
			self._project_name = self.name
			self._directory = 'iOS' #default directory name
		else:
			(self._project_name, ) = match.groups()
			self._project_path = self.name[:match.start()-1]
			match = re.search(re.escape('/'), self._project_path)
			if match:
				self._directory = self._project_path[match.end():]
			else:
				self._directory = 'iOS'
			

		self._guid = None
		self._deps = None
		self._xcode_version = xcode_version
		self._projectVersion = None
		self._product_name = 'Unity-iphone'
		self._exclude_list = []
		self.guid()

	def __str__(self):
		return str(self.name)+" target:"+str(self.target)+" guid:"+str(self._guid)+" prodname: "+self._product_name

	def uniqueid(self):
		return self.name + ':' + self.target

	def path(self):
		# TODO: No sense calculating this every time, just store it when we get the name.
		if re.match('^[a-zA-Z0-9\.\-:+"]+$', self.name):
			return os.path.join(Paths.src_dir, self._directory, self.name.strip('"')+'.xcodeproj', 'project.pbxproj')
		elif not re.match('project.pbxproj$', self.name):
			return os.path.join(self.name, 'project.pbxproj')
		else:
			return self.name

	# A pbxproj file is contained within an xcodeproj file.
	# This method simply strips off the project.pbxproj part of the path.
	def xcodeprojpath(self):
		return os.path.dirname(self.path())

	def guid(self):
		if not self._guid:
			self.dependencies()

		return self._guid

	def version(self):
		if not self._projectVersion:
			self.dependencies()

		return self._projectVersion

	# Load the project data from disk.
	def get_project_data(self):
		if self._project_data is None:
			if not os.path.exists(self.path()):
				logging.info("Couldn't find the project at this path:")
				logging.info(self.path())
				return None

			project_file = open(self.path(), 'r')
			self._project_data = project_file.read()

		return self._project_data

	# Write the project data to disk.
	def set_project_data(self, project_data):
		if self._project_data != project_data:
			self._project_data = project_data
			project_file = open(self.path(), 'w')
			project_file.write(self._project_data)

	def plugin_folder_exist(self, folder_name):
		folder_path = os.path.join(Paths.src_dir,self._directory,"Classes",folder_name)
		return os.path.isdir(folder_path)

	def get_project_classes_folder (self):
		return os.path.join(self._project_path,"Classes")

	def get_parent_path(self, folder_path):
		folder_name = os.path.basename(folder_path)
		parent_path = folder_path[:re.search(folder_name,folder_path).start()-1]

		return parent_path		

	def get_parent_folder(self, folder_path):
		folder_name = os.path.basename(folder_path)
		parent_path = folder_path[:re.search(folder_name,folder_path).start()-1]
		parent_name = os.path.basename(parent_path)

		return parent_name		

	def is_subfolder (self, folder_path, parent_folder_name):
		parent_name = self.get_parent_folder(folder_path)

		return parent_name == parent_folder_name

	def find_folder_in_root(self, rootdir, folder_name):
		for root, dirs, files in os.walk(rootdir):
			for dir_name in dirs:
				if not dir_name.startswith('.'):
					filepath = os.path.join(root,dir_name)
					if filepath.endswith(folder_name):
						filepath = os.path.realpath(filepath)
						return filepath

		return None

	def find_folder_starting_at(self, folder_name, rootdir):
		sys.stdout.write('Searching folder '+folder_name+'...')
		sys.stdout.flush()

		for n in range(1, 5):
			path = self.find_folder_in_root(rootdir, folder_name)
			if path:
				sys.stdout.write('  Found at '+path+'\n')
				sys.stdout.flush()
				return path

			rootdir =  os.path.realpath(os.path.join(rootdir, '..'))

		sys.stdout.write('  Not Found!\n')
		sys.stdout.flush()
		return None


	def get_filetype(self, filename):
		case = {
			'm':'c.objc',
			'mm':'cpp.objcpp',
			'cpp':'cpp.cpp',
			'png':'image',
			'xib':'xib',
			'scpt':'script',
			'a':'ar',
			'framework':'framework',
			'bundle':'plug-in'
			}

		ext = filename[re.search('\.',filename).end():]
		try:
			filetype = case[ext]
		except:
			filetype = 'c.h'
		
		return filetype


	def get_last_known_filetype(self, file_type, file_ext):

		case = {
 			'frameworks': '\"wrapper.'+file_type+'\"',
			'pb-project': '\"wrapper.'+file_type+'\"',
			'plug-in': '\"wrapper.'+file_type+'\"',
			'framework': 'wrapper.'+file_type,
			'image':'image.'+file_ext,
			'xib':'file.'+file_type,
			'script':'file',
			'ar':'archive.'+file_type
			}

		try:
			last_known_file_type = case[file_type]
		except:
			last_known_file_type = 'sourcecode.'+file_type
		
		return last_known_file_type


	# Get and cache the dependencies for this project.
	def dependencies(self):
		if self._deps is not None:
			return self._deps

		project_data = self.get_project_data()
		
		if project_data is None:
			logging.error("Unable to open the project file at this path (is it readable?): "+self.path())
			return None

		# Get project file format version

		result = re.search('\tobjectVersion = ([0-9]+);', project_data)

		if not result:
			logging.error("Can't recover: unable to find the project version for your target at: "+self.path())
			return None
	
		(self._projectVersion,) = result.groups()
		self._projectVersion = int(self._projectVersion)

		# Get configuration list guid

		result = re.search('[A-Z0-9]+ \/\* '+re.escape(self.target)+' \*\/ = {\n[ \t]+isa = PBXNativeTarget;(?:.|\n)+?buildConfigurationList = ([A-Z0-9]+) \/\* Build configuration list for PBXNativeTarget "'+re.escape(self.target)+'" \*\/;',
		                   project_data)

		if result:
			(self.configurationListGuid, ) = result.groups()
		else:
			self.configurationListGuid = None


		# Get configuration list
		
		if self.configurationListGuid:
			match = re.search(re.escape(self.configurationListGuid)+' \/\* Build configuration list for PBXNativeTarget "'+re.escape(self.target)+'" \*\/ = \{\n[ \t]+isa = XCConfigurationList;\n[ \t]+buildConfigurations = \(\n((?:.|\n)+?)\);', project_data)
			if not match:
				logging.error("Couldn't find the configuration list.")
				return False

			(configurationList,) = match.groups()
			self.configurations = re.findall('[ \t]+([A-Z0-9]+) \/\* (.+) \*\/,\n', configurationList)

		# Get build phases

		result = re.search('([A-Z0-9]+) \/\* '+re.escape(self.target)+' \*\/ = {\n[ \t]+isa = PBXNativeTarget;(?:.|\n)+?buildPhases = \(\n((?:.|\n)+?)\);',
		                   project_data)
	
		if not result:
			logging.error("Can't recover: Unable to find the build phases from your target at: "+self.path())
			return None
	
		(self._guid, buildPhases, ) = result.groups()

		# Get the build phases we care about.

		match = re.search('([A-Z0-9]+) \/\* Resources \*\/', buildPhases)
		if match:
			(self._resources_guid, ) = match.groups()
		else:
			self._resources_guid = None
		
		match = re.search('([A-Z0-9]+) \/\* Frameworks \*\/', buildPhases)
		if not match:
			logging.error("Couldn't find the Frameworks phase from: "+self.path())
			logging.error("Please add a New Link Binary With Libraries Build Phase to your target")
			logging.error("Right click your target in the project, Add, New Build Phase,")
			logging.error("  \"New Link Binary With Libraries Build Phase\"")
			return None

		(self._frameworks_guid, ) = match.groups()


		match = re.search('([A-Z0-9]+) \/\* Sources \*\/', buildPhases)
		if not match:
			logging.error("Couldn't find the Sources phase from: "+self.path())
			logging.error("Please add a New Link Binary With Libraries Build Phase to your target")
			logging.error("Right click your target in the project, Add, New Build Phase,")
			logging.error("  \"New Link Binary With Libraries Build Phase\"")
			return None

		(self._sources_guid, ) = match.groups()

		# Get the dependencies

		result = re.search(re.escape(self._guid)+' \/\* '+re.escape(self.target)+' \*\/ = {\n[ \t]+isa = PBXNativeTarget;(?:.|\n)+?dependencies = \(\n((?:[ \t]+[A-Z0-9]+ \/\* PBXTargetDependency \*\/,\n)*)[ \t]*\);\n',
		                   project_data)
	
		if not result:
			logging.error("Unable to get dependencies from: "+self.path())
			return None
	
		(dependency_set, ) = result.groups()
		dependency_guids = re.findall('[ \t]+([A-Z0-9]+) \/\* PBXTargetDependency \*\/,\n', dependency_set)

		# Parse the dependencies

		dependency_names = []

		for guid in dependency_guids:
			result = re.search(guid+' \/\* PBXTargetDependency \*\/ = \{\n[ \t]+isa = PBXTargetDependency;\n[ \t]*name = (["a-zA-Z0-9\.\-]+);',
			                   project_data)
		
			if result:
				(dependency_name, ) = result.groups()
				dependency_names.append(dependency_name)

		self._deps = dependency_names


		# Get the product guid and name.

		result = re.search(re.escape(self._guid)+' \/\* '+re.escape(self.target)+' \*\/ = {\n[ \t]+isa = PBXNativeTarget;(?:.|\n)+?productReference = ([A-Z0-9]+) \/\* (.+?) \*\/;',
		                   project_data)
	
		if not result:
			logging.error("Unable to get product guid from: "+self.path())
			return None
	
		(self._product_guid, self._product_name, ) = result.groups()

		return self._deps

	# Add a line to the PBXBuildFile section.
	#
	# <default_guid> /* <name> in Frameworks */ = {isa = PBXBuildFile; fileRef = <file_ref_hash> /* <name> */; };
	#
	# Returns: <default_guid> if a line was added.
	#          Otherwise, the existing guid is returned.
	def add_framework_buildfile(self, name, file_ref_hash, default_guid):
		project_data = self.get_project_data()

		match = re.search('\/\* Begin PBXBuildFile section \*\/\n((?:.|\n)+?)\/\* End PBXBuildFile section \*\/', project_data)

		if not match:
			logging.error("Couldn't find PBXBuildFile section.")
			return None

		(subtext, ) = match.groups()

		buildfile_hash = None
		
		match = re.search('([A-Z0-9]+).+?fileRef = '+re.escape(file_ref_hash), subtext)
		if match:
			(buildfile_hash, ) = match.groups()
			logging.info("This build file already exists: "+buildfile_hash)
		
		if buildfile_hash is None:
			match = re.search('\/\* Begin PBXBuildFile section \*\/\n', project_data)

			buildfile_hash = default_guid
		
			libfiletext = "\t\t"+buildfile_hash+" /* "+name+" in Frameworks */ = {isa = PBXBuildFile; fileRef = "+file_ref_hash+" /* "+name+" */; };\n"
			project_data = project_data[:match.end()] + libfiletext + project_data[match.end():]
		
		self.set_project_data(project_data)
		
		return buildfile_hash


	# Add a line to the PBXBuildFile section.
	#
	# <default_guid> /* <name> in Sources */ = {isa = PBXBuildFile; fileRef = <file_ref_hash> /* <name> */; };
	#
	# Returns: <default_guid> if a line was added.
	#          Otherwise, the existing guid is returned.
	def add_source_buildfile(self, name, file_ref_hash, default_guid):
		project_data = self.get_project_data()

		match = re.search('\/\* Begin PBXBuildFile section \*\/\n((?:.|\n)+?)\/\* End PBXBuildFile section \*\/', project_data)

		if not match:
			logging.error("Couldn't find PBXBuildFile section.")
			return None

		(subtext, ) = match.groups()

		buildfile_hash = None
		
		match = re.search('([A-Z0-9]+).+?fileRef = '+re.escape(file_ref_hash), subtext)
		if match:
			(buildfile_hash, ) = match.groups()
			logging.info("This build file already exists: "+buildfile_hash)
			#print "This build file already exists: "+buildfile_hash
		
		if buildfile_hash is None:
			match = re.search('\/\* Begin PBXBuildFile section \*\/\n', project_data)

			buildfile_hash = default_guid

			if name[-2:] != '.h':
				libfiletext = "\t\t"+buildfile_hash+" /* "+name+" in Sources */ = {isa = PBXBuildFile; fileRef = "+file_ref_hash+" /* "+name+" */; };\n"
				project_data = project_data[:match.end()] + libfiletext + project_data[match.end():]
			#print "ADD_SOURCE_BUILDFILE:"+name+","+file_ref_hash
            
		
		self.set_project_data(project_data)
        
		return buildfile_hash


	# Add a line to the PBXBuildFile section.
	#
	# <default_guid> /* <name> in Resources */ = {isa = PBXBuildFile; fileRef = <file_ref_hash> /* <name> */; };
	#
	# Returns: <default_guid> if a line was added.
	#          Otherwise, the existing guid is returned.
	def add_resource_buildfile(self, name, file_ref_hash, default_guid):
		project_data = self.get_project_data()

		match = re.search('\/\* Begin PBXBuildFile section \*\/\n((?:.|\n)+?)\/\* End PBXBuildFile section \*\/', project_data)

		if not match:
			logging.error("Couldn't find PBXBuildFile section.")
			return None

		(subtext, ) = match.groups()

		buildfile_hash = None
		
		match = re.search('([A-Z0-9]+).+?fileRef = '+re.escape(file_ref_hash), subtext)
		if match:
			(buildfile_hash, ) = match.groups()
			logging.info("This build file already exists: "+buildfile_hash)
			#print "This build file already exists: "+buildfile_hash
		
		if buildfile_hash is None:
			match = re.search('\/\* Begin PBXBuildFile section \*\/\n', project_data)

			buildfile_hash = default_guid
		
			libfiletext = "\t\t"+buildfile_hash+" /* "+name+" in Resources */ = {isa = PBXBuildFile; fileRef = "+file_ref_hash+" /* "+name+" */; };\n"
			project_data = project_data[:match.end()] + libfiletext + project_data[match.end():]
			#print "ADD_RESOURCES_BUILDFILE:"+name+","+file_ref_hash
            
		
		self.set_project_data(project_data)
        
		return buildfile_hash

	# Add a line to the PBXFileReference section.
	#
	# <default_guid> /* <name> */ = {isa = PBXFileReference; lastKnownFileType = "wrapper.<file_type>"; name = <name>; path = <rel_path>; sourceTree = <source_tree>; };
	#
	# Returns: <default_guid> if a line was added.
	#          Otherwise, the existing guid is returned.
	def add_filereference(self, name, file_type, default_guid, rel_path, source_tree):
		project_data = self.get_project_data()

		match = re.search('\+', rel_path)
		if (match):
			rel_path = '"'+rel_path.strip('"')+'"'

		fileref_hash = None
		file_ext = name[re.search('\.',name).end():]

		last_known_file_type = self.get_last_known_filetype(file_type, file_ext)

		# build the fileref property to search for its existence
		search_ref = '([A-Z0-9]+) \/\* '+re.escape(name)+' \*\/ = \{isa = PBXFileReference; '
		# sourcecode types add a fileEncoding property
		if last_known_file_type.startswith('sourcecode'):
			search_ref = search_ref+'fileEncoding = 4; '
		search_ref = search_ref+'lastKnownFileType = '+re.escape(last_known_file_type)+'; path = '+re.escape(rel_path)+';'
		match = re.search(search_ref, project_data)

		if match:
			logging.info("This file has already been added.")
			(fileref_hash, ) = match.groups()
			
		else:
			match = re.search('\/\* Begin PBXFileReference section \*\/\n', project_data)

			if not match:
				logging.error("Couldn't find the PBXFileReference section.")
				return False

			fileref_hash = default_guid	
			file_ext = name[re.search('\.',name).end():]

			last_known_file_type = self.get_last_known_filetype(file_type, file_ext)

			# build the fileref property
			pbxfileref = "\t\t"+fileref_hash+" /* "+name+" */ = {isa = PBXFileReference; " 
			# sourcecode types add a fileEncoding property
			if last_known_file_type.startswith('sourcecode'):
				pbxfileref = pbxfileref+"fileEncoding = 4; "
			pbxfileref = pbxfileref+"lastKnownFileType = "+last_known_file_type+"; path = "+rel_path+"; "
			# files with rel_path need to add the 'name' property to avoid showing the full path as name
			if rel_path.strip('"') != name:
				pbxfileref = pbxfileref+"name = "+name+"; "
			pbxfileref = pbxfileref+"sourceTree = "+source_tree+"; };\n"	

			# add the file reference into the project data and save
			project_data = project_data[:match.end()] + pbxfileref + project_data[match.end():]
			self.set_project_data(project_data)

		return fileref_hash

	# Add a file to the given PBXGroup.
	#
	# <guid> /* <name> */,
	def add_file_to_group(self, name, guid, group, group_hash):
		project_data = self.get_project_data()

		if group_hash:
			match = re.search(re.escape(group_hash)+' \/\* '+re.escape(group)+' \*\/ = \{\n[ \t]+isa = PBXGroup;\n[ \t]+children = \(\n((?:.|\n)+?)\);', project_data)
		else:
			match = re.search(' \/\* '+re.escape(group)+' \*\/ = \{\n[ \t]+isa = PBXGroup;\n[ \t]+children = \(\n((?:.|\n)+?)\);', project_data)
		
		if not match:
			logging.error("Couldn't find the group: "+group+" for file: "+name)
			return False

		(children,) = match.groups()
		match = re.search('\/\* '+re.escape(name)+' \*\/', children)
		if match:
			logging.info("The file: "+name+" is already a member of group "+group)
		else:
			if group_hash:
				match = re.search(re.escape(group_hash)+' \/\* '+re.escape(group)+' \*\/ = \{\n[ \t]+isa = PBXGroup;\n[ \t]+children = \(\n', project_data)
			else:
				match = re.search(' \/\* '+re.escape(group)+' \*\/ = \{\n[ \t]+isa = PBXGroup;\n[ \t]+children = \(\n', project_data)
			
			pbxgroup = "\t\t\t\t"+guid+" /* "+name+" */,\n"
			project_data = project_data[:match.end()] + pbxgroup + project_data[match.end():]

		self.set_project_data(project_data)

		return True

	# Add a file to the Libraries PBXGroup.
	#
	# <guid> /* <name> */,
	def add_file_to_libraries(self, name, guid):
		return self.add_file_to_group(name, guid, 'Libraries',None)

	# Add a file to the Frameworks PBXGroup.
	#
	# <guid> /* <name> */,
	def add_file_to_frameworks(self, name, guid):
		return self.add_file_to_group(name, guid, 'Frameworks',None)

	# Add a file to the Resources PBXGroup.
	#
	# <guid> /* <name> */,
	def add_file_to_resources(self, name, guid):
		match = re.search('\/\* '+re.escape('Resources')+' \*\/ = \{\n[ \t]+isa = PBXGroup;\n[ \t]+children = \(\n((?:.|\n)+?)\);', self.get_project_data())
		if not match:
			return self.add_file_to_group(name, guid, 'Supporting Files',None)

		return self.add_file_to_group(name, guid, 'Resources',None)

	def add_file_to_phase(self, name, guid, phase_guid, phase):
		project_data = self.get_project_data()

		match = re.search(re.escape(phase_guid)+" \/\* "+re.escape(phase)+" \*\/ = {(?:.|\n)+?files = \(((?:.|\n)+?)\);", project_data)

		if not match:
			logging.error("Couldn't find the "+phase+" phase.")
			"Couldn't find the "+phase+" files"
			return False

		(files, ) = match.groups()

		match = re.search(re.escape(guid), files)
		if match:
			logging.info("The file has already been added.")
			#print "The file has already been added."
		else:
			match = re.search(re.escape(phase_guid)+" \/\* "+phase+" \*\/ = {(?:.|\n)+?files = \(\n", project_data)
			if not match:
				logging.error("Couldn't find the "+phase+" files")
				#print "Couldn't find the "+phase+" files"
				return False

			frameworktext = "\t\t\t\t"+guid+" /* "+name+" in "+phase+" */,\n"
			project_data = project_data[:match.end()] + frameworktext + project_data[match.end():]

		self.set_project_data(project_data)
		

		return True

	def get_rel_path_to_products_dir(self):
		project_path = os.path.dirname(os.path.abspath(self.xcodeprojpath()))
		build_path = os.path.join(os.path.join(os.path.dirname(Paths.src_dir), 'Build'), 'Products')
		return relpath(project_path, build_path)


	def add_file_to_frameworks_phase(self, name, guid):
		return self.add_file_to_phase(name, guid, self._frameworks_guid, 'Frameworks')


	def add_file_to_resources_phase(self, name, guid):
		if self._resources_guid is None:
			logging.error("No resources build phase found in the destination project")
			logging.error("Please add a New Copy Bundle Resources Build Phase to your target")
			logging.error("Right click your target in the project, Add, New Build Phase,")
			logging.error("  \"New Copy Bundle Resources Build Phase\"")
			return False

		return self.add_file_to_phase(name, guid, self._resources_guid, 'Resources')


	def add_file_to_source_phase(self, name, guid):
		return self.add_file_to_phase(name, guid, self._sources_guid, 'Sources')


	def add_header_search_path(self, configuration, search_path):
		if not configuration:
			for config in self.configurations:
				if not self.add_build_setting(config[1], 'HEADER_SEARCH_PATHS', search_path):
					return False
			
			return True
		else:
			return self.add_build_setting(configuration, 'HEADER_SEARCH_PATHS', search_path)


	def add_static_header_search_paths(self, configuration):
		return self.add_header_search_path(configuration, '"$(inherited)"')
		

	def add_framework_search_path(self, configuration, search_path):
		if not configuration:
			for config in self.configurations:
				if not self.add_build_setting(config[1], 'FRAMEWORK_SEARCH_PATHS', search_path):
					return False
			
			return True
		else:
			return self.add_build_setting(configuration, 'FRAMEWORK_SEARCH_PATHS', search_path)


	def add_static_framework_search_paths(self, configuration):
		return self.add_framework_search_path(configuration, '"$(inherited)"')


	def add_library_search_path(self, configuration, search_path):
		if not configuration:
			for config in self.configurations:
				if not self.add_build_setting(config[1], 'LIBRARY_SEARCH_PATHS', search_path):
					return False
			
			return True
		else:
			return self.add_build_setting(configuration, 'LIBRARY_SEARCH_PATHS', search_path)
		

	def add_static_library_search_paths(self, configuration):
		return self.add_library_search_path(configuration, '"$(inherited)"')
		

	def get_search_path_for_filedir(self, filedir):
		return '"\\\"$(SRCROOT)/'+os.path.relpath(filedir, self._project_path)+'\\\""'


	def replace_build_setting(self, configuration, setting_name, oldvalue, newvalue):
		project_data = self.get_project_data()

		match = re.search('\/\* '+configuration+' \*\/ = {\n[ \t]+isa = XCBuildConfiguration;\n(?:.|\n)+?[ \t]+buildSettings = \{\n((?:.|\n)+?)\};', project_data)
		if not match:
			#print "Couldn't find the "+configuration+" configuration in "+self.path()
			return False

		settings_start = match.start(1)
		settings_end = match.end(1)

		(build_settings, ) = match.groups()

		match = re.search(re.escape(setting_name)+' = ((?:.|\n)+?);', build_settings)

		if not match:
			#print "Couldn't find the setting: "+setting_name
			return False
		else:
			# Build settings already exist.
			(search_paths,) = match.groups()
			rmatch = re.search(re.escape(oldvalue),search_paths)
			if not rmatch:
				#print "Couldn't find value: "+oldvalue
				return False
			else:
				existing_path = rmatch
				build_settings = build_settings[:match.start(1)] + newvalue + build_settings[match.end(1):]
				project_data = project_data[:settings_start] + build_settings + project_data[settings_end:]

		self.set_project_data(project_data)

		return True


	def add_build_setting(self, configuration, setting_name, value):
		project_data = self.get_project_data()

		match = re.search('\/\* '+configuration+' \*\/ = {\n[ \t]+isa = XCBuildConfiguration;\n(?:.|\n)+?[ \t]+buildSettings = \{\n((?:.|\n)+?)\};', project_data)
		if not match:
			return False

		settings_start = match.start(1)
		settings_end = match.end(1)

		(build_settings, ) = match.groups()

		match = re.search(re.escape(setting_name)+' = ((?:.|\n)+?);', build_settings)

		if not match:
			# Add a brand new build setting. No checking for existing settings necessary.
			settingtext = '\t\t\t\t'+setting_name+' = '+value+';\n'

			project_data = project_data[:settings_start] + settingtext + project_data[settings_start:]
		else:
			# Build settings already exist. Is there one or many?
			(search_paths,) = match.groups()
			if re.search('\(\n', search_paths):
				# Many
				match = re.search(re.escape(value), search_paths)
				if not match:
					# If value has any spaces in it, Xcode will split it up into
					# multiple entries.
					escaped_value = re.escape(value).replace(' ', '",\n[ \t]+"')
					match = re.search(escaped_value, search_paths)
					if not match and not re.search(re.escape(value.strip('"')), search_paths):
						match = re.search(re.escape(setting_name)+' = \(\n', build_settings)

						build_settings = build_settings[:match.end()] + '\t\t\t\t\t'+value+',\n' + build_settings[match.end():]
						project_data = project_data[:settings_start] + build_settings + project_data[settings_end:]
			else:
				# One
				if search_paths.strip('"') != value.strip('"'):
					existing_path = search_paths
					path_set = '(\n\t\t\t\t\t'+value+',\n\t\t\t\t\t'+existing_path+'\n\t\t\t\t)'
					build_settings = build_settings[:match.start(1)] + path_set + build_settings[match.end(1):]
					project_data = project_data[:settings_start] + build_settings + project_data[settings_end:]

		self.set_project_data(project_data)

		return True

	def get_hash_base(self, uniquename):
		examplehash = '0FFFFEEEDDDCCCBBBAAA0000'
		uniquehash = hashlib.sha224(uniquename).hexdigest().upper()
		uniquehash = uniquehash[:len(examplehash) - 3]
		return '0F'+uniquehash

	def add_framework(self, framework, group_name, rel_path):
		tthash_base = self.get_hash_base(framework)
		
		if not group_name:
			fileref_hash = self.add_filereference(framework, 'frameworks', tthash_base+'0', 'System/Library/Frameworks/'+framework, 'SDKROOT')
		else:
			filetype = self.get_filetype(framework)
			fileref_hash = self.add_filereference(framework, filetype, tthash_base+'0', rel_path, '\"<group>\"')


		libfile_hash = self.add_framework_buildfile(framework, fileref_hash, tthash_base+'1')

		if not group_name:
			if not self.add_file_to_frameworks(framework, fileref_hash):
				return False
		else:
			if not self.add_file_to_group(framework, fileref_hash, group_name,None):
				return False

		
		if not self.add_file_to_frameworks_phase(framework, libfile_hash):
			return False
		
		return True

	def add_library(self, library):
		tthash_base = self.get_hash_base(library)
		
		fileref_hash = self.add_filereference(library, 'frameworks', tthash_base+'0', 'usr/lib/'+library, 'SDKROOT')
		libfile_hash = self.add_framework_buildfile(library, fileref_hash, tthash_base+'1')
		if not self.add_file_to_libraries(library, fileref_hash):
			return False
		
		if not self.add_file_to_frameworks_phase(library, libfile_hash):
			return False
		
		return True

	def add_group(self, group, group_hash, path):
		project_data = self.get_project_data()
		existing_group_hash = None

		match = re.search('\/\* End PBXGroup section \*\/\n', project_data)
		if not match:
			logging.error("Couldn't find the group section.")
			return None
		
		group_start = match.start()

		# create the new group
		existing_group_hash = group_hash
		if path == group:
			productgrouptext = "\t\t"+existing_group_hash+" /* "+group+" */ = {\n\t\t\tisa = PBXGroup;\n\t\t\tchildren = (\n\t\t\t\t);\n\t\t\tpath = "+path+";\n\t\t\tsourceTree = \"<group>\";\n\t\t};\n"
		else:
			productgrouptext = "\t\t"+existing_group_hash+" /* "+group+" */ = {\n\t\t\tisa = PBXGroup;\n\t\t\tchildren = (\n\t\t\t\t);"+"\n\t\t\tname = "+group+";\n\t\t\tpath = "+path+";\n\t\t\tsourceTree = \"<group>\";\n\t\t};\n"
		
		project_data = project_data[:group_start] + productgrouptext + project_data[group_start:]

		self.set_project_data(project_data)

		return existing_group_hash
	

	def refactor_frameworks (self):
		project_data = self.get_project_data()

		# find the source group
		match = re.search(' \/\* CustomTemplate \*\/ = \{\n[ \t]+isa = PBXGroup;\n[ \t]+children = \(\n((?:.|\n)+?)\);', project_data)
		if not match:
			logging.error("Couldn't find the CustomTemplate children.")
			return False

		#find the file inside the group
		(children,) = match.groups()

		lines = re.split('\n',children)

		for line in lines:
			if re.search('\.framework',line):
				# remove the file from the CustomTemplate group
				match = re.search(re.escape(line)+"\n",project_data)
				project_data = project_data[:match.start()]+project_data[match.end():]
				self.set_project_data(project_data)

				# add the framework to the group
				match = re.search('\/\* ((?:.)+?) \*\/', line)
				framework = line[match.start()+3:match.end()-3]
				guid = line[4:match.start()-1]
				self.add_file_to_frameworks(framework, guid)

				# get updated project_data
				project_data = self.get_project_data()

		return True


	def refactor_libraries (self):
		project_data = self.get_project_data()

		# find the source group
		match = re.search(' \/\* CustomTemplate \*\/ = \{\n[ \t]+isa = PBXGroup;\n[ \t]+children = \(\n((?:.|\n)+?)\);', project_data)
		if not match:
			logging.error("Couldn't find the CustomTemplate children.")
			return False

		#find the file inside the group
		(children,) = match.groups()

		lines = re.split('\n',children)

		for line in lines:
			if re.search('\.dylib|\.a',line):
				# remove the file from the CustomTemplate group
				match = re.search(re.escape(line)+"\n",project_data)
				project_data = project_data[:match.start()]+project_data[match.end():]
				self.set_project_data(project_data)

				# add the library to the group
				match = re.search('\/\* ((?:.)+?) \*\/', line)
				library = line[match.start()+3:match.end()-3]
				guid = line[4:match.start()-1]
				self.add_file_to_libraries(library, guid)

				# get updated project_data
				project_data = self.get_project_data()

		return True


	# Get the PBXFileReference from the given PBXBuildFile guid.
	def get_filerefguid_from_buildfileguid(self, buildfileguid):
		project_data = self.get_project_data()
		match = re.search(buildfileguid+' \/\* .+ \*\/ = {isa = PBXBuildFile; fileRef = ([A-Z0-9]+) \/\* .+ \*\/;', project_data)

		if not match:
			logging.error("Couldn't find PBXBuildFile row.")
			return None

		(filerefguid, ) = match.groups()
		
		return filerefguid

	def get_filepath_from_filerefguid(self, filerefguid):
		project_data = self.get_project_data()
		match = re.search(filerefguid+' \/\* .+ \*\/ = {isa = PBXFileReference; .+ path = (.+); .+ };', project_data)

		if not match:
			logging.error("Couldn't find PBXFileReference row.")
			return None

		(path, ) = match.groups()
		
		return path


	# Get all source files that are "built" in this project. This includes files built for
	# libraries, executables, and unit testing.
	def get_built_sources(self):
		project_data = self.get_project_data()
		match = re.search('\/\* Begin PBXSourcesBuildPhase section \*\/\n((?:.|\n)+?)\/\* End PBXSourcesBuildPhase section \*\/', project_data)

		if not match:
			logging.error("Couldn't find PBXSourcesBuildPhase section.")
			return None
		
		(buildphasedata, ) = match.groups()
		
		buildfileguids = re.findall('[ \t]+([A-Z0-9]+) \/\* .+ \*\/,\n', buildphasedata)
		
		project_path = os.path.dirname(os.path.abspath(self.xcodeprojpath()))
		
		filenames = []
		
		for buildfileguid in buildfileguids:
			filerefguid = self.get_filerefguid_from_buildfileguid(buildfileguid)
			filepath = self.get_filepath_from_filerefguid(filerefguid)

			filenames.append(os.path.join(project_path, filepath.strip('"')))

		return filenames


	# Get all header files that are "built" in this project. This includes files built for
	# libraries, executables, and unit testing.
	def get_built_headers(self):
		project_data = self.get_project_data()
		match = re.search('\/\* Begin PBXHeadersBuildPhase section \*\/\n((?:.|\n)+?)\/\* End PBXHeadersBuildPhase section \*\/', project_data)

		if not match:
			logging.error("Couldn't find PBXHeadersBuildPhase section.")
			return None
		
		(buildphasedata, ) = match.groups()

		buildfileguids = re.findall('[ \t]+([A-Z0-9]+) \/\* .+ \*\/,\n', buildphasedata)
		
		project_path = os.path.dirname(os.path.abspath(self.xcodeprojpath()))
		
		filenames = []
		
		for buildfileguid in buildfileguids:
			filerefguid = self.get_filerefguid_from_buildfileguid(buildfileguid)
			filepath = self.get_filepath_from_filerefguid(filerefguid)
			
			filenames.append(os.path.join(project_path, filepath.strip('"')))

		return filenames


	def exclude(self, root):
		for name in self._exclude_list:
			match = re.search(name,root)
			if match:
				return True

		return False


	def add_exclude(self, exclude_path):
		#check the exclude_path is not already on the list and add it
		if not self.exclude(exclude_path):
			self._exclude_list.append(exclude_path)
			print "Excluding: "+exclude_path


	def add_file_phase(self, root, filename, group_name, group_hash):
		filepath = os.path.join(root,filename)
		if self.exclude(root) or self.exclude(filepath):
			return

		tthash_base = self.get_hash_base(filepath)
		filetype = self.get_filetype(filename)

		# Add File Reference
		fileref_hash = self.add_filereference(filename, filetype, tthash_base+'0', filename, '\"<group>\"')

		# Add target file
		if (filetype == 'image' or filetype == 'xib' or filetype == 'plug-in' or filetype == 'script'):
			libfile_hash = self.add_resource_buildfile(filename, fileref_hash, tthash_base+'5')
		else:
			libfile_hash = self.add_source_buildfile(filename, fileref_hash, tthash_base+'5')

		# Add sources
		if (filetype == 'image' or filetype == 'xib' or filetype == 'plug-in' or filetype == 'script'):
			self.add_file_to_resources_phase(filename, libfile_hash)
		else:
			self.add_file_to_source_phase(filename, libfile_hash)

		# Add file to group
		self.add_file_to_group(filename, fileref_hash, group_name, group_hash)

		return fileref_hash


	def add_group_phase(self, root, dirname, group_name, prev_group_hash, reference):			
		filepath = os.path.join(root,dirname)
		if self.exclude(filepath):
			return

		tthash_base = self.get_hash_base(filepath)

		# Add group
		group_hash = self.add_group(dirname,tthash_base+'4', reference)
		self.add_file_to_group(dirname, group_hash, group_name, prev_group_hash)

		return group_hash


	def add_classes(self, rootdir, sdk_folder_name, add_files):
		for root, dirs, files in os.walk(rootdir):
			group_name = os.path.basename(root)
			group_hash = self.get_hash_base(root)+'4'
			match = re.search('.bundle|.framework',root)
			if self.exclude(root):
				continue

			if not match:
				###############################################
				# process files
				###############################################
				for filename in files:
					sys.stdout.write('.')
					sys.stdout.flush()
			
					filepath = os.path.join(root,filename)
					if self.exclude(filepath):
						continue

					# if library add as a framework and to library search paths
					if filename.endswith('.a'):
						if add_files:
							self.add_framework(filename, group_name, filename)
						
						search_path = self.get_search_path_for_filedir(root)
						self.add_library_search_path(None, search_path)
					
					# ommit hidden files
					elif not filename.startswith('.'):
						if add_files:
							self.add_file_phase(root, filename, group_name, group_hash)

				###############################################
				# process directories
				###############################################
				for dirname in dirs:	
					dirpath = os.path.join(root,dirname)

					if self.exclude(dirpath):
						continue

					if dirname.endswith('.framework'):	
						if add_files:
							self.add_framework(dirname, group_name, dirname)

						# add framework search path
						search_path = self.get_search_path_for_filedir(root)
						self.add_framework_search_path(None, search_path)
						# add header_search_paths
						search_path = self.get_search_path_for_filedir(dirpath+"/Headers")
						self.add_header_search_path(None, search_path)
					elif not dirname.endswith('.bundle'):
						if add_files:
							self.add_group_phase(root, dirname, group_name, group_hash, dirname)

						# all sdk subfolders should be on the header_search_path
						if sdk_folder_name and self.is_subfolder(dirpath, sdk_folder_name):
							search_path = self.get_search_path_for_filedir(dirpath)
							self.add_header_search_path(None, search_path)

						
			else:
				# handle frameworks and bundles
				tthash_base = self.get_hash_base(root)
				previous_root = root[:re.search(group_name,root).start()-1]
				previous_group = os.path.basename(previous_root)
				if root.endswith('.framework'):
					if add_files:
						self.add_framework(group_name, previous_group, group_name)

					# add framework search paths
					search_path = self.get_search_path_for_filedir(previous_root)
					self.add_framework_search_path(None, search_path)
					# add header search paths
					search_path = self.get_search_path_for_filedir(root+"/Headers")
					self.add_header_search_path(None, search_path)
				elif root.endswith('.bundle') and not previous_group.endswith('.bundle'):
					if add_files:
						self.add_file_phase(root, group_name, previous_group, self.get_hash_base(previous_root)+'4')		


	def add_sdks_classes(self, dep, sdk_folder_name, include_list, add_classes):
		project_data = self.get_project_data()
		dep_data = dep.get_project_data()

		if project_data is None or dep_data is None:
			return False

		rootdir = self.find_folder_starting_at(sdk_folder_name, self._project_path)
		if not rootdir:
			return False

		#add all exclusions
		for root, dirs, files in os.walk(rootdir):
			for dirname in dirs:
				dirpath = os.path.join(root,dirname)
				exclude = True
				if not include_list:
					exclude = False
				else:
					for include_folder in include_list:
						match=re.search(include_folder, dirpath)
						if match:
							exclude = False
				if exclude:
					self.add_exclude(dirpath+"$")

		if add_classes:
			sys.stdout.write('Adding classes from: '+rootdir)
		else:
			sys.stdout.write('Adding search_paths from: '+rootdir)
		sys.stdout.flush()

		# create a new group sdk and add all the sdk subfolders on it
		root_group_name = 'CustomTemplate'
		group_name = os.path.basename(rootdir)
		reference = os.path.relpath(rootdir, self._project_path)		

		if add_classes:
			tthash_base = self.get_hash_base(rootdir)
			group_hash = self.add_group(group_name, tthash_base+'4', reference)
			self.add_file_to_group(group_name, group_hash, root_group_name, None)

		# sdk folder should be added into the frameworks search path
		search_path = self.get_search_path_for_filedir(rootdir)
		self.add_framework_search_path(None, search_path)

		# add all sdk classes
		self.add_classes(rootdir, os.path.basename(rootdir), add_classes)
		
		return True					

	
	def add_ofunity_framework(self, dep, build_folder_name, framework_name, add_classes):
		project_data = self.get_project_data()
		dep_data = dep.get_project_data()

		if project_data is None or dep_data is None:
			return False

		if build_folder_name:
			build_path = self.find_folder_starting_at(build_folder_name, self._project_path)
			if not build_path:
				return False		
			rootdir = os.path.join(build_path, framework_name)
		else:
			rootdir = self.find_folder_starting_at(framework_name, self._project_path)
			if not rootdir:
				return False		

		#make sure the framework path exists
		if os.path.isdir(rootdir):
			if add_classes:
				sys.stdout.write('Adding framework from: '+rootdir)
				sys.stdout.flush()
				self.add_framework(framework_name, 'Frameworks', os.path.relpath(rootdir, self._project_path))
			else:
				sys.stdout.write('Adding header_search_paths from: '+rootdir)
				sys.stdout.flush()
		
			# add framework search paths
			search_path = self.get_search_path_for_filedir(build_path)
			self.add_framework_search_path(None, search_path)
			# add header search paths
			search_path = self.get_search_path_for_filedir(rootdir+"/Headers")
			self.add_header_search_path(None, search_path)

		return True


	def add_customization_classes(self, dep, custom_folder_name, app_name, add_classes):
		project_data = self.get_project_data()
		dep_data = dep.get_project_data()

		if project_data is None or dep_data is None:
			return False

		root_group_name = 'Classes'
		rootdir = self.find_folder_starting_at(custom_folder_name, self._project_path)
		if not rootdir:
			return False

		group_name = os.path.basename(rootdir)
		reference = os.path.relpath(rootdir,self.get_project_classes_folder())

		#add Customization group
		if add_classes:
			tthash_base = self.get_hash_base(rootdir)
			group_hash = self.add_group(group_name, tthash_base+'4', reference)
			self.add_file_to_group(group_name, group_hash, root_group_name, None)
			root_group_name = group_name
		
		rootdir = os.path.join(rootdir, app_name)
		if not os.path.isdir(rootdir):
			logging.error(app_name+" folder does not exist on Customization.")
			return False

		if add_classes:
			tthash_base = self.get_hash_base(rootdir)
			group_name = app_name
			group_hash = self.add_group(group_name, tthash_base+'4', group_name)
			self.add_file_to_group(group_name, group_hash, root_group_name, None)
			sys.stdout.write('Adding classes from: '+rootdir)
			sys.stdout.flush()
		else:
			sys.stdout.write('Adding search_paths from: '+rootdir)
			sys.stdout.flush()


		# add all inner classes from Customization/app_name
		self.add_classes(rootdir, None, add_classes)

		return True


	def add_ofunity_classes(self, dep, classes_folders, exclude_list, add_classes):
		project_data = self.get_project_data()
		dep_data = dep.get_project_data()
		root_group_name = 'Classes'
		starting_search_folder = self._project_path
		
		if project_data is None or dep_data is None:
			return False

		#add all exclusions
		for exclude_path in exclude_list:
			self.add_exclude(exclude_path)

		for class_folder in classes_folders:
			folder_dir = self.find_folder_starting_at(class_folder, starting_search_folder)
			if not folder_dir:
				#try searching from the project path
				folder_dir = self.find_folder_starting_at(class_folder, self._project_path)
				if not folder_dir:
					return False
			rootdir = os.path.realpath(os.path.join(self._project_path, folder_dir))
			# we update starting search_folder to the last found folder
			starting_search_folder = rootdir
			group_name = os.path.basename(rootdir)
			reference = os.path.relpath(rootdir,self.get_project_classes_folder())

			sys.stdout.write('Adding classes from: '+rootdir)
			sys.stdout.flush()
			tthash_base = self.get_hash_base(rootdir)
			group_hash = self.add_group(group_name, tthash_base+'4', reference)
			self.add_file_to_group(group_name, group_hash, root_group_name, None)

			self.add_classes(rootdir, None, add_classes)

			sys.stdout.write('   DONE!\n')
			sys.stdout.flush()

		
		return True			