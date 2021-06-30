using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace TopDownHordeShooter.Utils.Input
{
    class InputListener
    {
        //Keyboard event handlers
        //key is down
        public event EventHandler<KeyboardEventArgs> OnKeyDown = delegate { };
        //key was up and is now down
        public event EventHandler<KeyboardEventArgs> OnKeyPressed = delegate { };
        //key was down and is now up
        public event EventHandler<KeyboardEventArgs> OnKeyUp = delegate { };

        public event EventHandler<MouseEventArgs> OnMouseButtonDown = delegate { }; 
        
        // Current and previous input states
        private KeyboardState PrevKeyboardState { get; set; }
        private KeyboardState CurrentKeyboardState { get; set; }
        private MouseState PrevMouseState { get; set; }
        private MouseState CurrentMouseState { get; set; }
        
        // List of keys to check for
        public HashSet<Keys> KeyList;
        public HashSet<MouseButton> ButtonList;
        public InputListener()
        {
            CurrentKeyboardState = Keyboard.GetState(); 
            PrevKeyboardState = CurrentKeyboardState;
            KeyList = new HashSet<Keys>();
            ButtonList = new HashSet<MouseButton>();
        }
        
        public void AddKey(Keys key)
        {
            KeyList.Add(key);
        }

        public void AddButton(MouseButton button)
        {
            ButtonList.Add(button);
        }
        public void Update()
        {
            // Update states
            PrevKeyboardState = CurrentKeyboardState;
            PrevMouseState = CurrentMouseState;
            
            // Get states
            CurrentKeyboardState = Keyboard.GetState();
            CurrentMouseState = Mouse.GetState();
            
            // Fire events
            FireKeyboardEvents();
            FireMouseEvents();
        }
        
        private void FireKeyboardEvents()
        {
            // Check through each key in the key list
            foreach (var key in KeyList)
            {
                // Is the key currently down?
                if (CurrentKeyboardState.IsKeyDown(key) && !PrevKeyboardState.IsKeyDown(key))
                {
                    // Fire the OnKeyDown event
                    if (OnKeyDown != null)
                        OnKeyDown(this, new KeyboardEventArgs(key, CurrentKeyboardState,
                            PrevKeyboardState));
                }

                // Has the key been released? (Was down and is now up)
                if (PrevKeyboardState.IsKeyDown(key) && CurrentKeyboardState.IsKeyUp(key))
                {
                    // Fire the OnKeyUp event
                    if (OnKeyUp != null)
                        OnKeyUp(this, new KeyboardEventArgs(key, CurrentKeyboardState, PrevKeyboardState));
                }

                if (PrevKeyboardState.IsKeyDown(key) && CurrentKeyboardState.IsKeyDown(key))
                {
                    // Fire KeyPress event
                    if (OnKeyPressed != null)
                        OnKeyPressed(this, new KeyboardEventArgs(key, CurrentKeyboardState, PrevKeyboardState));
                }
            }
        } 
        
        private void FireMouseEvents()
        {
            // Check through each key in the key list
            foreach (var button in from button in ButtonList
                where button == MouseButton.LEFT
                where CurrentMouseState.LeftButton == ButtonState.Pressed &&
                      PrevMouseState.LeftButton != ButtonState.Pressed
                where OnMouseButtonDown != null
                select button)
            {
                OnMouseButtonDown(this, new MouseEventArgs(button, CurrentMouseState, PrevMouseState));
            }
        }
    }
}