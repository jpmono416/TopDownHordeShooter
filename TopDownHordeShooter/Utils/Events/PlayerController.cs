namespace TopDownHordeShooter.Utils.Events
{
    public delegate void PlayerEventHandler(object sender, PlayerEventArgs args);
    
    public class PlayerController : BaseEventController
    {
        public event PlayerEventHandler Changed;

        protected virtual void OnChanged(PlayerEventArgs args)
        {
            Changed?.Invoke(this, args);
        }

        public void MoveLeft(PlayerEventArgs args) => OnChanged(args);
        public void MoveRight(PlayerEventArgs args) => OnChanged(args);
        public void MoveUp(PlayerEventArgs args) => OnChanged(args);
        public void MoveDown(PlayerEventArgs args) => OnChanged(args);
        public void Reload(PlayerEventArgs args) => OnChanged(args);
        public void NextWeapon(PlayerEventArgs args) => OnChanged(args); 
        
        public void PreviousWeapon(PlayerEventArgs args) => OnChanged(args);
        public void Shoot(PlayerEventArgs args) => OnChanged(args);
    }
}