namespace SRCSharpForm.Lib
{
    public static class Windows
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern short GetKeyState(int nVirtKey);
    }
}
