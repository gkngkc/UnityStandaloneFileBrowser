using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace SFB
{
    public static class AwaitableStandaloneFileBrowser
    {
        public static UniTask<string[]> OpenFilePanelAsync(string title, string directory, string extension, bool multiselect, CancellationToken cancellationToken = default)
            => OpenFilePanelAsync(title, directory, new[] { new ExtensionFilter("", extension) }, multiselect, cancellationToken);

        public static async UniTask<string[]> OpenFilePanelAsync(string title, string directory, IEnumerable<ExtensionFilter> extensions, bool multiselect, CancellationToken cancellationToken = default)
        {
            var completionSource = new UniTaskCompletionSource<string[]>();
            StandaloneFileBrowser.OpenFilePanelAsync(title, directory, extensions.ToArray(), multiselect, paths =>
            {
                if (paths.Length == 0 || string.IsNullOrEmpty(paths[0]))
                {
                    completionSource.TrySetCanceled();
                    return;
                }
                completionSource.TrySetResult(paths);
            });
            var paths = await completionSource.Task;
            cancellationToken.ThrowIfCancellationRequested();
            return paths;
        }

        public static async UniTask<string[]> OpenFolderPanelAsync(string title, string directory, bool multiselect, CancellationToken cancellationToken = default)
        {
            var completionSource = new UniTaskCompletionSource<string[]>();
            StandaloneFileBrowser.OpenFolderPanelAsync(title, directory, multiselect, strings =>
            {
                if (strings.Length == 0 || string.IsNullOrEmpty(strings[0]))
                {
                    completionSource.TrySetCanceled();
                    return;
                }
                completionSource.TrySetResult(strings);
            });
            var paths = await completionSource.Task;
            cancellationToken.ThrowIfCancellationRequested();
            return paths;
        }

        public static UniTask<string> SaveFilePanelAsync(string title, string directory, string defaultName, string extension, CancellationToken cancellationToken = default)
            => SaveFilePanelAsync(title, directory, defaultName, new[] { new ExtensionFilter("", extension) }, cancellationToken);

        public static async UniTask<string> SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions, CancellationToken cancellationToken = default)
        {
            var completionSource = new UniTaskCompletionSource<string>();
            StandaloneFileBrowser.SaveFilePanelAsync(title, directory, defaultName, extensions, path =>
            {
                if (string.IsNullOrEmpty(path))
                {
                    completionSource.TrySetCanceled();
                    return;
                }
                completionSource.TrySetResult(path);
            });
            var result = await completionSource.Task;
            cancellationToken.ThrowIfCancellationRequested();
            return result;
        }
    }
}
