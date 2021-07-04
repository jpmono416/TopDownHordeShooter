using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

/*
 * Old implementation of player movement capture
 */
namespace TopDownHordeShooter.Utils.Input
{
    public delegate void GameAction(EButtonState buttonState, Vector2 amount);
    public class CommandManager
    {
        private readonly InputListener _mInput;
        private readonly Dictionary<Keys, GameAction> _mKeyBindings = new Dictionary<Keys, GameAction>();
        private readonly Dictionary<MouseButton, GameAction> _mMouseButtonBindings = new Dictionary<MouseButton, GameAction>();
        public CommandManager()
        {
            _mInput = new InputListener();
            // Register events with the input listener
            _mInput.OnKeyDown += this.OnKeyDown;
            _mInput.OnKeyPressed += this.OnKeyPressed;
            _mInput.OnKeyUp += this.OnKeyUp;
            _mInput.OnMouseButtonDown += this.OnMouseButtonDown;
        } 
        
        public void Update()
        {
            // Update polling input listener, everything else is handled by events
            _mInput.Update();
        }
        
        
        public void OnKeyDown(object sender, KeyboardEventArgs e)
        {
            var action = _mKeyBindings[e.Key];
            if (action != null)
            {
                action(EButtonState.DOWN, new Vector2(1.0f));
            }
        }

        public void OnMouseButtonDown(object sender, MouseEventArgs e)
        {
            var action = _mMouseButtonBindings[e.Button];

            if (action != null)
            {
                action(EButtonState.DOWN, new Vector2(e.CurrentState.X, e.CurrentState.Y));
            }
        }
        
        public void OnKeyUp(object sender, KeyboardEventArgs e)
        {
            var action = _mKeyBindings[e.Key];
            if (action != null)
            {
                action(EButtonState.UP, new Vector2(1.0f));
            }
        }
        
        public void OnKeyPressed(object sender, KeyboardEventArgs e)
        {
            var action = _mKeyBindings[e.Key];
            if (action != null)
            {
                action(EButtonState.PRESSED, new Vector2(1.0f));
            }
        }
        
        public void AddKeyboardBinding(Keys key, GameAction action)
        {
            // Add key to listen for when polling
            _mInput.AddKey(key);
            // Add the binding to the command map
            _mKeyBindings.Add(key, action);
        }
        
        public void AddMouseBinding(MouseButton button, GameAction action)
        {
            // Add key to listen for when polling
            _mInput.AddButton(button);

            // Add the binding to the command map
            _mMouseButtonBindings.Add(button, action);
        }
    }
}