using System.Runtime.CompilerServices;

namespace TemplateGenTests
{
    internal class WatermarkWrapper
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ActivateWatermark() => AddWatermark.API.ActivateWatermark();
    }
}
