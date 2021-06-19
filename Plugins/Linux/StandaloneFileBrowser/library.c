#define _GNU_SOURCE
#include <stddef.h>
#include <string.h>
#include <malloc.h>
#include <stdbool.h>
#include <unistd.h>
#include <stdlib.h>
#include <assert.h>
#include <sys/mman.h>
#include <sys/wait.h>
#include <sys/syscall.h>

#include "library.h"

typedef void (*callbackFunc)(const char *);

extern char _binary_build_dialog_start[];
extern char _binary_build_dialog_end[];

char *dialogExecutablePath;

void DialogInit() {
    // Avoid linking against higher glibc ABI version
    // int fd = memfd_create("Menci", MFD_CLOEXEC);
    int fd = syscall(__NR_memfd_create, "Menci", MFD_CLOEXEC);
    assert(fd != 0);
    write(fd, _binary_build_dialog_start, _binary_build_dialog_end - _binary_build_dialog_start);
    asprintf(&dialogExecutablePath, "/proc/self/fd/%d", fd);
}

void readn(pid_t pid, int fd, void *buffer, size_t n) {
    while (n) {
        int status;
        if (waitpid(pid, &status, WNOHANG) == pid) {
            assert(("The child process exited unexpectedly. Please refer to the log in stderr.", false));
        }
        ssize_t r;
        assert((r = read(fd, buffer, n)) >= 0);
        n -= r;
        buffer = (void *)((char *)buffer + r);
    }
}

const char* runSubprocess(const char **args) {
    args[0] = "Menci";

    int pipeFd[2];
    assert(pipe(pipeFd) != -1);

    pid_t pid = fork();
    assert(pid != -1);
    if (pid == 0) {
        assert(dup2(pipeFd[1], STDOUT_FILENO) != -1);
        execv(dialogExecutablePath, (char* const*)args);
        perror("execv");
        abort();
    } else {
        assert(close(pipeFd[1]) == 0);

        size_t size;
        readn(pid, pipeFd[0], &size, sizeof(size));

        char *result = malloc(size + 1);
        readn(pid, pipeFd[0], result, size);
        result[size] = '\0';

        assert(close(pipeFd[0]) == 0);

        int status;
        waitpid(pid, &status, 0);
        assert(WIFEXITED(status) && WEXITSTATUS(status) == 0);

        return result;
    }
}

const char* DialogOpenFilePanel(const char* title, const char* directory, const char* extension,
                                bool multiselect) {
    const char *args[] = {NULL, "DialogOpenFilePanel", title, directory, extension, multiselect ? "1" : "0", NULL};
    return runSubprocess(args);
}

const char* DialogOpenFolderPanel(const char* title, const char* directory, bool multiselect) {
    const char *args[] = {NULL, "DialogOpenFolderPanel", title, directory, multiselect ? "1" : "0", NULL};
    return runSubprocess(args);
}

const char* DialogSaveFilePanel(const char* title, const char* directory, const char* defaultName,
                                const char* filters) {
    const char *args[] = {NULL, "DialogSaveFilePanel", title, directory, defaultName, filters, NULL};
    return runSubprocess(args);
}

void DialogOpenFilePanelAsync(const char* title, const char* directory, const char* extension,
                              bool multiselect, callbackFunc cb) {
    // TODO Add async capability
    cb(DialogOpenFilePanel(title, directory, extension, multiselect));

}

void DialogOpenFolderPanelAsync(const char* title, const char* directory, bool multiselect,
                                callbackFunc cb) {
    // TODO Add async capability
    cb(DialogOpenFolderPanel(title, directory, multiselect));

}

void DialogSaveFilePanelAsync(const char* title, const char* directory, const char* defaultName,
                              const char* filters, callbackFunc cb) {
    // TODO Add async capability
    cb(DialogSaveFilePanel(title, directory, defaultName, filters));
}
