namespace TopDownHordeShooter.Utils.Events
{
    public delegate void PickupEventHandler(object sender, PickupEventArgs args);
    
    public sealed class PickupController : BaseEventController
    {
        public event PickupEventHandler Changed;

        private void OnChanged(PickupEventArgs args) => Changed?.Invoke(this, args);

        public void Spawn(PickupEventArgs args) => OnChanged(args); 

        public void Collected(PickupEventArgs args) => OnChanged(args); 
        
        public void ApplyEffect(PickupEventArgs args) => OnChanged(args);
    }
}