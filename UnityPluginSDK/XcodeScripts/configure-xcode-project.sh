#!/bin/sh

show_help () {
	echo ""
	echo "Usage $0 [-p <xcodeproj directory>] [-x <xcodeproj file>] [-n <app name>][-c <source directory>] [-t <target name>] [-s] [-h] "
	echo ""
	echo "Generate the Giraffe library. Valid options are:"
	echo "	-p <xcodeproj directory>: The project's directory name or the full path to it. If name is given the script will try to find the folder on the system."
	echo "  -x <xcodeproj file> 	: The name of the project file or the full path to it. If name, it will start searching from the xcodeproj directory (if arg given)"
	echo "	-n <app name>	    	: The name of the application this script is used on."
	echo "	-c <source directory> 	: Copy the code from the source directory into the project folder"
	echo "	-t <target name>		: The target name to apply the changes to. By default this value will be the same as the xcodeproj name if provided."
	echo "	-s 						: Add search paths, libraries and frameworks only. It will not add classes into the project."
	echo "  -f <OF framework name>  : Set the name of the OF framework to use."
	echo "	-a 						: Include all sdks"
	echo "	-h 						: Show this help"
	exit
}

max_path_depth () {
	echo $1 | grep -o "/" | wc -l | sed s/\ //g
}

find_directory () {
	found=false
	path=${1}

	#to stop the search and don't make it last forever
	max_level_allowed=$(max_path_depth ${1})
	let max_level_allowed=max_level_allowed-1
	max_level=0
	while [ $found == false ]; do
		all=`find $path -type d -name ${2}`
		if test -z "$all"
		then
			path="$path/.."
			let max_level=max_level+1
			if (($max_level >= 3)); then
				echo "Not Found"
				return 1
			fi
			if (($max_level >= $max_level_allowed)); then
				echo "Not Found"
				return 1
			fi
		else
			ret=${all[0]}
			for file_path in $all; do
				if [ ${#file_path} -lt ${#ret} ]; then
					ret=$file_path
				fi
			done
			echo $ret
			project_path=$ret
			found=true
		fi
	done

	return 0
}

#default values
project_file="Unity-iPhone"
source_directory="UnityPluginSDK"
project_directory="OpenFeintSample"
app_name="MyApp"
target_name="Unity-iPhone"
classes_directory="Classes"
copy=false
add_classes=true
all_sdks=false
of_framework="OFUnity"

#get options from arguments
while getopts "p:x:n:c:t:f:sah" opt
do
	case $opt in
	p) project_directory=$OPTARG ;;
	x) project_file=$OPTARG ;;
	n) app_name=$OPTARG ;;
	c) source_directory=$OPTARG
	copy=true ;;
	t) target_name=$OPTARG ;;
	f) of_framework=$OPTARG ;;
	s) add_classes=false ;;
	a) all_sdks=true ;;
	*) show_help
	esac
done

if [[ ! "$project_file" == "*\.*" ]]; then
	project_file="$project_file.xcodeproj"
fi

cwd=$(pwd)
if [ ! -d "$project_file" ]; then

	#if project_file is not an full path search first for the project directory starting at cwd
	echo "Searching for directory $project_directory..."
	project_path=$(find_directory $cwd $project_directory)

	if [ -d "$project_path" ]; then
		project_path=`cd $project_path; pwd`
		echo "Found at $project_path"
		echo
	else
		echo "Not Found."
		project_path=$cwd
	fi

	#search for the project file starting on the project directory (if it was found)
	echo "Searching for project file $project_file..."
	project_path=$(find_directory $project_path $project_file)
	if [ ! -d "$project_path" ]
	then
		project_path=`cd $project_path; pwd`
		target_directory=$(dirname $project_path)
		project_file=$(basename $project_path)
		echo "$project_file not found."
		echo "EXIT CODE 2"
		exit 2	
	else
		echo "Found at $project_path"
		echo
	fi
fi

# check if the path ends with .xcodeproj
if [[  ! ${project_path: -10} == ".xcodeproj" ]]; then
	echo "Invalid xcodeproj file at $project_path. Aborting"
	exit 2
fi

# copy all sources to project classes
if [ "$copy" == true ]; then
	if [ ! -d "$source_directory" ]; then
		echo "Searching for directory $source_directory..."
		source_directory=$(find_directory $cwd $source_directory)
		if [ ! -d "$source_directory" ]
		then
			source_directory=`cd $source_directory; pwd`
			echo "$source_directory not found."
			echo "EXIT CODE 3"
			exit 3	
		else
			echo "Found at $source_directory"
			echo
		fi
	fi

	proj_classes_directory="$target_directory/$classes_directory"
	dirlist=$(ls $source_directory)
	for dir in $dirlist ; do
		source_dir="$source_directory/$dir"
		if [ -d $source_dir ]; then
			target_dir="$target_directory/$dir"
			if [ -d $target_dir ]; then
				echo $target_dir "already exists, skip copy."
			else
				echo "Copying files from $source_dir to $target_dir."
				cp -R $source_dir $target_dir
			fi
		fi 
	done

fi

#call the python script
echo "Modifying xcodeproj file..."
cmd="python OFPbxprojSetup.py -p $project_path -t $target_name -n $app_name"
if [ ! -z "$of_framework" ]; then
	cmd="$cmd -f $of_framework"
fi
if [ "$add_classes" == false ]; then
	cmd="$cmd -s"
fi
if [ "$all_sdks" == true ]; then
	cmd="$cmd -a"
fi

eval $cmd