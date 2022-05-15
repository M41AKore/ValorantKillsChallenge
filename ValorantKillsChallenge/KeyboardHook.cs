using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ValorantKillsChallenge
{
    /**
        * Credit: https://stackoverflow.com/a/27309185 
    */
    public sealed class KeyboardHook : IDisposable
    {       
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk); // Registers a hot key with Windows
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id); // Unregisters the hot key with Windows

        private class Window : NativeWindow, IDisposable
        {
            private static int WM_HOTKEY = 0x0312;

            public Window()
            {
                this.CreateHandle(new CreateParams());
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (m.Msg == WM_HOTKEY)
                {                  
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);
                    
                    if (KeyPressed != null) KeyPressed(this, new KeyPressedEventArgs(modifier, key)); // invoke the event to notify the parent.
                }
            }

            public event EventHandler<KeyPressedEventArgs> KeyPressed;

            #region IDisposable Members
            public void Dispose()
            {
                this.DestroyHandle();
            }
            #endregion
        }

        private Window _window = new Window();
        private int _currentId;

        public KeyboardHook() // register the event of the inner native window.
        {        
            _window.KeyPressed += delegate (object? sender, KeyPressedEventArgs args)
            {
                if (KeyPressed != null) KeyPressed(this, args);
            };
        }

        public void RegisterHotKey(ModifierKeys modifier, Keys key) // Registers a hot key in the system
        {           
            _currentId = _currentId + 1; // increment the counter

            if (!RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key)) // register the hot key.
                throw new InvalidOperationException("Couldn’t register the hot key.");
        }

        public event EventHandler<KeyPressedEventArgs> KeyPressed; // A hot key has been pressed

        public void UnregisterHotkeys() // unregister all the registered hot keys -> CANT ADD NEW ONES EITHER THO, DISPOSES EVERYTHING
        {
            for (int i = _currentId; i > 0; i--)
                UnregisterHotKey(_window.Handle, i);
        }

        #region IDisposable Members
        public void Dispose()
        {
            UnregisterHotkeys();
            _window.Dispose(); // dispose the inner native window.
        }
        #endregion
    }

    /// <summary>
    /// Event Args for the event that is fired after the hot key has been pressed.
    /// </summary>
    public class KeyPressedEventArgs : EventArgs
    {
        private ModifierKeys _modifier;
        private Keys _key;

        internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
        {
            _modifier = modifier;
            _key = key;
        }

        public ModifierKeys Modifier => _modifier;
        public Keys Key => _key; 
    }

    /// <summary>
    /// The enumeration of possible modifiers.
    /// </summary>
    [Flags]
    public enum ModifierKeys : uint
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }
}
