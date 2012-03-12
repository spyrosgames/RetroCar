
EXTRA_DIST =  Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=../../../../giraffe/test/Make

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG_X86)
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
UNITYENGINEMOCK_DLL_SOURCE=../../../../giraffe/test/UnityEngineMock/bin/Debug/UnityEngineMock.dll

endif

ifeq ($(CONFIG),RELEASE_X86)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = bin/Release/Base.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../UnityEngineMock/bin/Release/UnityEngineMock.dll
BUILD_DIR = bin/Release

BASE_DLL_MDB=
UNITYENGINEMOCK_DLL_SOURCE=../../../../giraffe/test/UnityEngineMock/bin/Debug/UnityEngineMock.dll

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(BASE_DLL_MDB) \
	$(UNITYENGINEMOCK_DLL)  

LINUX_PKGCONFIG = \
	$(BASE_PC)  


RESGEN=resgen2

UNITYENGINEMOCK_DLL = $(BUILD_DIR)/UnityEngineMock.dll
BASE_PC = $(BUILD_DIR)/base.pc

FILES = \
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
	System \
	System.Net \
	System.Security

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make



$(eval $(call emit-deploy-target,UNITYENGINEMOCK_DLL))
$(eval $(call emit-deploy-wrapper,BASE_PC,base.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'


$(ASSEMBLY_MDB): $(ASSEMBLY)
$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	make pre-all-local-hook prefix=$(prefix)
	mkdir -p $(shell dirname $(ASSEMBLY))
	make $(CONFIG)_BeforeBuild
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	make $(CONFIG)_AfterBuild
	make post-all-local-hook prefix=$(prefix)

install-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-install-local-hook prefix=$(prefix)
	make install-satellite-assemblies prefix=$(prefix)
	mkdir -p '$(DESTDIR)$(libdir)/$(PACKAGE)'
	$(call cp,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(BASE_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(UNITYENGINEMOCK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	mkdir -p '$(DESTDIR)$(libdir)/pkgconfig'
	$(call cp,$(BASE_PC),$(DESTDIR)$(libdir)/pkgconfig)
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(BASE_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(UNITYENGINEMOCK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(BASE_PC),$(DESTDIR)$(libdir)/pkgconfig)
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local

include $(top_srcdir)/rules.make
