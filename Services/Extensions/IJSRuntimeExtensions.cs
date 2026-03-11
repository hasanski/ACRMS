using Microsoft.JSInterop;

namespace ACRMS.Services.Extensions
{
    public static class IJSRuntimeExtensions
    {
        public static async Task ToastrSuccess(this IJSRuntime js, string message)
        {
            await js.InvokeVoidAsync("ShowToastr", "success", message);
        }

        public static async Task ToastrError(this IJSRuntime js, string message)
        {
            await js.InvokeVoidAsync("ShowToastr", "error", message);
        }
        public static async Task ToastrInfo(this IJSRuntime js, string message)
        {
            await js.InvokeVoidAsync("ShowToastr", "info", message);
        }
        // toastr.info('info messages');
    }
}
