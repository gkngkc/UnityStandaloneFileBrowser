cmake_minimum_required(VERSION 3.12)
project(StandaloneFileBrowser C)

set(CMAKE_C_STANDARD 11)

# Use the package PkgConfig to detect GTK+ headers/library files
FIND_PACKAGE(PkgConfig REQUIRED)
PKG_CHECK_MODULES(GTK3 REQUIRED gtk+-3.0)

# Setup CMake to use GTK+, tell the compiler where to look for headers
# and to the linker where to look for libraries
INCLUDE_DIRECTORIES(${GTK3_INCLUDE_DIRS})
LINK_DIRECTORIES(${GTK3_LIBRARY_DIRS})

# Add other flags to the compiler
ADD_DEFINITIONS(${GTK3_CFLAGS_OTHER})

add_library(${PROJECT_NAME} SHARED library.c library.h)

TARGET_LINK_LIBRARIES(${PROJECT_NAME} ${GTK3_LIBRARIES})

add_executable(${PROJECT_NAME}_EXE main.c)

TARGET_LINK_LIBRARIES(${PROJECT_NAME}_EXE ${PROJECT_NAME} ${GTK3_LIBRARIES})