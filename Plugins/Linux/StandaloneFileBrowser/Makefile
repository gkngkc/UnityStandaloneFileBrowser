LD = ld
CC = gcc
COPTS = -O2 -Wall -Wextra

.PHONY: all
all: build/main build/libStandaloneFileBrowser.so

build/dialog: dialog.c
	mkdir -p build
	cd build && cmake .. -DCMAKE_BUILD_TYPE=Release && make
build/dialog.bin.o: build/dialog
	$(LD) -r -b binary -o build/dialog.bin.o build/dialog
build/library.o: library.c library.h
	$(CC) $(COPTS) -o build/library.o -c library.c -fPIC
build/libStandaloneFileBrowser.so: build/dialog.bin.o build/library.o
	$(CC) $(COPTS) -o build/libStandaloneFileBrowser.so build/library.o build/dialog.bin.o -shared
build/main: build/libStandaloneFileBrowser.so main.c library.h
	$(CC) $(COPTS) -o build/main main.c build/libStandaloneFileBrowser.so

.PHONY: clean
clean:
	rm -rf build
