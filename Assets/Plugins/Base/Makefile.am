
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG_X86
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG;UNITY_ANDROID;"
ASSEMBLY = bin/Debug/Base.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../../../../giraffe/test/UnityEngineMock/bin/Debug/UnityEngineMock.dll
BUILD_DIR = bin/Debug

BASE_DLL_MDB_SOURCE=bin/Debug/Base.dll.mdb
BASE_DLL_MDB=$(BUILD_DIR)/Base.dll.mdb
UNITYENGINEMOCK_DLL_SOURCE=../UnityEngineMock/bin/Debug/UnityEngineMock.dll
UNITYENGINEMOCK_DLL_MDB_SOURCE=../UnityEngineMock/bin/Debug/UnityEngineMock.dll.mdb
UNITYENGINEMOCK_DLL_MDB=$(BUILD_DIR)/UnityEngineMock.dll.mdb

endif

if ENABLE_RELEASE_X86
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = bin/Release/Base.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../../../../giraffe/test/UnityEngineMock/bin/Debug/UnityEngineMock.dll
BUILD_DIR = bin/Release

BASE_DLL_MDB=
UNITYENGINEMOCK_DLL_SOURCE=../../../../giraffe/test/UnityEngineMock/bin/Debug/UnityEngineMock.dll
UNITYENGINEMOCK_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(BASE_DLL_MDB) \
	$(UNITYENGINEMOCK_DLL) \
	$(UNITYENGINEMOCK_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(BASE_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES =  \
	*.cs \
	PropertiesPlaceHolder/*.cs
#	PropertiesPlaceHolder/PropertiesLoader.cs \
#	CallbackFetcherBase.cs \
#	CallbackFetcherFactory.cs \
#	CallbackFetcherProperties.cs \
#	Define.cs \
#	GameObjectManagerInitializer.cs \
#	ICallback.cs \
#	IUnityToNativeMessage.cs \
#	minijson_143.cs \
#	NativeToUnity.cs \
#	UnityToJavaMessage.cs \
#	UnityToNative.cs \
#	UnityToNativeMessageManager.cs \
#	UnityToObjCMessage.cs \
#	UnityToPlatformUnsupportedMessage.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	PropertiesPlaceHolder/properties.json.txt \
	base.pc.in 

REFERENCES =  \
	../../../../giraffe/test/UnityEngineMock/bin/Debug/UnityEngineMock.dll \
	System \
	System.Net \
	System.Security

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

UNITYENGINEMOCK_DLL = $(BUILD_DIR)/UnityEngineMock.dll
BASE_PC = $(BUILD_DIR)/base.pc

$(eval $(call emit-deploy-target,UNITYENGINEMOCK_DLL))
$(eval $(call emit-deploy-target,UNITYENGINEMOCK_DLL_MDB))
$(eval $(call emit-deploy-wrapper,BASE_PC,base.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
