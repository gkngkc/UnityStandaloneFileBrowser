#include <stdio.h>
#include <gtk/gtk.h>
#include <malloc.h>
#include "library.h"

int main(int argc, char *argv[]) {
    DialogInit();
    const char* folders = DialogOpenFolderPanel("Unity Open Folder", "/", true);
    printf("Folders selected: %s\n", folders);
    const char* files = DialogOpenFilePanel("Unity Open File", "", "", true);
    printf("Files selected: %s\n", files);
    const char* filesFilters = DialogOpenFilePanel("Unity Open File With Filters", "", "Image Files;png,jpg,jpeg|Sound Files;mp3,wav|All Files;*", true);
    printf("Files selected: %s\n", filesFilters);
    return 0;
}